using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class GamePlayingClockUI : MonoBehaviour
    {
        [SerializeField] private Image timerImage;

        private void Update() => 
            timerImage.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
