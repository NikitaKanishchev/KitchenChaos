using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            playButton.onClick.AddListener(() =>
            {
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
