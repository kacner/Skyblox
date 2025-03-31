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
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("RerollMovementState", 10, 10);
        InvokeRepeating("HandleFireingStates", 10, Random.RandomRange((float)3, 4));
        playerTransform = GameManager.instance.player.transform;
        bossFireing = GetComponent<bossFireing>();
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

    private void Update()
    {
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
            CurrentState = MovementStates.Standby;
        }
    }
}
