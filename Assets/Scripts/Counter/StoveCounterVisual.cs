using UnityEngine;

namespace Counter
{
    public class StoveCounterVisual : MonoBehaviour
    {
        [SerializeField] private StoveCounter _stoveCounter;
        
        [SerializeField] private GameObject stoveOnGameObject;
        [SerializeField] private GameObject particlesGameObject;

        private void Start() => 
            _stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;

        private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            bool showVisual = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
            stoveOnGameObject.SetActive(showVisual);
            particlesGameObject.SetActive(showVisual);
        }
    }
}
