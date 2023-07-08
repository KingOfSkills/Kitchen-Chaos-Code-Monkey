using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnCut;

    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // Counter has no KitchenObject
            if (player.HasKitchenObject())
            {
                // Player has KitchenObject
                //if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                //{
                //    // Player has KitchenObject that can be cut
                //    KitchenObject kitchenObject = player.GetKitchenObject();
                //    kitchenObject.SetKitchenObjectParent(this);

                //    InteractLogicPlaceObjectOnCounterServerRpc();
                //}

                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);

                InteractLogicPlaceObjectOnCounterServerRpc();
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
                        // Plate does not have the ingredient already
                        KitchenObject.DestoryKitchenObject(GetKitchenObject());

                    }
                }
            }
            else
            {
                //Player has nothing
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }
    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }
    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // Has something on it
            CutObjectServerRpc();
            CuttingProcessDoneServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectClientRpc();
        }
    }
    [ClientRpc]
    private void CutObjectClientRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeOutputFromInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });


            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void CuttingProcessDoneServerRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeOutputFromInput(GetKitchenObject().GetKitchenObjectSO());

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputFromInput(GetKitchenObject().GetKitchenObjectSO());

                KitchenObject.DestoryKitchenObject(GetKitchenObject());

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeOutputFromInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }
    private KitchenObjectSO GetOutputFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeOutputFromInput(inputKitchenObjectSO);
        return cuttingRecipeSO.output;
    }
    private CuttingRecipeSO GetCuttingRecipeOutputFromInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
