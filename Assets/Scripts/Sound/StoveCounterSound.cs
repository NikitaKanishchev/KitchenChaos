using System;
using Counter;
using UnityEngine;

namespace Sound
{
    public class StoveCounterSound : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;
        
        private AudioSource _audioSource;

        private float _warningSoundTimer;
        private float _warningSoundTimerMax = .2f;

        private bool _playWarningSound;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
            stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        }

        private void Update()
        {
            if (_playWarningSound)
            {
                _warningSoundTimer -= Time.deltaTime;
                
                if (_warningSoundTimer <= 0f)
                {
                    _warningSoundTimer = _warningSoundTimerMax;
                    
                    SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
                }
            }
        }

        private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            float burnShowProgressAmount = .5f;
            _playWarningSound = stoveCounter.IsFried() && e.progressNormilized >= burnShowProgressAmount;
        }

        private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;

            if (playSound)
                _audioSource.Play();
            else
            {
                _audioSource.Pause();
            }
        }
    }
}
