using UnityEngine;
using UnityEngine.UI;

namespace IU
{
    public class PlateIconsSingleUI : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSo)
        {
            _image.sprite = kitchenObjectSo.sprite;
        }
    }
}
