using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{

    [SerializeField] private float current_HP;
    public float Max_HP = 10;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    [Header("Dmg Color Settings")]
    [SerializeField] private int speed = 3;
    [SerializeField] private float duration = 1;
    public AnimationCurve animationCurve;
    public Material originalDeathMat;
    private Material deathDMGmat;
    public ParticleSystem dmgSystem;

    [SerializeField] private float invincibilityTimer = 0.25f;
    private float CurrentInvincibilityTimer;

    [Header("DisplaySettings")]
    [SerializeField] private Image HPSlider;
    [SerializeField] private float SliderSpeed = 5;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();


        CurrentInvincibilityTimer = invincibilityTimer;

        rb = GetComponent<Rigidbody2D>();
        current_HP = Max_HP;


        deathDMGmat = new Material(originalDeathMat);

        deathDMGmat = GetComponent<SpriteRenderer>().material;

        deathDMGmat.SetColor("_FlashColor", new Color(0.616f, 0.38f, 1f));
    }


    private void FixedUpdate()
    {
        CurrentInvincibilityTimer -= Time.fixedDeltaTime; //subtrakts

        CurrentInvincibilityTimer = Mathf.Clamp(CurrentInvincibilityTimer, 0, invincibilityTimer);
    }

    public void TakeDmg(float dmg, Vector3 AttackerPos, float KnockBackAmount) //meelee
    {
        if (CurrentInvincibilityTimer <= 0)
        {
            dmgSystem.Play();

            applyKnockback(AttackerPos, KnockBackAmount);
            StartCoroutine(flashDMGcolor());

            if ((current_HP - dmg) <= 0)
            {
                StartCoroutine(RemoveRb());
            }
            else
            {
                current_HP -= dmg;
            }
            StartCoroutine(updateHealthBar());
            CurrentInvincibilityTimer = invincibilityTimer;
        }
        
    }
    public void TakeDmg(float dmg, Vector3 AttackerPos, float KnockBackAmount, GameObject Arrow) //bow
    {
        if (CurrentInvincibilityTimer <= 0)
        {
            Arrow.GetComponent<SpriteRenderer>().material = deathDMGmat;

            dmgSystem.Play();

            applyKnockback(AttackerPos, KnockBackAmount);
            StartCoroutine(flashDMGcolor());

            if ((current_HP - dmg) <= 0)
            {
                StartCoroutine(RemoveRb());
            }
            else
            {
                current_HP -= dmg;
            }
            StartCoroutine(updateHealthBar());
            CurrentInvincibilityTimer = invincibilityTimer;
        }
        
    }

    private void applyKnockback(Vector3 attackerPos, float knockbackAmount)
    {
        if (rb != null)
        {

            Vector3 transformPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector2 KBdir = (transformPos - attackerPos).normalized;
            rb.AddForce(KBdir * knockbackAmount, ForceMode2D.Impulse);
        }
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

    IEnumerator RemoveRb()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(rb);

        playerMovement.Die();
    }

    public void changeHandMat()
    {
        GetComponent<SpriteRenderer>().material = deathDMGmat;


        Transform[] transformArray = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in transformArray)
        {
            if (child.name.Contains("__Weapond__"))
            {
                SpriteRenderer[] weaponRenderers = child.GetComponentsInChildren<SpriteRenderer>(true);

                foreach (SpriteRenderer renderer in weaponRenderers)
                {
                    renderer.material = deathDMGmat;
                }
            }
        }
    }

    IEnumerator updateHealthBar()
    {
        float healthPercentage = Mathf.Clamp01(current_HP / Max_HP);
        float difference = Mathf.Abs(HPSlider.fillAmount - healthPercentage);

        while (!Mathf.Approximately(HPSlider.fillAmount, healthPercentage))
        {
            HPSlider.fillAmount = Mathf.Lerp(HPSlider.fillAmount, healthPercentage, Time.deltaTime * SliderSpeed);
            yield return null;
        }

        HPSlider.fillAmount = healthPercentage;
    }
}