using System;
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
            GameManager.Instance.OnGamePaused += KitchenGameManager_OnGamePaused;
            GameManager.Instance.OnGameUnpaused += KitchenGameManager_OnGameUnpaused;
            
            Hide();
        }

        private void KitchenGameManager_OnGamePaused(object sender, EventArgs e)
        {
            Show();
        }

        private void KitchenGameManager_OnGameUnpaused(object sender, EventArgs e)
        {
            Hide();
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
