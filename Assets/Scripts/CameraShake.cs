using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Yarn.Unity;

public class CameraShake : MonoBehaviour
{
    CinemachineVirtualCamera cinemachineCamera;
    CinemachineBasicMultiChannelPerlin perlinCamShake;
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeTime = 0.2f;

    private float timer;
    
    [YarnCommand("ShakeScreen")]
    [ContextMenu("Start Camera Shake")]
    public void StartShake()
    {
        cinemachineCamera = GetComponent<CameraManager>().CurrentCamera;
        perlinCamShake = cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlinCamShake.m_AmplitudeGain = shakeIntensity;

        timer = shakeTime;
    }

    public void StopShake()
    {
        cinemachineCamera = GetComponent<CameraManager>().CurrentCamera;
        perlinCamShake = cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlinCamShake.m_AmplitudeGain = 0f;
        timer = 0f;
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                StopShake();
            }
        }
    }
}
