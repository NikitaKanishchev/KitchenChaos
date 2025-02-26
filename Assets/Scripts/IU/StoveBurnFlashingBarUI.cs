using System;
using Counter;
using UnityEngine;

namespace IU
{
    public class StoveBurnFlashingBarUI : MonoBehaviour
    {
        private const string IS_FLASHING = "IsFlashing";
        
        [SerializeField] private StoveCounter _stoveCounter;

        private Animator _animator;
        
        private float burnShowProgressAmount = .5f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            
            _animator.SetBool(IS_FLASHING, false);
        }

        private void Start()
        {
            _stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        }

        private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            bool show = _stoveCounter.IsFried() && e.progressNormilized >= burnShowProgressAmount;

            _animator.SetBool(IS_FLASHING, show);
        }
    }
}
