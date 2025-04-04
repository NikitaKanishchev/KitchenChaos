using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI recipesDeliveredText;
        [SerializeField] private Button playAgainButton;

        private void Awake()
        {
            playAgainButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.Shutdown();
                Loader.Loader.Load(Loader.Loader.Scene.MainMenuScene);
            });
        }

        private void Start()
        {
            GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

            Hide();
        }
        private void GameManager_OnStateChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsGameOver())
            {
                Show();
                recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
            }
            else
                Hide();
        }

        private void Show() =>
            gameObject.SetActive(true);

        private void Hide() =>
            gameObject.SetActive(false);
    }
}