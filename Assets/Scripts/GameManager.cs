using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;

    private Dictionary<ulong, bool> _playerReadyDictionary;
    
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.WaitingToStart);

    private NetworkVariable<float> _countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> _gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 100f;

    private bool _isGamePaused = false;
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
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        _state.OnValueChanged += State_OnValueChanged;
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
        _isGamePaused = !_isGamePaused;

        if (_isGamePaused)
        {
            Time.timeScale = 0f;
            
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            
            OnGameUnpaused?.Invoke(this,EventArgs.Empty);
        }
    }
}