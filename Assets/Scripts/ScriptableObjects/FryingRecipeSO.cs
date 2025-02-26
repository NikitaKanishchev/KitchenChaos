using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu()]
    public class FryingRecipeSO : ScriptableObject
    {
        public KitchenObjectSO inpit;
    
        public KitchenObjectSO output;

        public float fryingTimerMax;
    }
}
