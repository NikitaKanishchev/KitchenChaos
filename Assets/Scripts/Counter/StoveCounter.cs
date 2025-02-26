using System;
using ScriptableObjects;
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

        private State _state;

        private float _fryingTimer;
        private float _burningTimer;

        private FryingRecipeSO _fryingRecipeSo;
        private BurningRecipeSO _burningRecipeSo;


        private void Start() => 
            _state = State.Idle;

        private void Update()
        {
            if (HasKitchenObject())
            {
                switch (_state)
                {
                    case State.Idle:
                        break;
                    case State.Frying:
                        _fryingTimer += Time.deltaTime;

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                        {
                            progressNormilized = _fryingTimer / _fryingRecipeSo.fryingTimerMax
                        });

                        if (_fryingTimer > _fryingRecipeSo.fryingTimerMax)
                        {
                            GetKitchenObject().DestroySelf();

                            KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);

                            _state = State.Fried;
                            _burningTimer = 0f;

                            _burningRecipeSo = GetBurningRecipeSOWithInput(GetKitchenObject().GetKithcenObjectSO());

                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                            {
                                state = _state
                            });
                        }

                        break;
                    case State.Fried:
                        _burningTimer += Time.deltaTime;

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                        {
                            progressNormilized = _burningTimer / _burningRecipeSo.burningTimerMax
                        });

                        if (_burningTimer > _burningRecipeSo.burningTimerMax)
                        {
                            GetKitchenObject().DestroySelf();

                            KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);

                            _state = State.Burned;

                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                            {
                                state = _state
                            });

                            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                            {
                                progressNormilized = 0f
                            });
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
                        player.GetKitchenObject().SetKitchenObjectParent(this);

                        _fryingRecipeSo = GetFryingRecipeSOWithInput(GetKitchenObject().GetKithcenObjectSO());

                        _state = State.Frying;
                        _fryingTimer = 0f;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = _state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                        {
                            progressNormilized = _fryingTimer / _fryingRecipeSo.fryingTimerMax
                        });
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
                            GetKitchenObject().DestroySelf();

                            _state = State.Idle;

                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                            {
                                state = _state
                            });

                            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                            {
                                progressNormilized = 0f
                            });
                        }
                    }
                }
                else
                {
                    GetKitchenObject().SetKitchenObjectParent(player);

                    _state = State.Idle;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = _state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                    {
                        progressNormilized = 0f
                    });
                }
            }
        }

        public bool IsFried()
        {
            return _state == State.Fried;
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