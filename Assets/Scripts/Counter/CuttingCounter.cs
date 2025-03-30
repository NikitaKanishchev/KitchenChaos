using System;
using Unity.Netcode;
using UnityEngine;

namespace Counter
{
    public class CuttingCounter : BaseCounter, IHasProgress
    {
        public static event EventHandler OnAnyCut;

        public new static void ResetStaticData()
        {
            OnAnyCut = null;
        }
        
        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

        public event EventHandler Oncut;

        [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

        private int _cuttingProgress;

        public override void Interact(Player player)
        {
            if (!HasKitchenObject())
            {
                if (player.HasKitchenObject())
                {
                    if (HasRecipeWithInput(player.GetKitchenObject().GetKithcenObjectSO()))
                    {
                        KitchenObject kitchenObject = player.GetKitchenObject();
                        
                        kitchenObject.SetKitchenObjectParent(this);
                        
                        InteractLogicPlaceObjectOnCounterClientRpc();
                       
                    }
                }
                else
                {
                    //Player not carrying anything   
                }
            }
            else
            {
                if (player.HasKitchenObject())
                {
                    //Player is carrying something
                    if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKithcenObjectSO()))
                            KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
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
            _cuttingProgress = 0;


            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormilized = 0f
            });
        }

        public override void InteractAlternate(Player player)
        {
            if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKithcenObjectSO()))
            {
                CutObjectServerRpc();
                TestCuttingProgressDontServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void CutObjectServerRpc()
        {
            CutObjectClientRpc();
        }

        [ClientRpc]
        private void CutObjectClientRpc()
        {
            _cuttingProgress++;

            Oncut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKithcenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormilized = (float) _cuttingProgress / cuttingRecipeSo.cuttingProgressMax
            });

            
        }

        [ServerRpc(RequireOwnership = false)]
        private void TestCuttingProgressDontServerRpc()
        {
            CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKithcenObjectSO());
            
            if (_cuttingProgress >= cuttingRecipeSo.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSo = GetOutputForInput(GetKitchenObject().GetKithcenObjectSO());

                KitchenObject.DestroyKitchenObject(GetKitchenObject());

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSo, this);
            }
        }

        private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSOWithInput(inputKitchenObjectSo);
            return cuttingRecipeSo != null;
        }

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
        {
            CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSOWithInput(inputKitchenObjectSo);
            if (cuttingRecipeSo != null)
            {
                return cuttingRecipeSo.output;
            }
            else
            {
                return null;
            }
        }

        private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            foreach (CuttingRecipeSO cuttingRecipeSo in cuttingRecipeSOArray)
            {
                if (cuttingRecipeSo.inpit == inputKitchenObjectSo)
                {
                    return cuttingRecipeSo;
                }
            }

            return null;
        }
    }
}