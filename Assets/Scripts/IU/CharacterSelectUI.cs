using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button readyButton;

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.Shutdown();
                Loader.Loader.Load(Loader.Loader.Scene.MainMenuScene);
            });
            
            readyButton.onClick.AddListener(() =>
            {
                CharacterSelectReady.Instance.SetPlayerReady();
            });
        }
    }
}
