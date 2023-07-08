using System;
using Unity.Netcode;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateSpawn;
    public event EventHandler OnPlateRemove;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        spawnPlateTimer += Time.deltaTime;
        if (GameManager.Instance.IsGamePlaying() && spawnPlateTimer > spawnPlateTimerMax && platesSpawnedAmount < platesSpawnedAmountMax)
        {
            SpawnPlateServerRpc();
        };
    }
    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmount++;
        spawnPlateTimer = 0f;

        OnPlateSpawn?.Invoke(this, EventArgs.Empty);
    }
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // Player has nothing
            if (platesSpawnedAmount > 0)
            {
                // Plate Counter has at lest one plate
                // Reset time so it doesn't spawn a plate immediately after picking one up when plate counter is full
                if (platesSpawnedAmount == 4)
                {
                    spawnPlateTimer = 0f;
                }

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                InteractLogicServerRpc();
            }
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
        platesSpawnedAmount--;

        OnPlateRemove?.Invoke(this, EventArgs.Empty);
    }
}
