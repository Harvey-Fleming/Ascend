using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance { get; private set; }

    GameObject player;

    [SerializeField] List<CinemachineVirtualCamera> _AllCameras;
    [SerializeField] CinemachineBrain cineBrain;
    [SerializeField] CinemachineFramingTransposer framingTransposer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        foreach(CinemachineVirtualCamera camera in _AllCameras)
        {
            if(camera.gameObject == cineBrain.ActiveVirtualCamera.VirtualCameraGameObject)
            {
                framingTransposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
                framingTransposer.m_TrackedObjectOffset = new Vector3(Mathf.Sign(player.transform.localScale.x) > 0? 0.5f : -0.5f , framingTransposer.m_TrackedObjectOffset.y, framingTransposer.m_TrackedObjectOffset.z);
            }
        }
    }
}
