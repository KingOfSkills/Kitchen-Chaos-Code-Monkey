using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrash;

    new public static void ResetStaticData()
    {
        OnAnyObjectTrash = null;
    }
    public override void Interact(Player player)
    {
        if(player.HasKitchenObject())
        {
            KitchenObject.DestoryKitchenObject(player.GetKitchenObject());

            InteractLogicServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnAnyObjectTrash?.Invoke(this, EventArgs.Empty);

    }
}
