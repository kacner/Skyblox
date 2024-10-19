using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BowFire : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject arrow;
    public Transform bulletTransform;
    public float holdtimer = 0;
    public float bowHoldTime = 1f;
    public Transform Hand;
    public Transform HoldingHand;
    public Transform ArrowSpriteTransform;
    public GameObject ArrowSprite;
    private Animator animator;

    private PlayerMovement playermovement;

    private float previousRotz;
    public float rotationThreshold;

    public Player player;
    public InventoryUI inventoryUI;

    private bool IsNormalSpeed;
    private bool IsSlowed;

    public int ArrowSlotID;

    public string inventoryName = "Backpack";
    private Inventory inventory;
    private Inventory HotbarInventory;

    public ItemData ThisBowsItemDataSheet;

    public float maxArrowVelocity = 80;

    void Start()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();
        player = GetComponentInParent<Player>();

        playermovement = GetComponentInParent<PlayerMovement>();
        playermovement.useMousePos = true;

        ArrowSprite.SetActive(false);

        animator = GetComponentInChildren<Animator>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //setts the right rotation of the bow when instantiated
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotz + 90);

        UpdateSortingLayers();

        inventory = GameManager.instance.player.inventory.GetInventoryByName("Backpack");
        HotbarInventory = GameManager.instance.player.inventory.GetInventoryByName("Toolbar");
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
        if (inventory.GetArrowCount() >= 1 && playermovement.AllCanAttack || HotbarInventory.GetArrowCount() >= 1 && playermovement.AllCanAttack)
        {
            bool inventoryHadTheArrow;
            if (inventory.GetArrowCount() >= 1)
                inventoryHadTheArrow = true;
            else
                inventoryHadTheArrow = false;

            if (Input.GetKey(KeyCode.Mouse1)) // Shooting starts
            {
                holdtimer += Time.fixedDeltaTime;
                holdtimer = Mathf.Clamp(holdtimer, 0, bowHoldTime);

                playermovement.canRoll = false;
                animator.SetBool("DrawingBow", true);
                ArrowSprite.gameObject.SetActive(true);

                if (!IsNormalSpeed)
                {
                    playermovement.maxSpeed /= 2; // Restore original speed
                    IsNormalSpeed = true;
                    IsSlowed = false;
                }

            }
            else // Mouse1 button is not held
            {
                if (holdtimer > 0.2f) // Ensure we only shoot if the bow was drawn
                {
                    playermovement.canRoll = true;

                    if (!IsSlowed)
                    {
                        playermovement.maxSpeed *= 2; // Slow down the player
                        IsSlowed = true;
                        IsNormalSpeed = false;
                    }

                    GameObject Arrow = Instantiate(arrow, bulletTransform.position, Quaternion.identity);
                    ArrowScript arrowScript = Arrow.GetComponent<ArrowScript>();
                    arrowScript.force = Mathf.Clamp(holdtimer * maxArrowVelocity, 20, maxArrowVelocity);
                    arrowScript.TheBowsItemDataSheet = ThisBowsItemDataSheet;
                    arrowScript.latePlayerPos = transform.position;
                    arrowScript.MaxChargeTime = bowHoldTime;
                    arrowScript.ChargedTime = holdtimer;

                    if (inventoryHadTheArrow)
                        inventory.RemoveArrow();
                    else
                        HotbarInventory.RemoveArrow();

                    GameManager.instance.ui_Manager.RefreshInventoryUI("Backpack");
                    GameManager.instance.ui_Manager.RefreshInventoryUI("Toolbar");

                    holdtimer = 0;
                    animator.SetBool("DrawingBow", false);
                    ArrowSprite.gameObject.SetActive(false);
                }
                else
                {
                    holdtimer = 0;
                    animator.SetBool("DrawingBow", false);
                    ArrowSprite.gameObject.SetActive(false);
                }
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
        if (playermovement.newLookDir == "Up" || playermovement.newLookDir == "UpRight" || playermovement.newLookDir == "UpLeft")
        {
            bulletTransform.GetComponent<SpriteRenderer>().sortingOrder = -3;
            ArrowSpriteTransform.GetComponent<SpriteRenderer>().sortingOrder = -2;
            Hand.GetComponent<SpriteRenderer>().sortingOrder = -1;
            HoldingHand.GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
        else
        {
            bulletTransform.GetComponent<SpriteRenderer>().sortingOrder = 1;
            ArrowSpriteTransform.GetComponent<SpriteRenderer>().sortingOrder = 0;
            Hand.GetComponent<SpriteRenderer>().sortingOrder = 2;
            HoldingHand.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
    }

    private void OnDestroy()
    {
        playermovement.useMousePos = false;
    }
}