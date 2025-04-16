using System;
using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class CharacterColorSelectSingleUI : MonoBehaviour
    {
        [SerializeField] private int colorId;
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectGameObject;


        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
            });
        }

        private void Start()
        {
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
            
            image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
            UpdateIsSelected();
        }

        private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
        {
            UpdateIsSelected();
        }

        private void UpdateIsSelected()
        {
            if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
            {
                selectGameObject.SetActive(true);
            }
            else
            {
                selectGameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        }
    }
}
