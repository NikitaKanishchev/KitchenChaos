using System;
using Unity.Netcode;
using UnityEngine;

namespace Counter
{
    public class PlatesCounter : BaseCounter
    {
        public event EventHandler OnPlateSpawned;
        public event EventHandler OnPlateRemoved;

        [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

        private int _platesSpawnedAmount;

        private int _platesSpawnedAmountMax = 4;

        private float _spawnPlateTimer;

        private float _spawnPlateTimerMax = 4f;

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }
            
            _spawnPlateTimer += Time.deltaTime;
            if (_spawnPlateTimer > _spawnPlateTimerMax)
            {
                _spawnPlateTimer = 0f;

                if (GameManager.Instance.IsGamePlaying() && _platesSpawnedAmount < _platesSpawnedAmountMax)
                {
                    SpawnPlateServerRpc();
                }
            }
        }

        [ServerRpc]
        private void SpawnPlateServerRpc()
        {
         SpawnPlateClientRpc();   
        }

        [ClientRpc]
        private void SpawnPlateClientRpc()
        {
            _platesSpawnedAmount++;
                    
            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        }

        public override void Interact(Player player)
        {
            if (!player.HasKitchenObject())
            {
                //Player is empty handed
            }

            if (_platesSpawnedAmount > 0)
            {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

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
            _platesSpawnedAmount--;
            
            OnPlateRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}