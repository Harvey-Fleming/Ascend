using System;

public static class GlobalEvents
{
    public static event EventHandler<SaveEventArg> SaveData;

    public static void OnSaveData(PlayerData playerData)
    {
#if !UNITY_WEBGL
        SaveData?.Invoke(null, new SaveEventArg(playerData));
#else
        //SaveData?.Invoke(null, new SaveEventArg(playerData));

#endif
        
    }

    public static event EventHandler<SaveEventArg> LoadData;

    public static void OnLoadData()
    {
#if !UNITY_WEBGL
        PlayerData pData = SaveSystem.LoadPlayer();
        LoadData?.Invoke(null, new SaveEventArg(pData));
#else
        PlayerData pData = SaveSystem.LoadPlayerPref();
        LoadData?.Invoke(null, new SaveEventArg(pData));

#endif
    }

}

public class SaveEventArg
{
    public PlayerData playerData;

    public SaveEventArg(PlayerData playerData)
    {
        this.playerData = playerData;
    }

}


