using System;
using UnityEngine;

namespace IU
{
    public class PauseMultiplayerUI : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.OnMultiplayerGamePaused += GameManager_OnMultiplayerGamePaused; 
            GameManager.Instance.OnMultiplayerGameUnPaused += GameManager_OnMultiplayerGameUnPaused;  
            
            Hide();
        }

        private void GameManager_OnMultiplayerGamePaused(object sender, EventArgs e)
        {
            Show();
        }

        private void GameManager_OnMultiplayerGameUnPaused(object sender, EventArgs e)
        {
            Hide();
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
