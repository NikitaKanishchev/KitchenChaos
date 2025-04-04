using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    public static GameInput Instance { get; private set; }
    
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;
    
    public enum Binding
    {
        Move_Up,        
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
        
        GamepadInteract,
        GamepadInteractAlternate,
        Gamepad_Pause,
    }

    private InputAction _inputAction;
    private int _bindingIndex;
    
    private PlayerInputActions _playerInputActions;
    private void Awake()
    {
        Instance = this;

        _playerInputActions = new PlayerInputActions();
        
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        _playerInputActions.Player.Enable();
        
        _playerInputActions.Player.Interact.performed += Interact_performed;
        _playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        _playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Interact.performed -= Interact_performed;
        _playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        _playerInputActions.Player.Pause.performed -= Pause_performed;
        
        _playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this,EventArgs.Empty);
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                return _playerInputActions.Player.Move.bindings[1].ToDisplayString();    
            
            case Binding.Move_Down:
                return _playerInputActions.Player.Move.bindings[2].ToDisplayString();    
            
            case Binding.Move_Left:
                return _playerInputActions.Player.Move.bindings[3].ToDisplayString();    
            
            case Binding.Move_Right:
                return _playerInputActions.Player.Move.bindings[4].ToDisplayString();    
            
            case Binding.Interact:
                return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            
            case Binding.InteractAlternate:
                return _playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            
            case Binding.Pause:
                return _playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            
            case Binding.GamepadInteract:
                return _playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            
            case Binding.GamepadInteractAlternate:
                return _playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            
            case Binding.Gamepad_Pause:
                return _playerInputActions.Player.Pause.bindings[1].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        _playerInputActions.Player.Disable();

        switch (binding)
        {
            default:
            case Binding.Move_Up:
                _inputAction = _playerInputActions.Player.Move;
                _bindingIndex = 1;
                break;
            case Binding.Move_Down:
                _inputAction = _playerInputActions.Player.Move;
                _bindingIndex = 2;
                break;
            case Binding.Move_Left:
                _inputAction = _playerInputActions.Player.Move;
                _bindingIndex = 3;
                break;
            case Binding.Move_Right:
                _inputAction = _playerInputActions.Player.Move;
                _bindingIndex = 4;
                break;
            
            case Binding.Interact:
                _inputAction = _playerInputActions.Player.Interact;
                _bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                _inputAction = _playerInputActions.Player.InteractAlternate;
                _bindingIndex = 0;
                break;
            case Binding.Pause:
                _inputAction = _playerInputActions.Player.Pause;
                _bindingIndex = 0;
                break;
            
            case Binding.GamepadInteract:
                _inputAction = _playerInputActions.Player.Interact;
                _bindingIndex = 1;
                break;
            case Binding.GamepadInteractAlternate:
                _inputAction = _playerInputActions.Player.InteractAlternate;
                _bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                _inputAction = _playerInputActions.Player.Pause;
                _bindingIndex = 1;
                break;
        }
        
        _inputAction.PerformInteractiveRebinding(_bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                _playerInputActions.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, _playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
                
                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }
}
