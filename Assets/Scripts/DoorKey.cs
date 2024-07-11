using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : MonoBehaviour
{
    [SerializeField] private GameObject parentDoor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            LevelEvents.OnKeyCollected(this, new KeyCollectEventArgs(parentDoor));
        }
    }
}
