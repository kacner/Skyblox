using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    
    [SerializeField] private int current_HP;
    public int Max_HP = 10;
    private Rigidbody2D rb;

    [Header("Dmg Color Settings")]
    [SerializeField] private int speed = 3;
    [SerializeField] private float duration = 1;
    public AnimationCurve animationCurve;
    public Material originalDeathMat;
    private Material deathDMGmat;

    [Header("Death")]
    public ParticleSystem[] ActivatingDeathParticles;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        current_HP = Max_HP;


        deathDMGmat = new Material(originalDeathMat);

        GetComponent<SpriteRenderer>().material = deathDMGmat;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(RollDeathCGI());
        }
    }

    public void TakeDmg(int dmg, Transform AttackerPos, float KnockBackAmount)
    {
        applyKnockback(AttackerPos, KnockBackAmount);
        StartCoroutine(flashDMGcolor());

        if ((current_HP - dmg) <= 0)
        {
            StartCoroutine(RollDeathCGI()); 
        }
        else
        {
            current_HP -= dmg;
        }
    }


    private void PostDeath()
    {
        Transform[] children = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach (Transform child in children)
        {
            child.SetParent(null); //make every children an orphan
        }
    }
    private IEnumerator suicide()
    {
        yield return null;
        Destroy(gameObject); //commit suicide
    }


    private void applyKnockback(Transform attackerPos, float knockbackAmount)
    {
        Vector2 KBdir = (transform.position - attackerPos.position).normalized;
        rb.AddForce(KBdir * knockbackAmount, ForceMode2D.Impulse);
    }
    private IEnumerator flashDMGcolor()
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


    private IEnumerator RollDeathCGI()
    {
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
            }

            deathDMGmat.SetFloat("_DisolveAmount", Mathf.Lerp(0, 1, ElapsedTime / Duration));
            ElapsedTime += Time.deltaTime;
            yield return null;
        }

        deathDMGmat.SetFloat("_DisolveAmount", 1f);
        StartCoroutine(suicide());
        PostDeath();
    }
}
