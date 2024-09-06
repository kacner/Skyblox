using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BowFire : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject arrow;
    public Transform bulletTransform;
    float holdtimer;
    bool staneone = false;
    public float bowHoldTime = 1f;
    public Transform Hand;
    public Transform HoldingHand;
    public Transform ArrowSpriteTransform;
    public GameObject ArrowSprite;
    private Animator animator;

    private PlayerMovement playermovement;

    private float previousRotz;
    public float rotationThreshold;

    private float currentSpeed;
    private float halfSpeed;
    private float minHalfSpeed;

    public Player player;
    public InventoryUI inventoryUI;

    private bool IsNormalSpeed;
    private bool IsSlowed;

    public int ArrowSlotID;

    void Start()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();
        player = GetComponentInParent<Player>();

        playermovement = GetComponentInParent<PlayerMovement>();

        ArrowSprite.SetActive(false);

        animator = GetComponentInChildren<Animator>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //setts the right rotation of the bow when instantiated
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotz + 90);

        UpdateSortingLayers();
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
        player.inventory.CheckForEmptySlot();

        //handels the shooting of the bow 
        if (player.inventory.GetArrowCount() >= 1)
        {

            if (Input.GetKey(KeyCode.Mouse1) || staneone) //shooting starts
            {
                holdtimer = Mathf.Clamp(holdtimer, 0, bowHoldTime);
                holdtimer += Time.deltaTime;

                playermovement.canRoll = false;
                animator.SetBool("DrawingBow", true);
                ArrowSprite.SetActive(true);

                if (holdtimer > bowHoldTime)
                    staneone = true;

                if (!IsNormalSpeed)
                {
                    playermovement.maxSpeed /= 2;
                    IsNormalSpeed = true;
                    IsSlowed = false;
                }


                if (!Input.GetKey(KeyCode.Mouse1)) //shooting is over
                {
                    playermovement.canRoll = true;

                    if (!IsSlowed)
                    {
                        playermovement.maxSpeed *= 2;
                        IsSlowed = true;
                        IsNormalSpeed = false;
                    }

                    holdtimer = 0;
                    Instantiate(arrow, bulletTransform.position, quaternion.identity);
                    player.inventory.RemoveArrow();
                    inventoryUI.Refresh();
                    staneone = false;
                }
                
            }
            else
            {
                holdtimer = 0;
                animator.SetBool("DrawingBow", false);
                ArrowSprite.SetActive(false);
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
}