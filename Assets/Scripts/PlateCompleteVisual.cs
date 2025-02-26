using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    private struct KitchenObjectSo_GameObject
    {
        public KitchenObjectSO kitchenObjectSo;
        public GameObject gameObject;
    }
    
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSo_GameObject> _kitchenObjectSOGameObjecList;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        foreach (KitchenObjectSo_GameObject kitchenObjectSoGameObject in _kitchenObjectSOGameObjecList)
            kitchenObjectSoGameObject.gameObject.SetActive(false);
    }
    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitchenObjectSo_GameObject kitchenObjectSoGameObject in _kitchenObjectSOGameObjecList)
        {
            if (kitchenObjectSoGameObject.kitchenObjectSo == e.KitchenObjectSo)
                kitchenObjectSoGameObject.gameObject.SetActive(true);
        }
    }
}