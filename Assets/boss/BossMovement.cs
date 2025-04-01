using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class BossMovement : MonoBehaviour
{

    [SerializeField] private MovementStates CurrentState = MovementStates.Standby;
    [SerializeField] private FireingStates CurrentFireingState = FireingStates.Circle;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float MovementSpeed = 10f;
    [SerializeField] private Transform[] retreetpositions;
    private Transform playerTransform;
    private Transform FurthestTransform = null;
    private bossFireing bossFireing;
    private Transform player;
    [SerializeField] private GameObject E;
    [SerializeField] private Sprite buttonDown;
    [SerializeField] private Sprite buttonUp;
    [SerializeField] private float range = 4;
    private bool hasBeenActivated = false;
    private SpriteRenderer E_spriteRenderer;
    private EnemyHp enemyhp;
    private SpriteRenderer spriteRenderer;
    private enum FireingStates
    {
        Circle, Burst, Barrier
    }
    private enum MovementStates
    {
        Following, Retreting, Standby
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyhp = GetComponent<EnemyHp>();
        enemyhp.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameManager.instance.player.transform;
        bossFireing = GetComponent<bossFireing>();
        player = GameManager.instance.player.transform;
        E_spriteRenderer = E.GetComponent<SpriteRenderer>();
    }

    void RerollMovementState()
    {
        int rnd = Random.RandomRange(1, 4);


        if (rnd == 1 && CurrentState != MovementStates.Standby)
            CurrentState = MovementStates.Standby;
        else if (rnd == 2 && CurrentState != MovementStates.Following)
            CurrentState = MovementStates.Following;
        else if (rnd == 3 && CurrentState != MovementStates.Retreting)
            CurrentState = MovementStates.Retreting;
        else
            CurrentState = MovementStates.Following;
    }
    void EnableBoss()
    {
        StartCoroutine(StartBlinkin());
        Invoke("EnableHp", 5);
        InvokeRepeating("RerollMovementState", 5, 10);
        InvokeRepeating("HandleFireingStates", 5, Random.RandomRange((float)3, 4));
    }
    IEnumerator StartBlinkin()
    {
        spriteRenderer.material.SetFloat("_FlashAmount", 1);
        float MainTimer = 0;
        float Timer = 0.8f;
        float nextToggleTime = Timer;

        while (MainTimer < 5)
        {
            MainTimer += Time.deltaTime;

            if (MainTimer >= nextToggleTime)
            {
                Timer *= 0.8f;  // adjust next wait interval
                nextToggleTime += Timer;

                if (spriteRenderer.material.GetFloat("_FlashAmount") == 0)
                    spriteRenderer.material.SetFloat("_FlashAmount", 1);
                else
                    spriteRenderer.material.SetFloat("_FlashAmount", 0);
            }

            yield return null;  // update every frame
        }

        spriteRenderer.material.SetFloat("_FlashAmount", 0);
    }
    void EnableHp()
    {
        enemyhp.enabled = true;
    }
    private void Update()
    {
        if (enemyhp.isDead)
            return;

        if (rb.velocity.x > 20f)
        {
            rb.velocity = new Vector2(20f,rb.velocity.y);
        }
        if (rb.velocity.y > 20f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 20f);
        }

        bool isWithinRange = (player.position - transform.position).magnitude < range;
        if (isWithinRange && !hasBeenActivated)
        {
            E.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                hasBeenActivated = true;
                EnableBoss();
                E_spriteRenderer.sprite = buttonDown;
            }
            else
            {
                E_spriteRenderer.sprite = buttonUp;
            }
        }
        else
            E.SetActive(false);

        if (CurrentState == MovementStates.Standby)
        {
            CurrentFireingState = FireingStates.Circle;
        }
        else if (CurrentState == MovementStates.Following)
        {
            CurrentFireingState = FireingStates.Burst;

            rb.AddForce((playerTransform.position - transform.position).normalized * MovementSpeed * Time.deltaTime, ForceMode2D.Force);
        }
        else if (CurrentState == MovementStates.Retreting)
        {
            CurrentFireingState = FireingStates.Barrier;
            Retreting();
        }
    }

    void HandleFireingStates()
    {
        if (enemyhp.isDead)
            return;

        if (CurrentFireingState == FireingStates.Circle)
        {
            bossFireing.Circle();
        }
        else if (CurrentFireingState == FireingStates.Burst)
        {
            bossFireing.Burst();
        }
        else if (CurrentFireingState == FireingStates.Barrier)
        {
            bossFireing.Barrier();
        }
    }
    public IEnumerator SecondPhase()
    {
        Transform[] arrows = GetComponentsInChildren<Transform>();
        List<Transform> ActualArrows = new List<Transform>();

        foreach (Transform arrow in arrows)
            if (arrow.name.Contains("Arrow"))
                ActualArrows.Add(arrow);
        
        for (int i = 0; i < ActualArrows.Count; i++)
        {
            rb.velocity = Vector2.zero;
            GameManager.instance.player.dropItem(GameManager.instance.itemManager.GetItemByName("Arrow"), transform.position);
            Destroy(ActualArrows[i].gameObject);
            yield return new WaitForSeconds(0.1f);
        }
    }
    void Retreting()
    {
        if (FurthestTransform == null)
        {
            float FurthestPosition = 0;
            for (int i = 0; i < retreetpositions.Length; i++)
            {
                float distance = (transform.position - retreetpositions[i].position).magnitude;
                if (FurthestPosition < distance)
                {
                    FurthestTransform = retreetpositions[i];
                    FurthestPosition = distance;
                }
            }
        }

        rb.AddForce((FurthestTransform.position - transform.position).normalized * MovementSpeed * Time.deltaTime, ForceMode2D.Force);
        if ((FurthestTransform.position - transform.position).sqrMagnitude < 1f)
        {
            FurthestTransform = null;
            rb.velocity = new Vector2(rb.velocity.x / 2, rb.velocity.y / 2);
            CurrentState = MovementStates.Standby;
        }
    }
}