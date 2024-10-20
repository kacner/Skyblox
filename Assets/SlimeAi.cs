using System.Collections;
using UnityEngine;

public class SlimeAi : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool shouldAi = true;
    private GameObject Target;
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    private Vector2 TargetDir;
    [SerializeField] private float TimeBeweenHops = 2f;

    private EnemyHp enemyHp;

    private Animator animator;

    [Header("Attack")]
    [SerializeField] private float Damage = 1f;
    [SerializeField] private float KnockbackAmount = 20f;
    [SerializeField] private bool isRetreetAvailable = true;

    private float lastRetreatTime = -1f;
    [SerializeField] private float retreatCooldown = 1f;
    private void Start()
    {
        Target = GameManager.instance.player.gameObject;
        enemyHp = GetComponent<EnemyHp>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("InvokeMoveMethod", TimeBeweenHops, TimeBeweenHops);
    }
    private void Update()
    {
        if (!enemyHp.isDead && Target != null && !GameManager.instance.player.GetComponent<PlayerMovement>().isDead)
        {
            TargetDir = Target.transform.position - transform.position;
            float facingDirection = TargetDir.x >= 0 ? 1 : -1;
            animator.SetFloat("Horizontal", facingDirection);
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, Target.transform.position) < 1f && Time.time > lastRetreatTime + retreatCooldown && isRetreetAvailable)
        {
            StartCoroutine(Retreet());
        }
    }

    private IEnumerator MoveToTarget()
    {
        yield return new WaitForSeconds(TimeBeweenHops / 2);

        float randomSideMovement = Random.Range(-1f, 1f);
        Vector2 perpendicular = new Vector2(-TargetDir.y, TargetDir.x); // skapar en 90 grader vinkelrät vector mot targetDir

        // Apply a small percentage of speed for the side movement
        Vector2 randomMovement = TargetDir.normalized * speed + perpendicular.normalized * randomSideMovement * (speed * 0.5f); // Adjust the multiplier to control side movement

        // Apply the force
        rb.AddForce(randomMovement, ForceMode2D.Force);
    }

    void InvokeMoveMethod()
    {
        StartCoroutine(MoveToTarget());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHp PlayerHP = collision.GetComponent<PlayerHp>();

        if (PlayerHP != null)
        {
            Debug.Log("Hit detected on Player!");

            PlayerHP.TakeDmg(Damage, transform.position, KnockbackAmount);
        }
    }

    IEnumerator Retreet()
    {
        lastRetreatTime = Time.time;
        yield return new WaitForSeconds(1f);

        rb.AddForce(-TargetDir.normalized * speed, ForceMode2D.Force);
    }
}
