using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class NewSwordBase1 : MonoBehaviour
{
    private PlayerMovement playermovement;

    private Animator animator; 
    
    public SpriteRenderer HolsterSpriteRenderer;
    public SpriteRenderer HolsterFromSideSpriteRenderer;
    public SpriteRenderer HolsterFromBackSpriteRenderer;
    public SpriteRenderer HolsterUpRightSpriteRenderer;
    public SpriteRenderer HolsterUpLeftSpriteRenderer;

    public SpriteRenderer[] SwordHands;

    public ParticleSystem[] RotationPFX;

    public float swingRecoilForce;

    public bool IsAttacking;

    [Header("Collition")]
    private BoxCollider2D hitbox;

    public bool hasHitTheFirstWay = false;

    private SpriteRenderer thisSpriterenderer;
    void Start()
    {
        animator = GetComponent<Animator>();
        thisSpriterenderer = GetComponentInChildren<SpriteRenderer>();
        hitbox = GetComponent<BoxCollider2D>();
        playermovement = GetComponentInParent<PlayerMovement>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(attack());
        }
    }

    private IEnumerator attack()
    {
        animator.SetBool("FirstAttack", true);

        IsAttacking = true;
        hitbox.enabled = true;

        foreach (SpriteRenderer SwordHands in SwordHands)
        {
            SwordHands.GetComponent<SpriteRenderer>().enabled = true;
        }

        playermovement.moreMouseBites();

        playermovement.IsAttacking = true;

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
            thisSpriterenderer.sortingOrder = 2;
        }
        else if (playermovement.newLookDir == "Up" || playermovement.newLookDir == "UpLeft" || playermovement.newLookDir == "UpRight")
        {
            thisSpriterenderer.sortingOrder = -2;
        }

        foreach (SpriteRenderer SwordHands in SwordHands)
        {
            SwordHands.GetComponent<SpriteRenderer>().enabled = false;
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

        playermovement.CanMove = true;
        playermovement.canRoll = true;

        foreach (SpriteRenderer SwordHands in SwordHands)
        {
            SwordHands.GetComponent<SpriteRenderer>().enabled = true;
        }


        hitbox.enabled = false;


        yield return new WaitForSeconds(0.5f); //cooldown

        animator.SetBool("FirstAttack", false);

        playermovement.IsAttacking = false;

        IsAttacking = false;
    }
}
