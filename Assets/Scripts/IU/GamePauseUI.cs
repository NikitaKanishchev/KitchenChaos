using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class GamePauseUI : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button optionsButton;

        private void Awake()
        {
         resumeButton.onClick.AddListener(() =>
         {
             GameManager.Instance.TogglePauseGame();
         });   
         
         mainMenuButton.onClick.AddListener(() =>
         {
             NetworkManager.Singleton.Shutdown();
             Loader.Loader.Load(Loader.Loader.Scene.MainMenuScene);
         });   
         
         optionsButton.onClick.AddListener(() =>
         {
             Hide();
             OptionsUI.Instance.Show(Show);
         });   
        }
        private void Start()
        {
            GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
            GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;

            Hide();
        }

        private void GameManager_OnLocalGameUnpaused(object sender, EventArgs e)
        {
            Hide();
        }

        private void GameManager_OnLocalGamePaused(object sender, EventArgs e)
        {
            Show();
        }

        private void Show()
        {
            gameObject.SetActive(true);
            
            resumeButton.Select();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
