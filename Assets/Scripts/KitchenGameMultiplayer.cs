using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO _kitchenObjectListSO;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSo), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSo)
    {
        return _kitchenObjectListSO.kitchenObjectSoList.IndexOf(kitchenObjectSo);
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
       return _kitchenObjectListSO.kitchenObjectSoList[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKithcenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKithcenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenNetworkObject);
        KitchenObject kitchenObject = kitchenNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenNetworkObject);
        KitchenObject kitchenObject = kitchenNetworkObject.GetComponent<KitchenObject>();
        
        kitchenObject.ClearKitchenObjectOnParent();
    } 
}