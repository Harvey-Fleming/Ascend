using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer()
    {
        Debug.Log("Started Saving");
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new();

        formatter.Serialize(stream, data);

        stream.Close();
        Debug.Log("Finished Saving");
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.data";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogWarning("No save file found in " + path);
            return null;
        }
    }

    #region WebGL
    public static PlayerData LoadPlayerPref()
    {
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
        PlayerData pData = new();

        pData.lastCheckpointPos[0] = PlayerPrefs.GetFloat("checkpointX", playerHealth.LastCheckpoint.transform.position.x);
        pData.lastCheckpointPos[1] = PlayerPrefs.GetFloat("checkpointY", playerHealth.LastCheckpoint.transform.position.y);
        pData.lastCheckpointPos[2] = PlayerPrefs.GetFloat("checkpointZ", playerHealth.LastCheckpoint.transform.position.z);

        Debug.Log("Loaded Checkpoint pos is " + pData.lastCheckpointPos[0] + ", " + pData.lastCheckpointPos[1] + ", " + pData.lastCheckpointPos[2] + ", ");

        return pData;
    }

    public static void SavePlayerPref()
    {
        Debug.Log("Saving Player Data with WebGL method");
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();

        PlayerPrefs.SetFloat("checkpointX", playerHealth.LastCheckpoint.transform.position.x);
        PlayerPrefs.SetFloat("checkpointY", playerHealth.LastCheckpoint.transform.position.y);
        PlayerPrefs.SetFloat("checkpointZ", playerHealth.LastCheckpoint.transform.position.z);

        PlayerPrefs.Save();
    }
    #endregion


}
