using UnityEngine;

namespace Counter
{
    public class ClearCounter : BaseCounter
    {
        [SerializeField] private KitchenObjectSO kitchenObjectSO;

        public override void Interact(Player player)
        {
            if (!HasKitchenObject())
            {
                if (player.HasKitchenObject())
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                else
                {
                 //Player not carrying anything   
                }
            }
            else
            {
                if (player.HasKitchenObject())
                {
                    if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKithcenObjectSO()))
                            GetKitchenObject().DestroySelf();
                    }
                    else
                    {
                        if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                        {
                            if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKithcenObjectSO()))
                                player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
                else
                    GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
