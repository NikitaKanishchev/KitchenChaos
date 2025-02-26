using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Mode _mode;
    
    private enum Mode
    {
        LookAt,
        LookAtInvert,
        CameraForward,
        CameraForwardInverted,
    }
    
    private void LateUpdate()
    {
        switch (_mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            
            case Mode.LookAtInvert:
                Vector3 dirFromCamera = transform.position = Camera.main.transform.position;
                    transform.LookAt(transform.position + dirFromCamera);
                break;
            
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}
