using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                KitchenGameLobby.Instance.LeaveLobby();
                NetworkManager.Singleton.Shutdown();
                Loader.Loader.Load(Loader.Loader.Scene.MainMenuScene);
            });
            
            readyButton.onClick.AddListener(() =>
            {
                CharacterSelectReady.Instance.SetPlayerReady();
            });
        }

        private void Start()
        {
            Lobby lobby = KitchenGameLobby.Instance.GetLobby();

            lobbyNameText.text = "Lobby Name: " + lobby.Name;
            lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
        }
    }
}
