using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField] Vector3 lowestPos;
    [SerializeField] Vector3 HighestPos;

    private void Update()
    {
        float xPos = Mathf.Clamp(target.transform.position.x, lowestPos.x, HighestPos.x);
        float yPos = Mathf.Clamp(target.transform.position.y, lowestPos.y, HighestPos.y);
        transform.position = new Vector3(xPos, yPos, transform.position.z);
    }

    private void FixedUpdate()
    {

        //Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0f, 0f));

        //Debug.DrawRay(ray.origin, ray.direction, Color.red, 1.0f);

        //if(!Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayerMask))
        //{
        //    Debug.Log("Should go up");
        //    transform.position += Vector3.up;
        //}
        //else if (Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayerMask))
        //{

        //}
    }
}
