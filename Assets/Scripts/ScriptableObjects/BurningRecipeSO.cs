using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu()]
    public class BurningRecipeSO : ScriptableObject
    {
        public KitchenObjectSO inpit;
    
        public KitchenObjectSO output;

        public float burningTimerMax;
    }
}
