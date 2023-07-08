using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Burning,
        Burnt
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);

    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        state.Value = State.Idle;
    }
    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChange;
        burningTimer.OnValueChanged += BurningTimer_OnValueChange;
        state.OnValueChanged += State_OnValueChange;
    }
    private void State_OnValueChange(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });
        if (state.Value == State.Burnt || state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }
    private void FryingTimer_OnValueChange(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimeMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });
    }
    private void BurningTimer_OnValueChange(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimeMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        switch (state.Value)
        {
            case State.Idle:
                break;
            case State.Frying:
                if (HasKitchenObject())
                {
                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value >= fryingRecipeSO.fryingTimeMax)
                    {
                        // Fried
                        fryingTimer.Value = 0f;
                        KitchenObject.DestoryKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        state.Value = State.Burning;
                        burningTimer.Value = 0f;
                        SetBurningRecipeClientRpc(
                            GameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                        );
                    }
                }
                break;
            case State.Burning:
                if (HasKitchenObject())
                {
                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value >= burningRecipeSO.burningTimeMax)
                    {
                        // Burned
                        KitchenObject.DestoryKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state.Value = State.Burnt;
                    }
                }
                break;
            case State.Burnt:
                break;
        }
    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // Counter has no KitchenObject
            if (player.HasKitchenObject())
            {
                // Player has KitchenObject
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Player has KitchenObject that can be fried
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc(
                        GameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
                    );
                }
            }
            else
            {
                // Player has nothing
            }
        }
        else
        {
            // Counter has KitchenObject
            if (player.HasKitchenObject())
            {
                // Player has KitchenObject
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKicthenObject))
                {
                    // Player is holding a plate
                    PlateKitchenObject plateKitchenObject = player.GetKitchenObject() as PlateKitchenObject;

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // Plate does not have the ingredient already\
                        KitchenObject.DestoryKitchenObject(GetKitchenObject());

                        state.Value = State.Idle;


                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else
            {
                //Player has nothing
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStateIdleServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kithchenObjectSOIndex)
    {
        SetFryingRecipeClientRpc(kithchenObjectSOIndex);
        fryingTimer.Value = 0f;
        state.Value = State.Frying;
    }
    [ClientRpc]
    private void SetFryingRecipeClientRpc(int kithchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = GameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kithchenObjectSOIndex);

        fryingRecipeSO = GetFryingRecipeOutputFromInput(kitchenObjectSO);
    }
    [ClientRpc]
    private void SetBurningRecipeClientRpc(int kithchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = GameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kithchenObjectSOIndex);

        burningRecipeSO = GetBurningRecipeOutputFromInput(kitchenObjectSO);
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeOutputFromInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }
    private KitchenObjectSO GetOutputFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeOutputFromInput(inputKitchenObjectSO);
        return fryingRecipeSO.output;
    }
    private FryingRecipeSO GetFryingRecipeOutputFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurningRecipeSO GetBurningRecipeOutputFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }
    public bool IsFried()
    {
        return state.Value == State.Burning;
    }

    public override void OnDestroy()
    {
        fryingTimer.OnValueChanged -= FryingTimer_OnValueChange;
        burningTimer.OnValueChanged -= BurningTimer_OnValueChange;
        state.OnValueChanged -= State_OnValueChange;
    }
}
