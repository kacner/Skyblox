using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickupTest : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().maxSpeed = 10f;
            Destroy(gameObject);
        }
    }

}
