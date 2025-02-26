using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO inpit;
    
    public KitchenObjectSO output;

    public int cuttingProgressMax;
}
