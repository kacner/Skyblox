using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Hands : MonoBehaviour
{
    public SpriteRenderer RightHand;
    public SpriteRenderer LeftHand;
    public PlayerMovement playermovement;
    void Start()
    {
        playermovement = GetComponentInParent<PlayerMovement>();
    }
    void Update()
    {
        UpdateHands();
        UpdateHandPos();
    }

    private void UpdateHands()
    {
        if (playermovement.newLookDir == "UpRight")
        {
            LeftHand.enabled = true;
            RightHand.enabled = false;
            LeftHand.sortingOrder = 1;
        }
        else if (playermovement.newLookDir == "UpLeft")
        {
            RightHand.enabled = true;
            LeftHand.enabled = false;
            RightHand.sortingOrder = 1;
        }
        else if (playermovement.newLookDir == "Up")
        {

            RightHand.enabled = true;
            LeftHand.enabled = true;
            RightHand.sortingOrder = -1;
            LeftHand.sortingOrder = -1;
        }
        else if (playermovement.newLookDir == "Down")
        {

            RightHand.enabled = true;
            LeftHand.enabled = true;
            RightHand.sortingOrder = 1;
            LeftHand.sortingOrder = 1;
        }
        else
        {
            RightHand.enabled = true;
            LeftHand.enabled = true;
            RightHand.sortingOrder = 1;
            LeftHand.sortingOrder = 1;
        }
    }


    private void UpdateHandPos()
    {
        if (playermovement.newLookDir == "Left" || playermovement.newLookDir == "DownLeft")
        {
            RightHand.gameObject.transform.localPosition = new Vector3(-0.241f, -0.183f, 0); //defr
            LeftHand.gameObject.transform.localPosition = new Vector3(0.137f, -0.23f, 0);
        }
        else if (playermovement.newLookDir == "Right" || playermovement.newLookDir == "DownRight")
        {
            RightHand.gameObject.transform.localPosition = new Vector3(0.031f, -0.241f, 0);
            LeftHand.gameObject.transform.localPosition = new Vector3(0.366f, -0.218f, 0);
        }
        else if (playermovement.newLookDir == "Up")
        {
            RightHand.gameObject.transform.localPosition = new Vector3(-0.241f, -0.183f, 0); //def
            LeftHand.gameObject.transform.localPosition = new Vector3(0.33f, -0.194f, 0);
        }
        else
        {
            RightHand.gameObject.transform.localPosition = new Vector3(-0.241f, -0.183f, 0); //def
            LeftHand.gameObject.transform.localPosition = new Vector3(0.4f, -0.194f, 0); //def
        }
    }
}
