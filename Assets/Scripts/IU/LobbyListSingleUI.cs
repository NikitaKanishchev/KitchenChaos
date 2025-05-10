using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class LobbyListSingleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        
        private Lobby _lobby;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                KitchenGameLobby.Instance.JoinWithId(_lobby.Id);
            });
        }

        public void SetLobby(Lobby lobby)
        {
            _lobby = lobby;
            lobbyNameText.text = lobby.Name;
        }
    }
}
