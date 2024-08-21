using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SwordBase : MonoBehaviour
{
    //private Camera mainCam;
    //private Vector3 mousePos;
    public Transform Sword;

    //private float previousRotz;
    public float rotationThreshold;
    private PlayerMovement playermovement;
    public Transform Sword__Weapond;
    private Animator animator;

    private bool sworddrawn;
    public bool facingToTheSide = false;
    public bool facingToTheSideAndUp = true;

    public SpriteRenderer SwordSpriteRenderer;
    public SpriteRenderer LHandSpriteRenderer;
    public SpriteRenderer RHandSpriteRenderer;
    public SpriteRenderer HolsterSpriteRenderer;
    public SpriteRenderer HolsterFromSideSpriteRenderer;

    //public bool shouldRotate = false;
    void Start()
    {
        animator = GetComponent<Animator>();

        playermovement = GetComponentInParent<PlayerMovement>();

        updateAnimations();

        /* if(shouldRotate)
         {
             mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

             //setts the right rotation of the bow when instantiated
             mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

             Vector3 rotation = mousePos - transform.position;

             float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

             transform.rotation = Quaternion.Euler(0, 0, rotz + 90);
         }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (playermovement.lookDir == "Left" || playermovement.lookDir == "UpLeft" || playermovement.lookDir == "Up")
        {
           Sword__Weapond.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (playermovement.lookDir == "Right" || playermovement.lookDir == "UpRight" || playermovement.lookDir == "Down")
        {
            Sword__Weapond.transform.localScale = new Vector3(1, 1, 1);
           //transform.position = new Vector2(0.077f, 0);
        }


        updateAnimations();


        animator.SetBool("FacingToTheSide", facingToTheSide);
        animator.SetBool("SwordDrawn", sworddrawn);

        if (Input.GetKeyDown(KeyCode.Mouse0) && sworddrawn == false)
        {
            sworddrawn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && sworddrawn == true )
        {
            sworddrawn = false;
        }


        /* if (shouldRotate)
         {
             mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

             Vector3 rotation = mousePos - transform.position;

             float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

             if (Mathf.Abs(rotz - previousRotz) > rotationThreshold)
             {
                 transform.rotation = Quaternion.Euler(0, 0, rotz + 90);
                 previousRotz = rotz;
             }
         }*/

    }

    private void updateAnimations()
    {

        if (playermovement.lookDir == "Up")
        {
            facingToTheSide = false;
            HolsterSpriteRenderer.enabled = true;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 4;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterFromSideSpriteRenderer.enabled = false;
        }
        else if (playermovement.lookDir == "Down")
        {
            facingToTheSide = false;
            HolsterSpriteRenderer.enabled = true;
            SwordSpriteRenderer.sortingOrder = 2;
            HolsterSpriteRenderer.sortingOrder = -1;
            RHandSpriteRenderer.sortingOrder = 3;
            LHandSpriteRenderer.sortingOrder = 1;
            HolsterFromSideSpriteRenderer.enabled = false;
        }
        else if (playermovement.lookDir == "UpRight")
        {
            facingToTheSide = false;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 4;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = false;
        }
        else if (playermovement.lookDir == "Left")
        {
            facingToTheSide = true;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 2;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = true;
        }
        else if (playermovement.lookDir == "Right")
        {
            facingToTheSide = true;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 2;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = true;
        }
        else if (playermovement.lookDir == "UpLeft")
        {
            facingToTheSide = false;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 2;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = false;
        }
        else if (playermovement.lookDir == "UpRight")
        {
            facingToTheSide = false;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 2;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = false;
        }
    }
}