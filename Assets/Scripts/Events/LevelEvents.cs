using System;
using UnityEngine;

public class LevelEvents : MonoBehaviour
{
    public static event EventHandler<KeyCollectEventArgs> KeyCollected;
    
    public static void OnKeyCollected(object sender, KeyCollectEventArgs args)
    {
        KeyCollected?.Invoke(sender, args);
    }
}

public class KeyCollectEventArgs
{
    public GameObject parentDoor;

    public KeyCollectEventArgs(GameObject parentDoor)
    {
        this.parentDoor = parentDoor;
    }
}

