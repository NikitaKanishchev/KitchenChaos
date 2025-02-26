using System;
using Counter;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClipRefsSO audioClipRefsSO;
        public static SoundManager Instance { get; private set; }

        private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

        private float _volume = 1f;

        private void Awake()
        {
            Instance = this;

            _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
        }

        private void Start()
        {
            DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
            CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
            Player.Instance.OnPickedSomething += Player_OnPickedSomething;
            BaseCounter.OnAnyObjectPlaceHere += BaseCounter_OnAnyObjectPlaceHere;
            TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
        }

        public void PlayFootstepsSound(Vector3 position, float volume) =>
            PlaySound(audioClipRefsSO.footstep, position, volume);
        
        public void PlayCountdownSound() =>
            PlaySound(audioClipRefsSO.warning, Vector3.zero);
        
        public void PlayWarningSound(Vector3 position) =>
            PlaySound(audioClipRefsSO.warning, position);

        public void ChangeVolume()
        {
            _volume += .1f;

            if (_volume > 1f)
            {
                _volume = 0f;
            }
            
            PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, _volume);
            PlayerPrefs.Save();
        }

        public float GetVolume()
        {
            return _volume;
        }
        private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplier = 1f) =>
            PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumeMultiplier);

        private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) =>
            AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * _volume);

        private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
        {
            DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
            PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
        }

        private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
        {
            DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
            PlaySound(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
        }

        private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
        {
            CuttingCounter cuttingCounter = sender as CuttingCounter;
            PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
        }

        private void Player_OnPickedSomething(object sender, EventArgs e) =>
            PlaySound(audioClipRefsSO.objectPickUp, Player.Instance.transform.position);

        private void BaseCounter_OnAnyObjectPlaceHere(object sender, EventArgs e)
        {
            BaseCounter baseCounter = sender as BaseCounter;
            PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
        }

        private void TrashCounter_OnAnyObjectTrashed(object sender, EventArgs e)
        {
            TrashCounter trashCounter = sender as TrashCounter;
            PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
        }
    }
}