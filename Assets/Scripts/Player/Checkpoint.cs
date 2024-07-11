using System;
using UnityEngine;
using Cinemachine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera checkpointCamera;

    public CinemachineVirtualCamera CheckpointCamera { get => checkpointCamera;}

    public void OnPlayerDeath(object sender, PlayerDeathEventArgs args)
    {
        if (args.currentCheckpoint == this)
        {
            CameraManager.instance.ForceSwapCamera(checkpointCamera); 
        }
    }

    private void OnEnable()
    {
        PlayerEvents.PlayerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        PlayerEvents.PlayerDeath -= OnPlayerDeath;
    }
}
