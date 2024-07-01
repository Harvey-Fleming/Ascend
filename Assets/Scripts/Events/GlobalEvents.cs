using System;

public static class GlobalEvents
{
    //public static event EventHandler<SaveEventArg> SaveData;

    public static void OnSaveData()
    {
#if !UNITY_WEBGL
        SaveSystem.SavePlayer();
        SaveSystem.SaveGameData();
#else
        SaveSystem.SavePlayerPref();
        SaveSystem.SaveGamePref();
#endif

    }

    public static event EventHandler<SaveEventArg> LoadData;

    public static void OnLoadData()
    {
#if !UNITY_WEBGL
        PlayerData pData = SaveSystem.LoadPlayer();
        GameData gData = SaveSystem.LoadGameData();
        LoadData?.Invoke(null, new SaveEventArg(pData, gData));
#else
        PlayerData pData = SaveSystem.LoadPlayerPref();
        GameData gData = SaveSystem.LoadGamePref();
        LoadData?.Invoke(null, new SaveEventArg(pData, gData));

#endif
    }

}

public class SaveEventArg
{
    public PlayerData playerData;
    public GameData gameData;
    public DialogueData dialogueData;

    public SaveEventArg(PlayerData playerData, GameData gameData)
    {
        this.playerData = playerData;
        this.gameData = gameData;
    }

}


