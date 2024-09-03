using UnityEngine;
using TMPro;

public class CollectCoin : MonoBehaviour
{
    [SerializeField] TMP_Text counterText;

    int coinsGathered = 0;

    bool isFirstCollect = true;

    public int CoinsGathered { get => coinsGathered; set => coinsGathered = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Coin")
        {
            if(isFirstCollect)
            {
                isFirstCollect = false;
                GameObject.FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue("firstcoin");            
            }
            coinsGathered++;
            AudioManager.instance.Play("CollectCoin");
            Destroy(collision.gameObject);
            if (CoinsGathered == 3)
            {
                coinsGathered = 0;
                GetComponent<PlayerHealth>().AddLives(1);
            }

            counterText.text = coinsGathered.ToString() + "/3";
        }
    }


}
