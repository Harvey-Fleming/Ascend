using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    
    public static void ClearSaveData()
    {
        List<string> paths = new List<string> { Application.persistentDataPath + "/player.data", Application.persistentDataPath + "/Game.data", Application.persistentDataPath + "/Dialogue.data"};

        foreach(string path in paths)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        PlayerPrefs.DeleteKey("checkpointX");
        PlayerPrefs.DeleteKey("checkpointY");
        PlayerPrefs.DeleteKey("checkpointZ");

        PlayerPrefs.DeleteKey("highestLevel");
    }

    #region - Player
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
        //Debug.Log("Saving Player Data with WebGL method");
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();

        PlayerPrefs.SetFloat("checkpointX", playerHealth.LastCheckpoint.transform.position.x);
        PlayerPrefs.SetFloat("checkpointY", playerHealth.LastCheckpoint.transform.position.y);
        PlayerPrefs.SetFloat("checkpointZ", playerHealth.LastCheckpoint.transform.position.z);

        PlayerPrefs.Save();
    }
    #endregion
    #endregion

    #region - Game

    public static void SaveGameData()
    {
        Debug.Log("Started Saving");
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/Game.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new();

        formatter.Serialize(stream, data);

        stream.Close();
        Debug.Log("Finished Saving");
    }

    public static GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/Game.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            //Debug.Log("Scene is " + data.lastLevel);
            return data;
        }
        else
        {
            Debug.LogWarning("No save file found in " + path);
            return null;
        }
    }


    #region - WebGL
    public static GameData LoadGamePref()
    {
        GameData gData = new();

        //Pack int into gData
        int highestLevel =  PlayerPrefs.GetInt("highestLevel", new GameData().lastLevel);

        gData.lastLevel = highestLevel;
        Debug.Log("Scene is " + gData.lastLevel);
        return gData;
    }

    public static void SaveGamePref()
    {
        //Debug.Log("Saving Game Data with WebGL method");

        PlayerPrefs.SetInt("highestLevel", new GameData().lastLevel);


        PlayerPrefs.Save();
    }
    #endregion
    #endregion

    #region - Dialogue

    public static void SaveDialogueData()
    {
        Debug.Log("Started Saving");
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/Dialogue.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        DialogueData data = new();

        formatter.Serialize(stream, data);

        stream.Close();
        Debug.Log("Finished Saving");
    }

    public static DialogueData LoadDialogueData()
    {
        string path = Application.persistentDataPath + "/Dialogue.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            DialogueData data = formatter.Deserialize(stream) as DialogueData;
            stream.Close();
            //Debug.Log("Scene is " + data.lastLevel);
            return data;
        }
        else
        {
            Debug.LogWarning("No save file found in " + path);
            return null;
        }
    }


    #region - WebGL
    public static DialogueData LoadDialoguePref()
    {
        DialogueData dData = new();

        
        return dData;
    }

    public static void SaveDialoguePref()
    {
        //Debug.Log("Saving Game Data with WebGL method");


        PlayerPrefs.Save();
    }
    #endregion
    #endregion



}
