using System.Collections;
using UnityEngine;

public class SwordBase : MonoBehaviour
{
    private RotateAround rotateAroundScript;
    private PlayerMovement playermovement;
    public Transform Sword__Weapond;
    private Animator animator;

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

    public SpriteRenderer[] SwordHands;

    public ParticleSystem RotationPFX;

    public Transform HolsterFromSide;
    public Transform Sword;

    [Space(10)]

    [Header("RotatingSettings")]
    public Transform RotatingSword;
    public float rotationSpeed = 100f;

    void Start()
    {
        RotationPFX.Stop();

        HolsterFromSide = transform.Find("HolsterFromSide").transform; //finds object
        Sword = transform.Find("Sword").transform; //finds object

        animator = GetComponent<Animator>();

        playermovement = GetComponentInParent<PlayerMovement>();
        rotateAroundScript = GetComponentInChildren<RotateAround>();

        updateAnimations();

        RotatingSword.GetComponent<SpriteRenderer>().enabled = false;
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && playermovement.IsSpinAttacking == false)
        {
           StartCoroutine(SpinAttack());
        }
    }

    private IEnumerator SpinAttack()
    {
        rotateAroundScript.matchRotation();

        foreach (SpriteRenderer SwordHands in SwordHands)
        {
            SwordHands.GetComponent<SpriteRenderer>().enabled = true;
        }

        playermovement.moreMouseBites();

        playermovement.IsSpinAttacking = true;

        RotatingSword.GetComponent<SpriteRenderer>().enabled = true;

        playermovement.CanMove = false;
        playermovement.canRoll = false;
        playermovement.moveDirection = new Vector2(playermovement.moveDirection.x / 4, playermovement.moveDirection.y / 4);

        RotationPFX.Play();

        HolsterFromBackSpriteRenderer.enabled = false;
        HolsterFromSideSpriteRenderer.enabled = false;
        HolsterSpriteRenderer.enabled = false;
        HolsterUpLeftSpriteRenderer.enabled = false;
        HolsterUpRightSpriteRenderer.enabled = false;

        LHandSpriteRenderer.enabled = false;
        RHandSpriteRenderer.enabled = false;
        SwordSpriteRenderer.enabled = false;

        float currentRotation = 0f;

        float neededRotation = 120f;

        while(currentRotation < neededRotation) //waits for x seconds
        {
            float rotationThisFrame = rotationSpeed * Time.fixedDeltaTime;

            // Apply the rotation
            RotatingSword.RotateAround(transform.position, Vector3.forward, rotationThisFrame);

            // Update the current rotation
            currentRotation += rotationThisFrame;

            // Check if we've completed a full rotation
            if (currentRotation >= neededRotation)
            {
                RotatingSword.RotateAround(transform.position, Vector3.forward, neededRotation - (currentRotation - rotationThisFrame));
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (currentRotation > 0f)
        {
            float rotationThisFrame = rotationSpeed * Time.fixedDeltaTime;

            // Apply the reverse rotation
            RotatingSword.RotateAround(transform.position, Vector3.forward, -rotationThisFrame);

            // Update the current rotation
            currentRotation -= rotationThisFrame;

            // Check if we've completed the reverse rotation
            if (currentRotation <= 0f)
            {
                // Make sure we stop at the exact starting point (0 degrees)
                RotatingSword.RotateAround(transform.position, Vector3.forward, -currentRotation);
                currentRotation = 0f;  // Ensure currentRotation is exactly 0
                break;
            }
            yield return null;


        }

        RotatingSword.transform.localPosition = new Vector2(-0.023f, -1.128f);
        RotatingSword.transform.localEulerAngles = new Vector3(0, 0, -135f);

        RotationPFX.Stop();

        playermovement.CanMove = true;
        playermovement.canRoll = true;

        LHandSpriteRenderer.enabled = true;
        RHandSpriteRenderer.enabled = true;
        SwordSpriteRenderer.enabled = true;


        RotatingSword.GetComponent<SpriteRenderer>().enabled = false;

        playermovement.IsSpinAttacking = false;

        foreach (SpriteRenderer SwordHands in SwordHands)
        {
            SwordHands.GetComponent<SpriteRenderer>().enabled = false;
        }
        playermovement.newLookDir = playermovement.DetermineLookDirection(); //makes the playermovement script updates its 
        updateAnimations();
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