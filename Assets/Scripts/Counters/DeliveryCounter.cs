using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class DeliveryCounter : BaseCounter
{
    public static event EventHandler OnRecipeSuccess;
    public static event EventHandler OnRecipeFailed;

    private static int successfulRecipeAmount;

    private void Start()
    {
        successfulRecipeAmount = 0;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // Player has something
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //DeliveryManager.Instance.DeliverRecipe(plateKitchenObject, this);
                // That something is a Plate
                if (DeliveryManager.Instance.TryDeliverRecipe(plateKitchenObject))
                {
                    //DeliveryManager.Instance.DeliverCorrectRecipeServerRpc();
                    DeliverCorrectRecipeServerRpc();
                }
                else
                {
                    DeliverIncorrectRecipeServerRpc();
                }
                KitchenObject.DestoryKitchenObject(player.GetKitchenObject());
            }
        }
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
    public void DeliverCorrectRecipeServerRpc()
    {
        DeliverCorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc()
    {
        successfulRecipeAmount++;
        //OnRecipeComplete?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public static int GetSuccessfulRecipeAmount()
    {
        return successfulRecipeAmount;
    }
}
