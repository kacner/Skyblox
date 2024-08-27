using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwordBase : MonoBehaviour
{
    //private Camera mainCam;
    //private Vector3 mousePos;
    //private float previousRotz;
    public float rotationThreshold;
    private PlayerMovement playermovement;
    public Transform Sword__Weapond;
    private Animator animator;

    private bool sworddrawn;
    public bool facingToTheRight = false;
    public bool facingToTheLeft = false;

    public SpriteRenderer SwordSpriteRenderer;
    public SpriteRenderer LHandSpriteRenderer;
    public SpriteRenderer RHandSpriteRenderer;
    public SpriteRenderer HolsterSpriteRenderer;
    public SpriteRenderer HolsterFromSideSpriteRenderer;
    public SpriteRenderer HolsterFromBackSpriteRenderer;
    public SpriteRenderer HolsterUpRightSpriteRenderer;
    public SpriteRenderer HolsterUpLeftSpriteRenderer;

    public Transform HolsterFromSide;
    public Transform Sword;

    //public bool shouldRotate = false;
    void Start()
    {
        HolsterFromSide = transform.Find("HolsterFromSide").transform; //finds object
        Sword = transform.Find("Sword").transform; //finds object

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

    void Update()
    {
        if (playermovement.lookDir == "Left"|| playermovement.lookDir == "UpLeft"|| playermovement.lookDir == "Up"|| playermovement.lookDir == "DownLeft")
        {
            Sword__Weapond.transform.localScale = new Vector3(-1, 1, 1); //flipps parent so everything getts flipped
        }
        else if (playermovement.lookDir == "Right"|| playermovement.lookDir == "UpRight"|| playermovement.lookDir == "Down"|| playermovement.lookDir == "UpLeft")
        {
            Sword__Weapond.transform.localScale = new Vector3(1, 1, 1); //restores parent so everything getts restored
        }

        if (playermovement.lookDir == "Up" || playermovement.lookDir == "Left" || playermovement.lookDir == "DownLeft")
        {
            transform.localPosition = new Vector2(0.015f, transform.localPosition.y); //applyes offset
        }
        else if (playermovement.lookDir != "Up" && playermovement.lookDir != "Left" && playermovement.lookDir != "DownLeft")
        {
            transform.localPosition = new Vector2(0.0666f, transform.localPosition.y); //restores offset
        }

        updateAnimations();

        if (playermovement.moveDirection != Vector2.zero) //updates animations if not standing still
        {
            animator.SetFloat("Horizontal", playermovement.moveX);
            animator.SetFloat("Vertical", playermovement.moveY);
        }
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
            facingToTheRight = false;
            facingToTheLeft = false;
            HolsterSpriteRenderer.enabled = true;
            SwordSpriteRenderer.sortingOrder = 2 - 6;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 4;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterFromSideSpriteRenderer.enabled = false;
            HolsterFromBackSpriteRenderer.enabled = true;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = false;
            Debug.Log("Up");
        }
        else if (playermovement.lookDir == "Down")
        {
            facingToTheRight = false;
            facingToTheLeft = false;
            HolsterSpriteRenderer.enabled = true;
            SwordSpriteRenderer.sortingOrder = 2;
            HolsterSpriteRenderer.sortingOrder = -1;
            RHandSpriteRenderer.sortingOrder = 3;
            LHandSpriteRenderer.sortingOrder = 1;
            HolsterFromSideSpriteRenderer.enabled = false;
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = false;
            Debug.Log("Down");
        }
        else if (playermovement.lookDir == "UpRight")
        {
            facingToTheRight = false;
            facingToTheLeft = false;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 4;
            LHandSpriteRenderer.sortingOrder = 1 - 3;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = false;
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = true;
            HolsterUpLeftSpriteRenderer.enabled = false;
            Debug.Log("UpRight");
        }
        else if (playermovement.lookDir == "Left" || playermovement.lookDir == "DownLeft")
        {
            facingToTheLeft = true;
            facingToTheRight = false;
            SwordSpriteRenderer.sortingOrder = 2 - 1;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 0;
            LHandSpriteRenderer.sortingOrder = 1 + 2;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = true;
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.sortingOrder = -1 + 3;
            HolsterFromSide.transform.localPosition = new Vector2(-0.5f, -0.382f);

            Sword.transform.localPosition = new Vector2(-0.108f, -0.242f);
            Debug.Log(Sword.transform.localPosition);

            HolsterFromSide.transform.localEulerAngles = new Vector3(HolsterFromSide.transform.localEulerAngles.x, 45f, HolsterFromSide.transform.localEulerAngles.z);
            Debug.Log("Left || DownLeft");
        }
        else if (playermovement.lookDir == "Right" || playermovement.lookDir == "DownRight")
        {
            facingToTheRight = true;
            facingToTheLeft = false;
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
            HolsterFromSide.transform.localPosition = new Vector2(-0.302f, -0.343f);

            Sword.transform.localPosition = new Vector2(0.262f, -0.285f);

            HolsterFromSide.transform.localEulerAngles = new Vector3(HolsterFromSide.transform.localEulerAngles.x, 0, HolsterFromSide.transform.localEulerAngles.z);
            Debug.Log("Right || DownRight");
        }

        else if (playermovement.lookDir == "UpLeft")
        {
            facingToTheRight = false;
            facingToTheLeft = false;
            SwordSpriteRenderer.sortingOrder = 2 - 3;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3 - 4;
            LHandSpriteRenderer.sortingOrder = 1 - 0;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = false;
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = true;
            Debug.Log("UpLeft");
        }
    }
}