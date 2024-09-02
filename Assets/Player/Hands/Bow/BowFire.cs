using Unity.Mathematics;
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


    void Start()
    {
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
        
        //handels the shooting of the bow 
        if (playermovement.arrowcount >= 1)
        {
            if (Input.GetKey(KeyCode.Mouse1) || staneone)
            {
                holdtimer = Mathf.Clamp(holdtimer, 0, bowHoldTime);
                holdtimer += Time.deltaTime;

                playermovement.CanMove = false;
                playermovement.moveDirection = Vector2.zero;
                playermovement.canRoll = false;

                animator.SetBool("DrawingBow", true);
                ArrowSprite.SetActive(true);

                if (holdtimer > bowHoldTime)
                    staneone = true;

                if (!Input.GetKey(KeyCode.Mouse1))
                {
                    playermovement.CanMove = true;
                    playermovement.canRoll = true;
                    holdtimer = 0;
                    Instantiate(arrow, bulletTransform.position, quaternion.identity);
                    playermovement.arrowcount--;
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