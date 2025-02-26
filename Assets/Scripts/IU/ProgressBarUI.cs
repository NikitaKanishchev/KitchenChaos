using System;
using Counter;
using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private GameObject hasProgressGameObject;
        [SerializeField] private Image barImage;
        
        private IHasProgress _hasProgress;

        private void Start()
        {
            _hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();

            if (_hasProgress == null)
            {
                Debug.LogError("GameObject" + hasProgressGameObject + " does not have a component that implements IHasProgress!");
            }
                
            _hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
            
            barImage.fillAmount = 0f;
            
            Hide();
        }

        private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            barImage.fillAmount = e.progressNormilized;

            if (e.progressNormilized == 0f || e.progressNormilized == 1f)
                Hide();
            else
                Show();
        }

        private void Show() => 
            gameObject.SetActive(true);

        private void Hide() => 
            gameObject.SetActive(false);
    }
}
