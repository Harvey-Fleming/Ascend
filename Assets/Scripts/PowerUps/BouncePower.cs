using System.Collections;
using UnityEngine;

public class BouncePower : MonoBehaviour, IPowerUp
{

    Rigidbody2D playerRB;

    private float duration = 1;
    private bool isActive;

    public float Duration { get => duration; }

    public bool IsActive { get => isActive; }

    private void Start()
    {
        playerRB = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
    }

    public void Activate()
    {
        isActive = true;
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
        playerRB.AddForce(Vector2.up * 15, ForceMode2D.Impulse);
        StartCoroutine(Deactivate());
    }

    public IEnumerator Deactivate()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(Duration);
        isActive = false;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && !IsActive)
        {
            Activate();
        }
    }
}
