using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnMultiplayerGamePaused; 
    public event EventHandler OnMultiplayerGameUnPaused; 

    private Dictionary<ulong, bool> _playerReadyDictionary;
    private Dictionary<ulong, bool> _playerPauseDictionary;
    
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.WaitingToStart);

    private NetworkVariable<float> _countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> _gamePlayingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<bool> _isGamePaused = new NetworkVariable<bool>(false);
    
    private float gamePlayingTimerMax = 100f;

    private bool _isLocalGamePaused = false;
    private bool _isLocalPlayerReady;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private void Awake()
    {
        Instance = this;

        _playerReadyDictionary = new Dictionary<ulong, bool>();
        _playerPauseDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        _state.OnValueChanged += State_OnValueChanged;

        _isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;
    }

    private void IsGamePaused_OnValueChanged(bool previousvalue, bool newvalue)
    {
        if (_isGamePaused.Value)
        {
            Time.timeScale = 0f;
            
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            
            OnMultiplayerGameUnPaused?.Invoke(this,EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (_state.Value == State.WaitingToStart)
        {
            _isLocalPlayerReady = true;
            
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool _allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
            {
                //This player is not ready
                _allClientsReady = false;
                break;
            }
        }

        if (_allClientsReady)
        {
            _state.Value = State.CountdownToStart;
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        switch (_state.Value)
        {
            case State.WaitingToStart:
                break;

            case State.CountdownToStart:
                _countdownToStartTimer.Value -= Time.deltaTime;
                if (_countdownToStartTimer.Value < 0f)
                {
                    _state.Value = State.GamePlaying;
                    _gamePlayingTimer.Value = gamePlayingTimerMax;
                    
                }
                break;

            case State.GamePlaying:
                _gamePlayingTimer.Value -= Time.deltaTime;
                if (_gamePlayingTimer.Value < 0f)
                {
                    _state.Value = State.GameOver;
                }
                break;

            case State.GameOver:
                break;
        }

        Debug.Log(_state);
    }

    public bool IsGamePlaying() => 
        _state.Value == State.GamePlaying;

    public bool IsCountdownToStartActive() => 
        _state.Value == State.CountdownToStart;

    public bool IsGameOver() => 
        _state.Value == State.GameOver;

    public bool IsLocalPlayerReady() => 
        _isLocalPlayerReady;

    public float GetCountdownToStartTimer()
    {
        return _countdownToStartTimer.Value;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (_gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    public void TogglePauseGame()
    {
        _isLocalGamePaused = !_isLocalGamePaused;

        if (_isLocalGamePaused)
        {
            PauseGameServerRpc();
            
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnPauseGameServerRpc();
            
            OnLocalGameUnpaused?.Invoke(this,EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
        
        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        foreach (ulong clientsId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (_playerPauseDictionary.ContainsKey(clientsId) && _playerPauseDictionary[clientsId])
            {
                _isGamePaused.Value = true;
                return;
            }
        }

        //All players are unpaused
        _isGamePaused.Value = false;
    }
}