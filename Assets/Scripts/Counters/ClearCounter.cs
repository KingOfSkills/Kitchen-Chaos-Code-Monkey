using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // Counter has no KitchenObject
            if (player.HasKitchenObject())
            {
                // Player has KitchenObject
                player.GetKitchenObject().SetKitchenObjectParent(this);
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // Plate does not have the ingredient already
                        KitchenObject.DestoryKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    //Player is not carrying Plate
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // Counter has Plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            // Plate does not have the ingredient already
                            KitchenObject.DestoryKitchenObject(player.GetKitchenObject());
                        }
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
}
