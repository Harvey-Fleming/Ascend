using UnityEngine;
using TMPro;

public class CollectCoin : MonoBehaviour
{
    [SerializeField] TMP_Text counterText;

    int coinsGathered = 0;

    public int CoinsGathered { get => coinsGathered; set => coinsGathered = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Coin")
        {
            coinsGathered++;
            Destroy(collision.gameObject);
            counterText.text = coinsGathered.ToString();
        }
    }


}
