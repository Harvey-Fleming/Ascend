using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData
{
    public int lastLevel;

    public GameData()
    {
        lastLevel = SceneManager.GetActiveScene().buildIndex;
    }
}
