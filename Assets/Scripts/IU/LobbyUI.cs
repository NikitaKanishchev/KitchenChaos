using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button quickJoinButton;
        [SerializeField] private Button joinCodeButton;
        [SerializeField] private TMP_InputField joinCodeInputField;
        [SerializeField] private TMP_InputField playerNameInputField;
        [SerializeField] private LobbyCreateUI _lobbyCreateUI;

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                KitchenGameLobby.Instance.LeaveLobby();
                Loader.Loader.Load(Loader.Loader.Scene.MainMenuScene);
            });
            
            createLobbyButton.onClick.AddListener(() =>
            {
                _lobbyCreateUI.Show();
            });
            
            quickJoinButton.onClick.AddListener(() =>
            {
                KitchenGameLobby.Instance.QuickJoin();
            });
            
            joinCodeButton.onClick.AddListener(() =>
            {
                KitchenGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
            });
        }

        private void Start()
        {
            playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();
            
            playerNameInputField.onValueChanged.AddListener((string newText) =>
            {
                KitchenGameMultiplayer.Instance.SetPlayerName(newText);
            });
        }
    }
}
