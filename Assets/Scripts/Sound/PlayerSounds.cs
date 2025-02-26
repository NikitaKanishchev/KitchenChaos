using System;
using UnityEngine;

namespace Sound
{
    public class PlayerSounds : MonoBehaviour
    {
        private Player _player;

        private float _footstepTimer;
        private float _footstepTimerMax = 0.1f;
        private float _volume = 1f;

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        private void Update()
        {
            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer < 0f)
            {
                _footstepTimer = _footstepTimerMax;

                if (_player.IsWalking()) 
                    SoundManager.Instance.PlayFootstepsSound(_player.transform.position, _volume);
            }
        }
    }
}
