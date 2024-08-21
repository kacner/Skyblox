using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;

public class BowFire : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject arrow;
    public Transform bulletTransform;
    float holdtimer;
    bool staneone = false;
    public float bowHoldTime = 1f;
    public Transform Hand;
    public Transform HoldingHand;
    public Transform ArrowSpriteTransform;
    public GameObject ArrowSprite;
    private Animator animator;

    private PlayerMovement playermovement;

    private float previousRotz;
    public float rotationThreshold;


    void Start()
    {
        playermovement = GetComponentInParent<PlayerMovement>();

        ArrowSprite.SetActive(false);

        animator = GetComponentInChildren<Animator>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //setts the right rotation of the bow when instantiated
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotz + 90);

        UpdateSortingLayers();
    }

    void Update()
    {
        UpdateSortingLayers();

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(rotz - previousRotz) > rotationThreshold)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotz + 90);
            previousRotz = rotz; 
        }
        
        //handels the shooting of the bow 
        if (playermovement.arrowcount >= 1)
        {
            if (Input.GetKey(KeyCode.Mouse1) || staneone)
            {
                holdtimer = Mathf.Clamp(holdtimer, 0, bowHoldTime);
                holdtimer += Time.deltaTime;

                animator.SetBool("DrawingBow", true);
                ArrowSprite.SetActive(true);

                if (holdtimer > bowHoldTime)
                    staneone = true;

                if (!Input.GetKey(KeyCode.Mouse1))
                {
                    holdtimer = 0;
                    Instantiate(arrow, bulletTransform.position, quaternion.identity);
                    playermovement.arrowcount--;
                    staneone = false;
                }
                
            }
            else
            {
                holdtimer = 0;
                animator.SetBool("DrawingBow", false);
                ArrowSprite.SetActive(false);
            }

        }
        else
        {
            animator.SetBool("DrawingBow", false);
            ArrowSprite.SetActive(false);
        }
    }

    private void UpdateSortingLayers()
    {
        //Bow Sorting Layers
        float zRotation = transform.eulerAngles.z;
        if (zRotation > 180f)
        {
            zRotation -= 360f;
        }
        // For the right side (positive angles 0 to 180 degrees)
        if (zRotation > 70f && zRotation < 180f)
        {
            bulletTransform.GetComponent<SpriteRenderer>().sortingOrder = -2;
        }
        // For the left side (negative angles -75 to -180 degrees)
        else if (zRotation < -75f && zRotation > -180f)
        {
            bulletTransform.GetComponent<SpriteRenderer>().sortingOrder = -2;
        }
        else
        {
            bulletTransform.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        //Drawing Hand sorting layer & Arrow Sorting Layers
        float zRotation2 = transform.eulerAngles.z;
        if (zRotation2 > 180f)
        {
            zRotation2 -= 360f;
        }

        // For the right side (positive angles 0 to 180 degrees)
        if (zRotation2 > 112.52f && zRotation2 < 180f)
        {
            ArrowSpriteTransform.GetComponent<SpriteRenderer>().sortingOrder = -2;
            Hand.GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
        // For the left side (negative angles -75 to -180 degrees)
        else if (zRotation2 < -105.1f && zRotation2 > -180f)
        {
            ArrowSpriteTransform.GetComponent<SpriteRenderer>().sortingOrder = -2;
            Hand.GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
        else
        {
            Hand.GetComponent<SpriteRenderer>().sortingOrder = 2;
            ArrowSpriteTransform.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }

        //Holding Hand sorting layer
        float zRotation3 = transform.eulerAngles.z;
        if (zRotation3 > 180f)
        {
            zRotation3 -= 360f;
        }

        // For the right side (positive angles 0 to 180 degrees)
        if (zRotation3 > 120f && zRotation3 < 180f)
        {
            HoldingHand.GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
        // For the left side (negative angles -75 to -180 degrees)
        else if (zRotation3 < -121.1f && zRotation3 > -180f)
        {
            HoldingHand.GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
        else
        {
            HoldingHand.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
    }
}