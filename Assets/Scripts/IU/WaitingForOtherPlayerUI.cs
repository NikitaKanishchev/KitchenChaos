using System;
using UnityEngine;

namespace IU
{
    public class WaitingForOtherPlayerUI : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
            
            GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
            
            Hide();
        }

        private void GameManager_OnStateChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsCountdownToStartActive())
            {
                Hide();
                Debug.Log("Hide canvas");
            }
        }

        private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsLocalPlayerReady())
            {
                Show();
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
