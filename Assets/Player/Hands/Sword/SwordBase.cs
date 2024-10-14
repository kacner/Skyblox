using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.UI;

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

    public ParticleSystem[] RotationPFX;

    public Transform HolsterFromSide;
    public Transform Sword;

    public Transform ControlObj;

    public float swingRecoilForce;

    [Space(10)]

    [Header("RotatingSettings")]
    public Transform RotatingSword;
    public float rotationSpeed = 100f;

    public bool IsAttacking;

    [Header("Collition")]
    public PolygonCollider2D hitbox;

    public bool hasHitTheFirstWay = false;


    void Start()
    {
        hitbox.enabled = false;
        foreach(ParticleSystem RotationPFX in RotationPFX)
        {
            var RotationPFXemition = RotationPFX.emission;
            RotationPFXemition.enabled = false;
        }

        animator = GetComponent<Animator>();

        playermovement = GetComponentInParent<PlayerMovement>();
        rotateAroundScript = GetComponentInChildren<RotateAround>();

        updateAnimationLayers();

        RotatingSword.GetComponent<SpriteRenderer>().enabled = false;
    }
    void Update()
    {
        if (playermovement.newLookDir == "Left" || playermovement.newLookDir == "UpLeft" || playermovement.newLookDir == "Up" || playermovement.newLookDir == "DownLeft")
        {
            ControlObj.transform.localScale = new Vector3(-1, 1, 1); //flipps parent so everything getts flipped
        }
        else if (playermovement.newLookDir == "Right" || playermovement.newLookDir == "UpRight" || playermovement.newLookDir == "Down" || playermovement.newLookDir == "UpLeft")
        {
            ControlObj.transform.localScale = new Vector3(1, 1, 1); //restores parent so everything getts restored
        }

        if (playermovement.newLookDir == "Up" || playermovement.newLookDir == "Left" || playermovement.newLookDir == "DownLeft")
        {
            transform.localPosition = new Vector2(0.015f, transform.localPosition.y); //applyes offset
        }
        else if (playermovement.newLookDir != "Up" && playermovement.newLookDir != "Left" && playermovement.newLookDir != "DownLeft")
        {
            transform.localPosition = new Vector2(0.0666f, transform.localPosition.y); //restores offset
        }

        updateAnimationLayers();

        if (playermovement.moveDirection != Vector2.zero) //updates animations if not standing still
        {
            animator.SetFloat("Horizontal", playermovement.moveX);
            animator.SetFloat("Vertical", playermovement.moveY);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && IsAttacking == false && playermovement.AllCanAttack)
        {
           StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        IsAttacking = true;
        hitbox.enabled = true;

        rotateAroundScript.matchRotation();

        foreach (SpriteRenderer SwordHands in SwordHands)
        {
            SwordHands.GetComponent<SpriteRenderer>().enabled = true;
        }

        playermovement.moreMouseBites();

        playermovement.IsAttacking = true;

        RotatingSword.GetComponent<SpriteRenderer>().enabled = true;

        playermovement.CanMove = false;
        playermovement.canRoll = false;

        Vector2 swingDirection = (Vector2)transform.position - playermovement.cursorspriteRectTransform.anchoredPosition;
        swingDirection.Normalize();
        playermovement.rb.AddForce(-swingDirection * swingRecoilForce, ForceMode2D.Force);
        playermovement.moveDirection = new Vector2(playermovement.moveDirection.x / 2, playermovement.moveDirection.y / 2);

        foreach (ParticleSystem RotationPFX in RotationPFX)
        {
            var RotationPFXemition = RotationPFX.emission;
            RotationPFXemition.enabled = true;
        }

        HolsterFromBackSpriteRenderer.enabled = false;
        HolsterFromSideSpriteRenderer.enabled = false;
        HolsterSpriteRenderer.enabled = false;
        HolsterUpLeftSpriteRenderer.enabled = false;
        HolsterUpRightSpriteRenderer.enabled = false;

        if (playermovement.newLookDir == "Down" || playermovement.newLookDir == "Left" || playermovement.newLookDir == "Right" || playermovement.newLookDir == "DownRight" || playermovement.newLookDir == "DownLeft")
        {
            SpriteRenderer[] Rotatingswordchilds = RotatingSword.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in Rotatingswordchilds)
            {
                spriteRenderer.sortingOrder = 2;
            }
            RotatingSword.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else if (playermovement.newLookDir == "Up" || playermovement.newLookDir == "UpLeft" || playermovement.newLookDir == "UpRight")
        {
            SpriteRenderer[] Rotatingswordchilds = RotatingSword.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in Rotatingswordchilds)
            {
                spriteRenderer.sortingOrder = -2;
            }
            RotatingSword.GetComponent<SpriteRenderer>().sortingOrder = -2;
        }

            LHandSpriteRenderer.enabled = false;
            RHandSpriteRenderer.enabled = false;
            SwordSpriteRenderer.enabled = false;

            float currentRotation = 0f;

            float neededRotation = 120f;

            while (currentRotation < neededRotation) //waits for x seconds
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

            yield return new WaitForSeconds(0.1f);

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



        foreach (SpriteRenderer SwordHands in SwordHands)
        {
            SwordHands.GetComponent<SpriteRenderer>().enabled = false;
        }
        yield return new WaitForSeconds(0.1f);

        foreach (ParticleSystem RotationPFX in RotationPFX)
        {
            var RotationPFXemition = RotationPFX.emission;
            RotationPFXemition.enabled = false;
        }

        RotatingSword.GetComponent<SpriteRenderer>().enabled = false;

        RotatingSword.transform.localPosition = new Vector2(0, -0.85f);
        RotatingSword.transform.localEulerAngles = new Vector3(0, 0, -135f);

        playermovement.CanMove = true;
        playermovement.canRoll = true;

        LHandSpriteRenderer.enabled = true;
        RHandSpriteRenderer.enabled = true;
        SwordSpriteRenderer.enabled = true; 

        //yield return new WaitForSeconds(0.5f);

        playermovement.IsAttacking = false;

        hitbox.enabled = false;

        IsAttacking = false;
    }

    public void updateAnimationLayers()
    {
        if (playermovement.newLookDir == "Up")
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
        }
        else if (playermovement.newLookDir == "Down")
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
        }
        else if (playermovement.newLookDir == "UpRight")
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
        }
        else if (playermovement.newLookDir == "Left" || playermovement.newLookDir == "DownLeft")
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

            //Sword.transform.localPosition = new Vector2(-0.108f, -0.242f);

            HolsterFromSide.transform.localEulerAngles = new Vector3(HolsterFromSide.transform.localEulerAngles.x, 45f, HolsterFromSide.transform.localEulerAngles.z);
        }
        else if (playermovement.newLookDir == "Right" || playermovement.newLookDir == "DownRight")
        {
            facingToTheRight = true;
            facingToTheLeft = false;
            SwordSpriteRenderer.sortingOrder = 1;
            HolsterSpriteRenderer.sortingOrder = -1 - 3;
            RHandSpriteRenderer.sortingOrder = 3;
            LHandSpriteRenderer.sortingOrder = 2;
            HolsterSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.enabled = true;
            HolsterFromBackSpriteRenderer.enabled = false;
            HolsterUpRightSpriteRenderer.enabled = false;
            HolsterUpLeftSpriteRenderer.enabled = false;
            HolsterFromSideSpriteRenderer.sortingOrder = -1;
            HolsterFromSide.transform.localPosition = new Vector2(-0.302f, -0.343f);

            //Sword.transform.localPosition = new Vector2(0.262f, -0.285f);

            HolsterFromSide.transform.localEulerAngles = new Vector3(HolsterFromSide.transform.localEulerAngles.x, 0, HolsterFromSide.transform.localEulerAngles.z);
        }

        else if (playermovement.newLookDir == "UpLeft")
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
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHp enemyHP = collision.GetComponent<EnemyHp>();

        if (enemyHP != null)
        {
            Debug.Log("Hit detected on enemy!");

            enemyHP.TakeDmg(1, transform, 20f);
        }
    }
}