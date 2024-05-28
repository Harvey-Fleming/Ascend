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
        //if(Camera.main.transform.rotation.z == 0)
        //{
        //    Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, 180);
        //}
        //else
        //{
        //    Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, 0);
        //}
        
        playerMovement.isInverse = !playerMovement.isInverse;
        playerMovement.GravityScale *= -1;
        playerMovement.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerMovement.gameObject.GetComponent<Rigidbody2D>().velocity.x, 0);
        playerMovement.gameObject.transform.localScale = new Vector3(playerMovement.gameObject.transform.localScale.x , playerMovement.gameObject.transform.localScale.y * -1, playerMovement.gameObject.transform.localScale.z);
        StartCoroutine(Deactivate());
    }

    public static void StaticActivate()
    {
        PlayerMovement playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        playerMovement.isInverse = !playerMovement.isInverse;
        playerMovement.GravityScale *= -1;
        playerMovement.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerMovement.gameObject.GetComponent<Rigidbody2D>().velocity.x, 0);
        playerMovement.gameObject.transform.localScale = new Vector3(playerMovement.gameObject.transform.localScale.x, playerMovement.gameObject.transform.localScale.y * -1, playerMovement.gameObject.transform.localScale.z);
    }

    public IEnumerator Deactivate()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(Duration);
        //playerMovement.isInverse = false;
        //Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, 0);
        //playerMovement.GravityScale *= -1;
        //playerMovement.gameObject.transform.localScale = new Vector3(playerMovement.gameObject.transform.localScale.x, playerMovement.gameObject.transform.localScale.y * -1, playerMovement.gameObject.transform.localScale.z);

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
