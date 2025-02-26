using System;
using Counter;
using UnityEngine;

namespace IU
{
    public class StoveBurningWarningUI : MonoBehaviour
    {
        [SerializeField] private StoveCounter _stoveCounter;

        private float burnShowProgressAmount = .5f;

        private void Start()
        {
            _stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
            
            Hide();
        }

        private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            bool show = _stoveCounter.IsFried() && e.progressNormilized >= burnShowProgressAmount;

            if (show)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
