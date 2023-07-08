using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeComplete;
    // Did not use Instance because I might want to use multiple DeliveryCounter in the future so
    // so I did it like CuttintCunters
    //public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    private static int successfulRecipeAmount;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    List<RecipeSO> waitingRecipeSOList;
    [SerializeField] private int waitingRecipeSOListMax = 4;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }
    //private void Start()
    //{
    //    GameManager.Instance.OnStateChange += GameManager_OnStateChange;
    //}

    //private void GameManager_OnStateChange(object sender, EventArgs e)
    //{
    //    if (GameManager.Instance.IsGamePlaying())
    //    {
    //        int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

    //        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];//UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

    //        waitingRecipeSOList.Add(waitingRecipeSO);

    //        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    //    }
    //}

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        if (waitingRecipeSOList.Count < waitingRecipeSOListMax)
        {
            spawnRecipeTimer -= Time.deltaTime;
            if (GameManager.Instance.IsGamePlaying() && spawnRecipeTimer <= 0)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
                spawnRecipeTimer = spawnRecipeTimerMax;
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];//UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    //public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    //{
    //    for (int i = 0; i < waitingRecipeSOList.Count; i++)
    //    {
    //        RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

    //        if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
    //        {
    //            // Have same number of ingredients
    //            bool allIngredientsMatches = true;
    //            foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
    //            {
    //                // Cycles through all ingredients in recipe
    //                bool foundIngredient = false;
    //                foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
    //                {
    //                    // Cycles through all ingredients on plate
    //                    if (plateKitchenObjectSO == recipeKitchenObjectSO)
    //                    {
    //                        foundIngredient = true;
    //                        break;
    //                    }
    //                }
    //                if (!foundIngredient)
    //                {
    //                    allIngredientsMatches = false;
    //                }
    //            }
    //            if (allIngredientsMatches)
    //            {
    //                DeliverCorrectRecipeServerRpc(i);
    //                return;
    //            }
    //        }
    //    }
    //    // No matches
    //    DeliverIncorrectRecipeServerRpc();
    //    //Debug.Log("Player did NOT deliver the correct recipe!");
    //}

    public bool TryDeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // Have same number of ingredients
                bool allIngredientsMatches = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    // Cycles through all ingredients in recipe
                    bool foundIngredient = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // Cycles through all ingredients on plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            foundIngredient = true;
                            break;
                        }
                    }
                    if (!foundIngredient)
                    {
                        allIngredientsMatches = false;
                    }
                }
                if (allIngredientsMatches)
                {
                    //DeliverCorrectRecipeServerRpc(i);

                    //waitingRecipeSOList.RemoveAt(i);
                    //Debug.Log("Player delivered the correct recipe!");
                    //OnRecipeComplete?.Invoke(this, EventArgs.Empty);
                    //OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    DeliverCorrectRecipeServerRpc(i);
                    return true;
                }
            }
        }
        // No matches
        //OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        return false;
        //Debug.Log("Player did NOT deliver the correct recipe!");
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        successfulRecipeAmount++;

        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeComplete?.Invoke(this, EventArgs.Empty);
        //OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    private void SpawnRecipe(int waitingRecipeSOIndex)
    {

    }

    public static int GetSuccessfulRecipeAmount()
    {
        return successfulRecipeAmount;
    }
}
