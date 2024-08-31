using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance { get; private set; }

    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallPanTime = 0.35f;
    public float fallSpeedYDampChangeThreshold;
    private float normalYPanAmount;

    public bool IsLerpingYDamping { get; private set;}

    public bool LerpedFromPlayerFalling { get; set; }

    GameObject player;

    [SerializeField] List<CinemachineVirtualCamera> allCameras;

    CinemachineVirtualCamera currentCamera;

    [SerializeField] CinemachineBrain cineBrain;
    CinemachineFramingTransposer framingTransposer;

    Coroutine panCameraRoutine;
    Coroutine lerpYCameraRoutine;

    void Start()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        player = GameObject.FindWithTag("Player");

        foreach(CinemachineVirtualCamera camera in allCameras)
        {
            if(camera.enabled)
            {
                currentCamera = camera;

                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                if(framingTransposer != null)
                {
                    normalYPanAmount = framingTransposer.m_YDamping;
                }
                
            }
        }
    }

    void Update()
    {
        if(framingTransposer != null)
        {
            framingTransposer.m_TrackedObjectOffset = new Vector3(Mathf.Sign(player.transform.localScale.x) > 0? 0.5f : -0.5f , framingTransposer.m_TrackedObjectOffset.y, framingTransposer.m_TrackedObjectOffset.z);
        }
    }

    #region - Y Damping
    public void LerpYDamping(bool isPlayerFalling)
    {
        if(framingTransposer != null)
            lerpYCameraRoutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount = 0f;
        

        if(isPlayerFalling)
        {
            endDampAmount = fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = normalYPanAmount;
        }

        float elapsedTime = 0f;
        while(elapsedTime < fallPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / fallPanTime);
            framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }
    #endregion

    #region - Swap Main Camera
    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if (currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true;

            cameraFromLeft.enabled = false;

            currentCamera = cameraFromRight;

            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        if (currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            cameraFromLeft.enabled = true;

            cameraFromRight.enabled = false;

            currentCamera = cameraFromLeft;

            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    public void ForceSwapCamera(CinemachineVirtualCamera newCamera)
    {
        if(newCamera != currentCamera)
        {
            newCamera.enabled = true;

            currentCamera.enabled = false;

            currentCamera = newCamera;

            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        
    }
    #endregion

    #region - Lookahead
    public void OnPanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        if(framingTransposer != null)
            panCameraRoutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if(!panToStartingPos)
        {
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
            }

            endPos *= panDistance;

            startingPos = new Vector2(0.5f, 0);

            endPos += startingPos;
        }
        else
        {
            startingPos = framingTransposer.m_TrackedObjectOffset;
            endPos = new Vector2(0.5f, 0);
        }

        float t = 0f;
        while(t < panTime)
        {
            t += Time.deltaTime;

            Vector3 lerpVal = Vector3.Lerp(startingPos, endPos, t/panTime);
            framingTransposer.m_TrackedObjectOffset = lerpVal;

            yield return null;
        }
    }
    #endregion
}
