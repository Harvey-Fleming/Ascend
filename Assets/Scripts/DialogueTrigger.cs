using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private bool hasRan;
    [SerializeField] private string nodeName;

    Yarn.Unity.DialogueRunner dialogueRunner;

    // Start is called before the first frame update
    void Start()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!hasRan && !FindObjectOfType<PlayerManager>().IsPaused && collision.gameObject.CompareTag("Player"))
        {
            hasRan = true;
            dialogueRunner.StartDialogue(nodeName);
        }
    }
}
