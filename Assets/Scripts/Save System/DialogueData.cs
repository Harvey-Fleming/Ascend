using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public Dictionary<string, bool> hasDialogueRanDictionary;

    public DialogueData()
    {
        GameObject[] dialogueTriggers = GameObject.FindGameObjectsWithTag("Dialogue");

        foreach(GameObject dialogueTrigger in dialogueTriggers)
        {
            DialogueTrigger triggerScript = dialogueTrigger.GetComponent<DialogueTrigger>();

            hasDialogueRanDictionary.Add(triggerScript.NodeName, triggerScript.HasRan);
        }
    }
}
