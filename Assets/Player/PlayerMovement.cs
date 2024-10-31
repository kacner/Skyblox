using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public bool dodgeRoll = false;
    public bool canRoll = true; //true
    public float RollCooldown = 0.75f; //0.75f
    public bool IsRolling = false; //false
    public float RollForce = 500f; //500f //forcemode.impulse
    public float RollDuration = 0.7f; //0.14f
    public float StillBoostFactor = 24000; //24000 //stillstanding forcemode.force

    public AnimationCurve rollSpeedCurve;

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
    public bool AllCanAttack = true;

    [Space(10)]

    [Header("Water")]
    [SerializeField] public bool isInWater;
    public Material WaterMat;
    public bool Grounded; 
    private Coroutine waterCoroutine;
    private enum  WaterState { None, Entering, Exiting}
    private WaterState waterState = WaterState.None;


    private HotbarScript hotbarscript;

    private SwordBase Swordbase;

    private InventoryUI inventoryUI;

    Vector2 mouseScreenPosition;


    private PlayerHp playerHp;

    private Animator CanvasAnimator;

    void Start()
    {
        playerHp = GetComponent<PlayerHp>();

        cursorspriteRectTransform.gameObject.SetActive(true);

        GameObject canvasObject = GameObject.Find("Canvas");
        inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();
        CanvasAnimator = canvasObject.GetComponent<Animator>();
        StartCoroutine(StartScreen());

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
        if (!Grounded)
        {
            ActWhenChangingDir();
        }

        // Handle water entry and exit
        if (isInWater && waterState != WaterState.Entering)
        {
            if (waterCoroutine != null)
            {
                StopCoroutine(waterCoroutine);
            }
            waterCoroutine = StartCoroutine(EnterWater());
            waterState = WaterState.Entering;
        }
        else if (!isInWater && waterState != WaterState.Exiting)
        {
            if (waterCoroutine != null)
            {
                StopCoroutine(waterCoroutine);
            }
            waterCoroutine = StartCoroutine(ExitWater());
            waterState = WaterState.Exiting;
        }

        // Reset water state if both conditions are false (no active water action)
        if (!isInWater && Grounded)
        {
            waterState = WaterState.None;
        }


        animator.SetBool("IsAttacking", IsAttacking);

        if (useMousePos)
        {
            newLookDir = DetermineLookDirection(mouseWorldPosition);
            lookDirVector = DetermineLookDirectionVector2(moveDirection);
            DetermineLookDirection(mouseWorldPosition);
        }

        if (CanMove && !GameManager.instance.ui_Manager.isInventoryToggeld)
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
        else
        {
            moveDirection = Vector2.zero;
        }

        if (!useMousePos && !isDead && !IsInvoking("UpdateHorVer"))
        {
            InvokeRepeating("UpdateHorVer", 0.01f, 0.01f);
        }
        else if ((useMousePos || isDead) && IsInvoking("UpdateHorVer"))
        {
            CancelInvoke("UpdateHorVer");
        }

        if (Input.GetKeyDown(KeyCode.Space) && !IsRolling && canRoll && !GameManager.instance.ui_Manager.isInventoryToggeld && rb != null)
        {
            StartCoroutine(Roll());
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && !isDead)
        {
            Die();
        }

        if (GameManager.instance.ui_Manager.isInventoryToggeld)
        {
            cursorspriteRectTransform.gameObject.SetActive(false);
        }
        else
        {
            cursorspriteRectTransform.gameObject.SetActive(true);
            inventoryUI.slotEndDrag();
        }

        
        if (rb != null && rb.velocity.magnitude > 6)
        {
            runningParticleSystem.enableEmission = true;
            var emmitino = runningParticleSystem.emission;
            emmitino.rateOverDistance = 5f;
        }
        else if (rb != null && rb.velocity.magnitude > 4)
        {
            runningParticleSystem.enableEmission = true;
            var emmitino = runningParticleSystem.emission;
            emmitino.rateOverDistance = 1f;
        }
        else
            runningParticleSystem.enableEmission = false;

        if (useMousePos && !isDead)
        {
            mouseScreenPosition = Input.mousePosition;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            Vector2 playerPos = transform.position;

            Vector2 relativeMousePos = mouseWorldPosition - playerPos;

            cursorspriteRectTransform.anchoredPosition = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y); //moves the cursor to the mousecursors location
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorspriteRectTransform.parent as RectTransform, mouseScreenPosition, null, out Vector2 localPoint);
            cursorspriteRectTransform.anchoredPosition = localPoint;

            mouseWorldPosition.x = Mathf.Round(relativeMousePos.x);
            mouseWorldPosition.y = Mathf.Round(relativeMousePos.y);

            animator.SetFloat("Horizontal", mouseWorldPosition.x);
            animator.SetFloat("Vertical", mouseWorldPosition.y);

        }
        else if (!useMousePos)
        {
            mouseScreenPosition = Input.mousePosition;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            cursorspriteRectTransform.anchoredPosition = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y); //moves the cursor to the mousecursors location
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorspriteRectTransform.parent as RectTransform, mouseScreenPosition, null, out Vector2 localPoint);
            cursorspriteRectTransform.anchoredPosition = localPoint;
        }

        if (Input.GetKeyDown(KeyCode.M))
            SlowMotion = !SlowMotion;

        if (SlowMotion == true)
            Time.timeScale = 0.1f;
        else
            Time.timeScale = 1f;

        if (Input.GetKey(KeyCode.N))
            Debug.Break();
    }
    private void FixedUpdate()
    {
        if (rb != null && rb.velocity.magnitude < 0.01f)
        {
            rb.velocity = Vector2.zero;
        }
        if (rb != null)
        {
            Velocity = rb.velocity.magnitude;
            animator.SetFloat("Speed", rb.velocity.magnitude);
        }
        if (rb != null && !isDead)
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
        /*spriterenderer.material = SolidColorMat; //changes the playermat to solidcolor

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

        if (dodgeRoll)
        {
            float dodgecount = 60f;

            if (dodgecount != 0)
            {
                dodgecount -= 1;
                rb.velocity = lookDirVector.normalized * 50f;
            }
        }
        else
        {
            float elapsedTime = 0f;

            while (elapsedTime < RollDuration)
            {
                if (moveDirection == Vector2.zero)
                    rb.AddForce(lookDirVector.normalized * StillBoostFactor * Time.fixedDeltaTime, ForceMode2D.Force);
                else
                    rb.AddForce(lookDirVector.normalized * RollForce * Time.fixedDeltaTime, ForceMode2D.Impulse);

                elapsedTime += Time.fixedDeltaTime;
                yield return null;
            }
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
        IsRolling = false;*/



        spriterenderer.material = SolidColorMat; // Change player material to solid color
        Color initialColor = spriterenderer.color; // Store initial color
        spriterenderer.color = DashColor; // Change color to dash color

        Transform[] allGameObjects = GetComponentsInChildren<Transform>(); // Get all game objects
        foreach (Transform child in allGameObjects)
        {
            if (child != null && child.name.Contains("__Weapond__"))
            {
                child.gameObject.SetActive(false);
            }
        }

        playerHp.isInvincible = true;

        RollingPFX.enableEmission = true; // Enable particle effects
        createtrailsprite = true;
        CanMove = false;
        IsRolling = true;

        float elapsedTime = 0f;

        while (elapsedTime < RollDuration)
        {
            float rollSpeedMultiplier = rollSpeedCurve.Evaluate(elapsedTime / RollDuration);
            rollSpeedMultiplier *= 5f;
            if (moveDirection == Vector2.zero) //initiate roll when still
            {
                rb.AddForce(lookDirVector.normalized * StillBoostFactor * rollSpeedMultiplier * Time.fixedDeltaTime, ForceMode2D.Force);
            }
            else //ini roll when moving
            {
                rb.AddForce(lookDirVector.normalized * RollForce * rollSpeedMultiplier * Time.fixedDeltaTime, ForceMode2D.Force);
            }
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        CanMove = true;
        yield return new WaitForSeconds(emitParticleAfterInitialRoll);
        createtrailsprite = false;
        RollingPFX.enableEmission = false;
        spriterenderer.color = initialColor;

        playerHp.isInvincible = false;

        playerHp.changeHandMat();

        foreach (Transform child in allGameObjects)
        {
            if (child != null && child.name.Contains("__Weapond__"))
            {
                child.gameObject.SetActive(true);
            }
        }

        yield return new WaitForSeconds(RollCooldown - emitParticleAfterInitialRoll);
        IsRolling = false;
    }
    public void Die()
    {
        Destroy(GetComponent<BoxCollider2D>());
        hotbarscript.destroyCurrentWeapond();
        isDead = true;
        CanMove = false; 
        animator.SetBool("isDead", true); //updates animator
        if (rb != null)
        rb.velocity = new Vector2(0, 0); //setts velocity to 0 
        AllCanAttack = false;

        StartCoroutine(DeathScreen());
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

    IEnumerator DeathScreen()
    {
        yield return new WaitForSeconds(2f);
        CanvasAnimator.SetTrigger("RollDeathScreen");
        yield return new WaitForSeconds(1f); //animation is over 
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator StartScreen()
    {
        CanvasAnimator.SetTrigger("RollStartScreen");
        yield return null;
    }


    IEnumerator EnterWater()
    {
        float time = 0f;
        float duration = 0.5f;
        while (time < duration)
        {
            // WaterMat.SetVector("_Offset", Vector2.Lerp(WaterMat.GetVector("_Offset"), new Vector2(0, 0.02f), time / duration));
            WaterMat.SetVector("_Offset", Vector2.Lerp(WaterMat.GetVector("_Offset"), new Vector2(0, 0.02f), time / duration));
            time += Time.deltaTime;
            print("EnteringWater    " + time + "time    " + duration + "duration");
            yield return null;
        }
        print("EXITenterwater");

        WaterMat.SetVector("_Offset", new Vector2(0, 0.02f));

        waterState = WaterState.None; // Reset state when done§ 
        waterCoroutine = null;
        Grounded = false;
    }
    IEnumerator ExitWater()
    {
        float time = 0f;
        float duration = 1f;
        while (time < duration)
        {
            //WaterMat.SetVector("_Offset", Vector2.Lerp(WaterMat.GetVector("_Offset"), new Vector2(0, 0), time / duration));
            WaterMat.SetVector("_Offset", Vector2.Lerp(WaterMat.GetVector("_Offset"), new Vector2(0, 0), time / duration));
            time += Time.deltaTime;
            print("Exeting    " + time + "time    " + duration + "duration");
            yield return null;
        }

        print("EXITexitwater");

        WaterMat.SetFloat("_CutofPosition", 1);

        WaterMat.SetVector("_Offset", new Vector2(0, 0f)); //set the CutofPos

        waterState = WaterState.None; // Reset state when done
        waterCoroutine = null;
        Grounded = true;
    }


    void ActWhenChangingDir()
    {
        if (lookDir == "Up")
        {
            WaterMat.SetFloat("_CutofPosition", 0.614f);
        }
        else if(lookDir == "UpRight")
        {
            WaterMat.SetFloat("_CutofPosition", 0.78f);
        }
        else if(lookDir == "Right" || lookDir == "DownRight")
        {
            WaterMat.SetFloat("_CutofPosition", 0.945f);
        }
        else if(lookDir == "Down")
        {
            WaterMat.SetFloat("_CutofPosition", 0.114f);
        }
        else if(lookDir == "Left" ||lookDir == "DownLeft")
        {
            WaterMat.SetFloat("_CutofPosition", 0.279f);
        }
        else if(lookDir == "UpLeft")
        {
            WaterMat.SetFloat("_CutofPosition", 0.447f);
        }
        
    }
}