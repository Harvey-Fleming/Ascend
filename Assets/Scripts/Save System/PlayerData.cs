using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] lastCheckpointPos;

    public PlayerData()
    {
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();

        Debug.Log(playerHealth);

        lastCheckpointPos = new float[3];
        lastCheckpointPos[0] = playerHealth.LastCheckpoint.transform.position.x;
        lastCheckpointPos[1] = playerHealth.LastCheckpoint.transform.position.y;
        lastCheckpointPos[2] = playerHealth.LastCheckpoint.transform.position.z;
    }
}
