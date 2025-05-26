using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playMultiplayerButton;
        [SerializeField] private Button playSingleplayerButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            playMultiplayerButton.onClick.AddListener(() =>
            {
                KitchenGameMultiplayer.playMultiplayer = true;
                Loader.Loader.Load(Loader.Loader.Scene.LobbyScene);
            });
            
            playSingleplayerButton.onClick.AddListener(() =>
            {
                KitchenGameMultiplayer.playMultiplayer = false;
                Loader.Loader.Load(Loader.Loader.Scene.LobbyScene);
            });
            
            exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });

            Time.timeScale = 1f;
        }
    }
}
