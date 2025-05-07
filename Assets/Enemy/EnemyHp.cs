using System.Collections;
using UnityEngine;
public interface IEnemy
{
    int ID { get; set; }
}

public class EnemyHp : MonoBehaviour, IEnemy
{
    [HideInInspector] public float current_HP;
    public float Max_HP = 10;
    private Rigidbody2D rb;
    public float knockBackReduction = 1;

    [Header("Dmg Color Settings")]
    [SerializeField] private int speed = 3;
    [SerializeField] private float duration = 1;
    public AnimationCurve animationCurve;
    [SerializeField] private Material originalDeathMat;
    protected Material deathDMGmat;
    public ParticleSystem dmgSystem;

    [SerializeField] protected float invincibilityTimer = 0.25f;
    protected float CurrentInvincibilityTimer;

    [Header("Death")]
    public ParticleSystem[] ActivatingDeathParticles;

    private Transform[] protectedObjects;

    [SerializeField] private GameObject DamagePopUpPrefab;

    [HideInInspector] public bool isDead = false;
    public int ID { get; set; }

    public virtual void Start()
    {
        ID = 0;

        protectedObjects = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            protectedObjects[i] = transform.GetChild(i);
        }


        CurrentInvincibilityTimer = invincibilityTimer;
        if (rb == null)
        rb = GetComponent<Rigidbody2D>();
        current_HP = Max_HP;


        deathDMGmat = new Material(originalDeathMat);

        deathDMGmat = GetComponent<SpriteRenderer>().material;
    }


    private void FixedUpdate()
    {
        CurrentInvincibilityTimer -= Time.deltaTime; //subtrakts

        CurrentInvincibilityTimer = Mathf.Clamp(CurrentInvincibilityTimer, 0, invincibilityTimer);
    }

    public virtual void TakeDmg(float dmg, Vector3 AttackerPos, float KnockBackAmount, GameObject Arrow = null)
    {
        if (CurrentInvincibilityTimer <= 0)
        {
            if (Arrow)
                Arrow.GetComponent<SpriteRenderer>().material = deathDMGmat;

            dmgSystem.Play();

            ApplyKnockback(AttackerPos, KnockBackAmount);
            StartCoroutine(FlashDMGcolor());
            SpawnDmgPopUp(dmg);

            if ((current_HP - dmg) <= 0)
            {
                DisableCollider();

                StartCoroutine(RollDeathCGI());

                isDead = true;

                CombatEvents.EnemyDied(this); //säger till eventsystem att denhär har dött
            }
            else
            {
                current_HP -= dmg;
            }


            CurrentInvincibilityTimer = invincibilityTimer;
        }
        else
        {
            DisableCollider();
        }
    }

    private void HandleChildren()
    {
        foreach (Transform child in protectedObjects)
        {
            child.SetParent(null); //make every children an orphan
        }
    }
    protected IEnumerator suicide()
    {
        yield return null;

        for (int i = 0; i < protectedObjects.Length; i++)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(protectedObjects[i].gameObject);
        }

        Destroy(gameObject); //commit suicide
    }

    protected void ApplyKnockback(Vector3 attackerPos, float knockbackAmount)
    {
        Vector3 transformPos = transform.position;
        Vector2 KBdir = (transformPos - attackerPos).normalized;
        rb?.AddForce(KBdir * knockbackAmount / knockBackReduction, ForceMode2D.Impulse);
    }
    protected IEnumerator FlashDMGcolor()
    {
        float ElapsedTime = 0f;

        while (ElapsedTime < duration)
        {
            deathDMGmat.SetFloat("_FlashAmount", Mathf.Lerp(1, 0, animationCurve.Evaluate(ElapsedTime)));
            ElapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        deathDMGmat.SetFloat("_FlashAmount", 0f);
    }

    protected IEnumerator RollDeathCGI()
    {
        rb.velocity = Vector2.zero;

        float ElapsedTime = 0.5f;
        bool particlesPlayed = false;
        float Duration = 3f;

        while (ElapsedTime < Duration)
        {
            if (!particlesPlayed && ElapsedTime >= (Duration * 0.4f)) //halfway through
            {
                foreach (ParticleSystem deathParticle in ActivatingDeathParticles)
                {
                    deathParticle.Play();
                }
                particlesPlayed = true;
                    GetComponent<LootTable>()?.GenerateDrop();
            }

            deathDMGmat.SetFloat("_DisolveAmount", Mathf.Lerp(0, 1, ElapsedTime / Duration));
            ElapsedTime += Time.deltaTime;
            yield return null;
        }

        deathDMGmat.SetFloat("_DisolveAmount", 1f);
        StartCoroutine(suicide());
        HandleChildren();
    }

    protected void DisableCollider()
    {
        PolygonCollider2D polygoncolider = GetComponent<PolygonCollider2D>();
        if (polygoncolider != null)
            polygoncolider.enabled = false;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
            boxCollider.enabled = false;
    }

    protected void SpawnDmgPopUp(float damage)
    {
        GameObject damagepopupObject = Instantiate(DamagePopUpPrefab, transform.position, Quaternion.identity);
        DamagePopUpScript damagePopUpScript = damagepopupObject.GetComponent<DamagePopUpScript>();
        damagePopUpScript.Setup(damage);
    }
}
