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

    public SpriteRenderer SwordSpriteRenderer;
    public SpriteRenderer LHandSpriteRenderer;
    public SpriteRenderer RHandSpriteRenderer;
    public SpriteRenderer HolsterSpriteRenderer;
    public SpriteRenderer HolsterFromSideSpriteRenderer;
    public SpriteRenderer HolsterFromBackSpriteRenderer;
    public SpriteRenderer HolsterUpRightSpriteRenderer;
    public SpriteRenderer HolsterUpLeftSpriteRenderer;

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
        if (playermovement.lookDir == "Left"|| playermovement.lookDir == "UpLeft"|| playermovement.lookDir == "Up"|| playermovement.lookDir == "DownLeft")
        {
            Sword__Weapond.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (playermovement.lookDir == "Right"|| playermovement.lookDir == "UpRight"|| playermovement.lookDir == "Down"|| playermovement.lookDir == "UpLeft")
        {
            Sword__Weapond.transform.localScale = new Vector3(1, 1, 1);
            
        }

        if(playermovement.lookDir == "Up")
        {
            transform.localPosition = new Vector2(0.015f, transform.localPosition.y);
        }
        else if (playermovement.lookDir == "Down")
        {
            transform.localPosition = new Vector2(0.0666f, transform.localPosition.y);
        }

        updateAnimations();


        animator.SetBool("FacingToTheSide", facingToTheSide);
        animator.SetBool("SwordDrawn", sworddrawn);

        if (Input.GetKeyDown(KeyCode.Mouse0) && sworddrawn == false)
        {
            DrawSword();
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

    private void DrawSword()
    {
        sworddrawn = true;
    }

    private void updateAnimations()
    {
        if (playermovement.lookDir == "Up")
        {
            facingToTheSide = false;
            HolsterSpriteRenderer.enabled = true;
            SwordSpriteRenderer.sortingOrder = 2 - 6;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 4;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterFromSideSpriteRenderer.enabled = false;
            HolsterFromBackSpriteRenderer.enabled = true;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = false;
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
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = false;
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
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = true;
            HolsterUpLeftSpriteRenderer.enabled = false;
        }
        else if (playermovement.lookDir == "Left" || playermovement.lookDir == "DownLeft")
        {
            facingToTheSide = true;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 0;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = true;
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.sortingOrder = -1 + 2;
        }
        else if (playermovement.lookDir == "Right" || playermovement.lookDir == "DownRight")
        {
            facingToTheSide = true;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 1;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = true;
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.sortingOrder = -1;
        }
    
        else if (playermovement.lookDir == "UpLeft")
        {
            facingToTheSide = false;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 4;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = false;
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = true;
        }
    }
}