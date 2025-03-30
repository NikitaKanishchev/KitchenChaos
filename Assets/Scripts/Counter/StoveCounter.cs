using System;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counter
{
    public class StoveCounter : BaseCounter, IHasProgress
    {
        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

        public class OnStateChangedEventArgs : EventArgs
        {
            public State state;
        }

        public enum State
        {
            Idle,
            Frying,
            Fried,
            Burned,
        }

        [SerializeField] private FryingRecipeSO[] fryingRecipeSoArray;
        [SerializeField] private BurningRecipeSO[] burningRecipeSoArray;

        private NetworkVariable<State>  _state = new NetworkVariable<State>(State.Idle);

        private NetworkVariable<float> _fryingTimer = new NetworkVariable<float>(0f);
        private NetworkVariable<float>  _burningTimer = new NetworkVariable<float>(0f);

        private FryingRecipeSO _fryingRecipeSo;
        private BurningRecipeSO _burningRecipeSo;
        
        public override void OnNetworkSpawn()
        {
            _fryingTimer.OnValueChanged += FryingTimer_OnValueChange;
            _burningTimer.OnValueChanged += BurningTimer_OnValueChange;
            _state.OnValueChanged += State_OnValueChange;
        }

        private void FryingTimer_OnValueChange(float previousValue, float newValue)
        {
            float fryingTimerMax = _fryingRecipeSo != null ? _fryingRecipeSo.fryingTimerMax : 1f;
            
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
            {
                progressNormilized = _fryingTimer.Value / fryingTimerMax
            });
        }
        
        private void BurningTimer_OnValueChange(float previousValue, float newValue)
        {
            float burningTimerMax = _burningRecipeSo != null ? _burningRecipeSo.burningTimerMax : 1f;
            
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
            {
                progressNormilized = _burningTimer.Value / burningTimerMax
            });
        }

        private void State_OnValueChange(State previousState, State newState)
        {
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
            {
                state = _state.Value
            });

            if (_state.Value == State.Burned || _state.Value == State.Idle)
            {
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                {
                    progressNormilized = 0f
                });
            }
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }
            
            if (HasKitchenObject())
            {
                switch (_state.Value)
                {
                    case State.Idle:
                        break;
                    case State.Frying:
                        _fryingTimer.Value += Time.deltaTime;
                        
                        if (_fryingTimer.Value > _fryingRecipeSo.fryingTimerMax)
                        {
                            KitchenObject.DestroyKitchenObject(GetKitchenObject());
                            
                            KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);

                            _state.Value = State.Fried;
                            _burningTimer.Value = 0f;
                            
                            SetBurningRecipeSOClientRpc(
                                KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKithcenObjectSO())
                                );
                        }

                        break;
                    case State.Fried:
                        _burningTimer.Value += Time.deltaTime;
                        
                        if (_burningTimer.Value > _burningRecipeSo.burningTimerMax)
                        {
                            KitchenObject.DestroyKitchenObject(GetKitchenObject());

                            KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);

                            _state.Value = State.Burned;
                        }

                        break;
                    case State.Burned:
                        break;
                }
            }
        }

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

                        InteractLogicPlaceObjectOnCounterServerRpc(
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKithcenObjectSO())
                            );
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
                        {
                            KitchenObject.DestroyKitchenObject(GetKitchenObject());

                            SetStateIdleServerRpc();
                        }
                    }
                }
                else
                {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    
                    SetStateIdleServerRpc();
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetStateIdleServerRpc()
        {
            _state.Value = State.Idle;
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
        {
            _fryingTimer.Value = 0f;
            _state.Value = State.Frying;

            SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
        }
        
        [ClientRpc]
        private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
        {
            KitchenObjectSO kitchenObjectSO =
                KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
            
            _fryingRecipeSo = GetFryingRecipeSOWithInput(kitchenObjectSO);
        }
        
        [ClientRpc]
        private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
        {
            KitchenObjectSO kitchenObjectSO =
                KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
            
            _burningRecipeSo = GetBurningRecipeSOWithInput(kitchenObjectSO);
        }

        public bool IsFried()
        {
            return _state.Value == State.Fried;
        }

        private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSo);
            return fryingRecipeSO != null;
        }

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
        {
            FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSo);
            if (fryingRecipeSO != null)
            {
                return fryingRecipeSO.output;
            }
            else
            {
                return null;
            }
        }

        private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSoArray)
            {
                if (fryingRecipeSO.inpit == inputKitchenObjectSo)
                    return fryingRecipeSO;
            }

            return null;
        }

        private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            foreach (BurningRecipeSO burningRecipeSO in burningRecipeSoArray)
            {
                if (burningRecipeSO.inpit == inputKitchenObjectSo)
                    return burningRecipeSO;
            }

            return null;
        }
    }
}