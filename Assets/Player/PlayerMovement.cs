using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float acceleration = 300f;
    private Rigidbody2D rb;
    public float maxSpeed = 6.5f;
    public float orignialMaxSpeed = 4f;
    public bool CanMove = true;
    public bool IsRolling = false;
    public float RollForce = 10f;
    public float RollDuration = 0.5f;
    private Animator animator;
    public bool isDead = false;
    public float RollCooldown = 0.75f;
    private bool createtrailsprite = false;
    private Vector2 moveDirection;
    public float moveX;
    public float moveY;
    public float Velocity;
    public string newLookDir = "ntr";
    public string lookDir = "skibidiOhioRizz";
    public string lastLookDir = "toiletAnanasNasDas";

    //particles
    public ParticleSystem runningParticleSystem;

    //placeholder quiver & arrow settings
    public int arrowcount = 5;
    public GameObject cursorsprite;
    public bool useMousePos = false;
    public bool IsUsingBow = false;

    private HotbarScript hotbarscript;

    Vector2 mouseScreenPosition;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        InvokeRepeating("CreateTrailSprite", 0.1f, 0.1f);

        if (useMousePos = false && !isDead)
        {
            InvokeRepeating("UpdateHorVer", 0.01f, 0.01f);
        }
        else
        {
            CancelInvoke("UpdateHorVer");
        }
        DetermineLookDirection();

        hotbarscript = GetComponent<HotbarScript>();
    }


    void Update()
    {
        if (CanMove)
        {
            moveX = Input.GetAxisRaw("Horizontal"); //value -1 or 1. left or right
            moveY = Input.GetAxisRaw("Vertical"); //value -1 or 1. down and up

            moveDirection = new Vector2(moveX, moveY).normalized;

            newLookDir = DetermineLookDirection();

            // Update lastlookdir only when lookdir changes and newLookDir is not "None"
            if (newLookDir != lookDir)
            {
                lastLookDir = lookDir;
                lookDir = newLookDir;
            }
        }

        if (!useMousePos && !isDead && !IsInvoking("UpdateHorVer"))
        {
            InvokeRepeating("UpdateHorVer", 0.01f, 0.01f);
        }
        else if ((useMousePos || isDead) && IsInvoking("UpdateHorVer"))
        {
            CancelInvoke("UpdateHorVer");
        }

        if (Input.GetKeyDown(KeyCode.Space) && moveDirection != Vector2.zero && !IsRolling)
        {
            StartCoroutine(Roll());
        }

        if (Input.GetKeyDown(KeyCode.E) && !isDead)
        {
            Die();
        }

        
        if (rb.velocity.magnitude > 6)
        {
            runningParticleSystem.enableEmission = true;
            var emmitino = runningParticleSystem.emission;
            emmitino.rateOverDistance = 5f;
        }
        else if (rb.velocity.magnitude > 4)
        {
            runningParticleSystem.enableEmission = true;
            var emmitino = runningParticleSystem.emission;
            emmitino.rateOverDistance = 1f;
        }
        else
            runningParticleSystem.enableEmission = false;

        //Sprinting

        /*if (Input.GetKey(KeyCode.LeftControl))
        {
            maxSpeed = 6.5f;
        }
        else
        {
            maxSpeed = orignialMaxSpeed;
        }*/


        if (useMousePos && !isDead)
        {
            cursorsprite.active = true;

            mouseScreenPosition = Input.mousePosition;
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            Vector2 playerPos = transform.position;

            Vector2 relativeMousePos = mouseWorldPosition - playerPos;
           
            cursorsprite.transform.position = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

            mouseWorldPosition.x = Mathf.Round(relativeMousePos.x);
            mouseWorldPosition.y = Mathf.Round(relativeMousePos.y);

            Vector2 mousePosition2D = new Vector2(relativeMousePos.x, relativeMousePos.y);

            animator.SetFloat("Horizontal", mouseWorldPosition.x);
            animator.SetFloat("Vertical", mouseWorldPosition.y);

        }
        else if (!useMousePos)
        {
            cursorsprite.active = false;
        }

        if (Input.GetKey(KeyCode.M))
            Time.timeScale = 0.1f;

    }
    private void FixedUpdate()
    {
        Velocity = rb.velocity.magnitude;

        animator.SetFloat("Speed", rb.velocity.magnitude);

        if (!isDead)
        {
            Vector2 targetVelocity = moveDirection * maxSpeed; // desired velocity based on input
            Vector2 velocityReq = targetVelocity - rb.velocity; // how much we need to change the velocity

            Vector2 moveforce = velocityReq * acceleration; // Calculate the force needed to reach the target velocity considering acceleration

            rb.AddForce(moveforce * Time.fixedDeltaTime, ForceMode2D.Force);

            acceleration = maxSpeed + 325 / 0.9f;
            
            animator.speed = Mathf.Max(1, rb.velocity.magnitude / 5);
        }
    }
    private IEnumerator Roll()
    {
        createtrailsprite = true;
        CanMove = false;
        IsRolling = true;

        rb.AddForce(moveDirection * RollForce * 2, ForceMode2D.Impulse);
        yield return new WaitForSeconds(RollDuration);

        CanMove = true;
        createtrailsprite = false;
        yield return new WaitForSeconds(RollCooldown);
        IsRolling = false;
    }

    private void Die()
    {
        hotbarscript.destroyCurrentWeapond();
        isDead = true;
        CanMove = false;
        animator.SetBool("isDead", true);
        rb.velocity = new Vector2(0, 0);
    }
        
    private void UpdateHorVer()
    {
        if (moveDirection != Vector2.zero)
        {
            animator.SetFloat("Horizontal", moveX);
            animator.SetFloat("Vertical", moveY);
        }
    }
    private void CreateTrailSprite()
    {
        if (createtrailsprite)
        {
            GameObject clonedObject = Instantiate(gameObject);

            Component[] components = clonedObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component is SpriteRenderer || component is Transform)
                {
                    continue;
                }
                else
                {
                    Destroy(component);
                }
            }
            clonedObject.AddComponent<TrailSpriteScript>();
        }
    }
    private string DetermineLookDirection()
    {
        if (moveX == -1 && moveY == 1)
            return "UpLeft";
        else if (moveX == 1 && moveY == 1)
            return "UpRight";
        else if (moveX == -1 && moveY == -1)
            return "DownLeft";
        else if (moveX == 1 && moveY == -1)
            return "DownRight";
        else if (moveX == 0 && moveY == -1)
            return "Down";
        else if (moveX == 0 && moveY == 1)
            return "Up";
        else if (moveX == 1 && moveY == 0)
            return "Right";
        else if (moveX == -1 && moveY == 0)
            return "Left";
        else
            return lookDir;
    }

}