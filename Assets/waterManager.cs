using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterManager : MonoBehaviour
{
    public PlayerMovement playermovement;
    [SerializeField] private bool isInWater = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
           playermovement.isInWater = false; // The player is on the ground, not in water
            isInWater = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            playermovement.isInWater = true; // Player leaves water
            isInWater = true;
        }
    }

}
