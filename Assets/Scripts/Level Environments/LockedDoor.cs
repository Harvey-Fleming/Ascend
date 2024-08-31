using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private List<DoorKey> doorKeys;


    public void OnKeyCollected(object sender, KeyCollectEventArgs args)
    {
        if(args.parentDoor.name == this.gameObject.name)
        {
            DoorKey key = (DoorKey)sender;
            //Cross the key off the list
            if (doorKeys.Contains(key))
            {
                doorKeys.Remove(key);

                Destroy(key.gameObject);

                if (doorKeys.Count == 0)
                {
                    OpenDoor();
                }
            }
        }
    }

    private void OpenDoor()
    {
        Destroy(this.gameObject);
    }

    private void OnEnable()
    {
        LevelEvents.KeyCollected += OnKeyCollected;
    }

    private void OnDisable()
    {
        LevelEvents.KeyCollected -= OnKeyCollected;
    }
}
