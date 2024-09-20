using System.Collections;
using UnityEngine;

public class GravityPower : MonoBehaviour, IPowerUp
{

    PlayerMovement playerMovement;

    [SerializeField] private float duration = 1;
    private bool isActive;

    public float Duration { get => duration; }

    public bool IsActive { get => isActive; }

    private void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void Activate()
    {
        isActive = true;

        playerMovement.FlipGraivty();
        StartCoroutine(Deactivate());
    }

    public static void StaticActivate()
    {
        PlayerMovement playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        playerMovement.FlipGraivty();
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
