using UnityEngine;
using Cinemachine;

public class ChaseCameraSwap : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera ChaseCamera;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CameraManager.instance.ForceSwapCamera(ChaseCamera);
    }
}
