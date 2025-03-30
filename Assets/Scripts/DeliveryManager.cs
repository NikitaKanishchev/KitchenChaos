using System;
using System.Collections.Generic;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompeted;

    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;

    private int _waitingRecipeMax = 4;
    private int _successfulRecipesAmount;
    private float _spawnRecipeTimer = 4f;
    private float _spawnRecipeTimerMax = 4f;

    private bool _ingredientFound;
    private bool _plateContestMatchesRecipe;

    private void Awake()
    {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        _spawnRecipeTimer -= Time.deltaTime;
        if (_spawnRecipeTimer <= 0f)
        {
            _spawnRecipeTimer = _spawnRecipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < _waitingRecipeMax)
            {
                int waitingRecipeSO = UnityEngine.Random.Range(0, recipeListSO.recipeSoList.Count);

                SpawnNewWaitingRecipeClientRpc(waitingRecipeSO);
            }
        }
    }

    [ClientRpc] private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSo = recipeListSO.recipeSoList[waitingRecipeSOIndex];
        
        waitingRecipeSOList.Add(waitingRecipeSo);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                _plateContestMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    _ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            _ingredientFound = true;
                            break;
                        }
                    }
                    if (!_ingredientFound)
                    {
                        _plateContestMatchesRecipe = false;
                    }
                }
                if (_plateContestMatchesRecipe)
                {
                    DeliveryCorrectRecipeServerRpc(i);
                    
                    return;
                }
            }
        }

        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }
    
    [ClientRpc] private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    
    [ServerRpc(RequireOwnership = false)] private void DeliveryCorrectRecipeServerRpc(int waitingRecipeSPListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSPListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSPListIndex)
    {
        _successfulRecipesAmount++;
                    
        waitingRecipeSOList.RemoveAt(waitingRecipeSPListIndex);
                    
        OnRecipeCompeted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }
    
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return _successfulRecipesAmount;
    }
}