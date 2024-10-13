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
    public Material originalMat;
    private Material DMGmat;
    [SerializeField] private int speed = 3;
    [SerializeField] private float duration = 1;
    public AnimationCurve animationCurve;

    [Header("Death")]
    public ParticleSystem[] ActivatingDeathParticles;

    public bool kill = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        current_HP = Max_HP;


        DMGmat = new Material(originalMat);

        GetComponent<SpriteRenderer>().material = DMGmat;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            die();
        }
    }

    public void TakeDmg(int dmg, Transform AttackerPos)
    {

        if ((current_HP - dmg) <= 0)
        {
            die();
        }
        else
        {
            current_HP -= dmg;
            StartCoroutine(flashDMGcolor());
            applyKnockback(AttackerPos);
        }
    }


    private void die()
    {

        DMGmat.SetFloat("_FlashAmount", 1);


        foreach (ParticleSystem deathParticle in ActivatingDeathParticles)
        {
            deathParticle.Play();
        }



        Transform[] children = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach (Transform child in children)
        {
            child.SetParent(null); //make every children an orphan
        }



        StartCoroutine(suicide());
    }
    private IEnumerator suicide()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject); //commit suicide
    }


    private void applyKnockback(Transform attackerPos)
    {
        Vector2 KBdir = (transform.position - attackerPos.position).normalized;
        rb.AddForce(KBdir * 20f, ForceMode2D.Impulse);
    }
    private IEnumerator flashDMGcolor()
    {
        float ElapsedTime = 0f;

        while (ElapsedTime < duration)
        {
            DMGmat.SetFloat("_FlashAmount", Mathf.Lerp(1, 0, animationCurve.Evaluate(ElapsedTime)));
            ElapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        DMGmat.SetFloat("_FlashAmount", 0f);
    }
}
