using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class RecipeSO : ScriptableObject
    {
        public List<KitchenObjectSO> kitchenObjectSOList;

        public string recipeName;
    }
}
