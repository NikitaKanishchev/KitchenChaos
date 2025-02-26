using System;
using Sound;
using TMPro;
using UnityEngine;

namespace IU
{
    public class GameStartCountdownUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _countdownText;

        private const string NUMBER_POPUP = "NumberPopUp";

        private Animator _animator;

        private int _previousCountdownNumber;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
            
            Hide();
        }

        private void Update()
        {
            int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
            _countdownText.text = countdownNumber.ToString();

            if (_previousCountdownNumber != countdownNumber)
            {
                _previousCountdownNumber = countdownNumber;
                _animator.SetTrigger(NUMBER_POPUP);
                
                SoundManager.Instance.PlayCountdownSound();
            }
        }

        private void GameManager_OnStateChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsCountdownToStartActive())
                Show();
            else
                Hide();
        }

        private void Show() => 
            gameObject.SetActive(true);

        private void Hide() => 
            gameObject.SetActive(false);
    }
}
