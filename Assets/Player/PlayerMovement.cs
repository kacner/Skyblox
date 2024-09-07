using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Main Movement Variables")]
    public float acceleration = 300f;
    public float maxSpeed = 6.5f;
    public float orignialMaxSpeed = 4f;
    public bool CanMove = true;
    public bool isDead = false;
    [HideInInspector]
    public Vector2 moveDirection;
    public float moveX;
    public float moveY;
    public float Velocity;
    public string newLookDir = "MousePay";
    public string lookDir = "skibidiOhioRizz";
    public string lastLookDir = "toiletAnanasNasDas";
    public Vector2 lookDirVector;
    private Animator animator;
    private Vector2 facingDirection = Vector2.down; //vector2 storing lookdir
    private bool SlowMotion = false;
    [HideInInspector]
    public Rigidbody2D rb;
    private SpriteRenderer spriterenderer;

    [Space(10)]

    [Header("DodgeRolling Settings")]
    public bool canRoll = true;
    public float RollCooldown = 0.75f;
    public bool IsRolling = false;
    public float RollForce = 200f;
    public float RollDuration = 0.14f;
    public float StillBoostFactor = 5500;
    private bool createtrailsprite = false;
    private float emitParticleAfterInitialRoll = 0.3f;
    
    [Space(10)]

    [Header("Particle Systems")]
    public ParticleSystem runningParticleSystem;
    public ParticleSystem RollingPFX;
    public Color DashColor = Color.white;
    public Material SolidColorMat;
    public Material SpriteDefaultLit;

    [Space(10)]

    [Header("Quiver & Arrow Settings")]
    public RectTransform cursorspriteRectTransform;
    public bool useMousePos = false;
    [HideInInspector]
    public Vector2 mouseWorldPosition;
    public bool IsUsingBow = false;

    [Space(10)]

    [Header("Attack")]
    public bool IsAttacking = false;

    private HotbarScript hotbarscript;

    private SwordBase Swordbase;

    private InventoryUI inventoryUI;

    Vector2 mouseScreenPosition;

    [Space(10)]

    [Header("InventoryCameraMovement")]
    public CinemachineVirtualCamera virtualCamera;  // The Cinemachine virtual camera
    public float xOffset = 5f;

    void Start()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();

        Swordbase = GetComponentInChildren<SwordBase>();

        spriterenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        InvokeRepeating("CreateTrailSprite", 0.05f, 0.05f); //makes the function spawn a trailsprite every 0.05f seconds.

        if (useMousePos = false && !isDead)
        {
            InvokeRepeating("UpdateHorVer", 0.01f, 0.01f); //repeats the updating of lookdirection
        }
        else
        {
            CancelInvoke("UpdateHorVer"); //cancels the updating of lookdirection
        }
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        hotbarscript = GetComponent<HotbarScript>();
        if (useMousePos)
        {
            lookDirVector = DetermineLookDirectionVector2(mouseWorldPosition);
            DetermineLookDirection(mouseWorldPosition);
        }
        else
        {
            lookDirVector = DetermineLookDirectionVector2(moveDirection);
            DetermineLookDirection(moveDirection);
        }
    }


    void Update()
    {
        handleCamera();

        animator.SetBool("IsAttacking", IsAttacking);

        if (useMousePos)
        {
            newLookDir = DetermineLookDirection(mouseWorldPosition);
            lookDirVector = DetermineLookDirectionVector2(moveDirection);
            DetermineLookDirection(mouseWorldPosition);
        }

        if (CanMove)
        {
            moveX = Input.GetAxisRaw("Horizontal"); //value -1 or 1. left or right
            moveY = Input.GetAxisRaw("Vertical"); //value -1 or 1. down and up

            moveDirection = new Vector2(moveX, moveY).normalized;

            if (useMousePos)
            {
                newLookDir = DetermineLookDirection(mouseWorldPosition);
                lookDirVector = DetermineLookDirectionVector2(moveDirection);
                DetermineLookDirection(mouseWorldPosition);
            }
            else
            {
                newLookDir = DetermineLookDirection(moveDirection);
                lookDirVector = DetermineLookDirectionVector2(moveDirection);
                DetermineLookDirection(moveDirection);
            }

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

        if (Input.GetKeyDown(KeyCode.Space) && !IsRolling && canRoll)
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
            cursorspriteRectTransform.gameObject.SetActive(true);
            //Cursor.visible = false; //disables the windows cursor

            mouseScreenPosition = Input.mousePosition;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            Vector2 playerPos = transform.position;

            Vector2 relativeMousePos = mouseWorldPosition - playerPos;

            cursorspriteRectTransform.anchoredPosition = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y); //moves the cursor to the mousecursors location
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorspriteRectTransform.parent as RectTransform, mouseScreenPosition, null, out Vector2 localPoint);
            cursorspriteRectTransform.anchoredPosition = localPoint;

            mouseWorldPosition.x = Mathf.Round(relativeMousePos.x);
            mouseWorldPosition.y = Mathf.Round(relativeMousePos.y);

            Vector2 mousePosition2D = new Vector2(relativeMousePos.x, relativeMousePos.y);

            animator.SetFloat("Horizontal", mouseWorldPosition.x);
            animator.SetFloat("Vertical", mouseWorldPosition.y);

        }
        else if (!useMousePos)
        {
            //Cursor.visible = true;
            cursorspriteRectTransform.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.M))
            SlowMotion = !SlowMotion;

        if (SlowMotion == true)
            Time.timeScale = 0.1f;
        else
            Time.timeScale = 1f;

        if (Input.GetKey(KeyCode.N))
            Debug.Break();

        if (Input.GetKey(KeyCode.Backspace))
            SceneManager.LoadScene("SampleScene");

    }
    private void FixedUpdate()
    {
        if (rb.velocity.magnitude < 0.01f)
        {
            rb.velocity = Vector2.zero;
        }

        Velocity = rb.velocity.magnitude;

        animator.SetFloat("Speed", rb.velocity.magnitude);

        if (!isDead)
        {
            Vector2 targetVelocity = moveDirection * maxSpeed; // desired velocity based on input
            Vector2 velocityReq = targetVelocity - rb.velocity; // how much we need to change the velocity

            Vector2 moveforce = velocityReq * acceleration; //calculate the force needed to reach the target velocity considering acceleration

            rb.AddForce(moveforce * Time.fixedDeltaTime, ForceMode2D.Force); //applyes the movement to the rb

            acceleration = maxSpeed + 325 / 0.9f; //bases the acceleration of
            
            animator.speed = Mathf.Max(1, rb.velocity.magnitude / 5); //bases the animator speed of velocity so the feet match the ground distance
        }
    }
    private IEnumerator Roll()
    {
        spriterenderer.material = SolidColorMat; //changes the playermat to solidcolor

        Color initialColor = spriterenderer.color; //stores the innitial color of player
        spriterenderer.color = DashColor; //makes the spriterenderers color to new color

        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>(); //putts all gameobjects in an array
        foreach (GameObject child in allGameObjects)
        {
            if (child != null && child.name.Contains("__Weapond__"))
            {
                child.SetActive(false); 
            }
        }

        RollingPFX.enableEmission = true; //enables emition 
        createtrailsprite = true;
        CanMove = false;
        IsRolling = true;

        float elapsedTime = 0f; 

        while (elapsedTime < RollDuration) 
        {
            //Apply default walking velocity
            if (moveDirection == Vector2.zero)
                rb.AddForce(lookDirVector.normalized * StillBoostFactor * Time.fixedDeltaTime, ForceMode2D.Force); //applys velocity in the facing direction when player is standing still
            else
            rb.AddForce(lookDirVector.normalized * RollForce * Time.fixedDeltaTime, ForceMode2D.Impulse); //when player moving apply dash velocity

            elapsedTime += Time.fixedDeltaTime; //counts up the timer
            yield return null; //wait for next frame
        }

        CanMove = true;
        yield return new WaitForSeconds(emitParticleAfterInitialRoll); //splitts up cooldown into 2 parts   1/2
        createtrailsprite = false;
        RollingPFX.enableEmission = false;
        spriterenderer.color = initialColor;
        spriterenderer.material = SpriteDefaultLit;

        foreach (GameObject child in allGameObjects)
        {
            if (child != null && child.name.Contains("__Weapond__"))
            {
                child.SetActive(true);
            }
        }

        yield return new WaitForSeconds(RollCooldown - emitParticleAfterInitialRoll);  //splitts up cooldown into 2 parts   2/2
        IsRolling = false;
    }
    private void Die()
    {
        hotbarscript.destroyCurrentWeapond(); 
        isDead = true; 
        CanMove = false; 
        animator.SetBool("isDead", true); //updates animator
        rb.velocity = new Vector2(0, 0); //setts velocity to 0 
    }
    private void UpdateHorVer()
    {
        if (Swordbase != null)
        {
            if (moveDirection != Vector2.zero && Swordbase.IsAttacking == false) //if player isnt moving
            {
                animator.SetFloat("Horizontal", moveX);
                animator.SetFloat("Vertical", moveY);
            }
        }
        else if (moveDirection != Vector2.zero)
        {
            animator.SetFloat("Horizontal", moveX);
            animator.SetFloat("Vertical", moveY);
        
        }
    }
    private void CreateTrailSprite()
    {
        if (createtrailsprite)
        {
            GameObject clonedObject = Instantiate(gameObject); //instantiates a duplicate of the player

            Component[] components = clonedObject.GetComponents<Component>(); //stores all components in a array

            foreach (Component component in components)
            {
                if (component is SpriteRenderer || component is Transform)
                {
                    continue; // om komponent är spriterenderer eller transform
                }
                else
                {
                    Destroy(component); //annars destroyas allt
                }
            }
            clonedObject.AddComponent<TrailSpriteScript>(); //lägger på ett script på player dupen
        }
    }
    public string DetermineLookDirection(Vector2 Direction)
    {
        int x = Mathf.RoundToInt(Direction.x);
        int y = Mathf.RoundToInt(Direction.y);

        if (x <= -1 && y >= 1) //UpLeft
            return "UpLeft";
        else if (x >= 1 && y >= 1) //UpRight
            return "UpRight";
        else if (x <= -1 && y <= -1) //DownLeft
            return "DownLeft";
        else if (x >= 1 && y <= -1) //DownRight
            return "DownRight";
        else if (x == 0 && y <= -1) //Down
            return "Down";
        else if (x == 0 && y >= 1) //Up
            return "Up";
        else if (x >= 1 && y == 0) //Right
            return "Right";
        else if (x <= -1 && y == 0) //Left
            return "Left";
        else
            return newLookDir; //return the last known value
    }
    public Vector2 DetermineLookDirectionVector2(Vector2 Direction)
    {
        int x = Mathf.RoundToInt(Direction.x);
        int y = Mathf.RoundToInt(Direction.y);

        if (x <= -1 && y >= 1)
            facingDirection = new Vector2(-1, 1); // UpLeft
        else if (x >= 1 && y >= 1)
            facingDirection = new Vector2(1, 1); // UpRight
        else if (x <= -1 && y <= -1)
            facingDirection = new Vector2(-1, -1); // DownLeft
        else if (x >= 1 && y <= -1)
            facingDirection = new Vector2(1, -1); // DownRight
        else if (x == 0 && y <= -1)
            facingDirection = Vector2.down; // Down
        else if (x == 0 && y >= 1)
            facingDirection = Vector2.up; // Up
        else if (x >= 1 && y == 0)
            facingDirection = Vector2.right; // Right
        else if (x <= -1 && y == 0)
            facingDirection = Vector2.left; // Left

        return facingDirection; //return the last known value
    }

    public void moreMouseBites()
    {
        mouseScreenPosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 playerPos = transform.position;

        Vector2 relativeMousePos = mouseWorldPosition - playerPos;

        cursorspriteRectTransform.anchoredPosition = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y); //moves the cursor to the mousecursors location
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorspriteRectTransform.parent as RectTransform, mouseScreenPosition, null, out Vector2 localPoint);
        cursorspriteRectTransform.anchoredPosition = localPoint;

        mouseWorldPosition.x = Mathf.Round(relativeMousePos.x);
        mouseWorldPosition.y = Mathf.Round(relativeMousePos.y);

        Vector2 mousePosition2D = new Vector2(relativeMousePos.x, relativeMousePos.y);

        animator.SetFloat("Horizontal", mouseWorldPosition.x);
        animator.SetFloat("Vertical", mouseWorldPosition.y);

        newLookDir = DetermineLookDirection(mouseWorldPosition);
        lookDirVector = DetermineLookDirectionVector2(mouseWorldPosition);
    }

    private void handleCamera()
    {
        CinemachineFramingTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (UI_Manager.isInventoryToggeld)
        {
            Vector3 currentOffset = transposer.m_TrackedObjectOffset;
            currentOffset.x = xOffset;  // Adjust X offset
            transposer.m_TrackedObjectOffset = currentOffset;
        }
        else
        {
            Vector3 currentOffset = transposer.m_TrackedObjectOffset;
            currentOffset.x = 0;  // Adjust X offset
            transposer.m_TrackedObjectOffset = currentOffset;
        }

    }
}