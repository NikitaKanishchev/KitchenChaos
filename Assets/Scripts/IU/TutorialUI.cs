using System;
using TMPro;
using UnityEngine;

namespace IU
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI keyMoveUpText;
        [SerializeField] private TextMeshProUGUI keyMoveDownText;
        [SerializeField] private TextMeshProUGUI keyMoveLeftText;
        [SerializeField] private TextMeshProUGUI keyMoveRightText;
        [SerializeField] private TextMeshProUGUI keyMoveInteractText;
        [SerializeField] private TextMeshProUGUI keyMoveInteractAlternateText;
        [SerializeField] private TextMeshProUGUI keyMovePauseText;
        
        [SerializeField] private TextMeshProUGUI keyMoveGamepadInteractText;
        [SerializeField] private TextMeshProUGUI keyMoveGamepadInteractAltText;
        [SerializeField] private TextMeshProUGUI keyMoveGamepadPauseText;

        private void Start()
        {
            GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
            GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
            
            UpdateVisual();
            
            Show();
        }

        private void GameManager_OnStateChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsCountdownToStartActive())
            {
                Hide();
            }
        }

        private void GameInput_OnBindingRebind(object sender, EventArgs e)
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
            keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
            keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
            keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
            keyMoveInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
            keyMoveInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
            keyMovePauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
            
            keyMoveGamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteract);
            keyMoveGamepadInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteractAlternate);
            keyMoveGamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
