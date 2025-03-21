using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Water2D;

public class PlayerHp : MonoBehaviour
{
    [Header("PlayerStats")]
    [SerializeField] private float current_HP;
    public float Max_HP = 10;
    public float RegenerationAmount = 0.5f; //regen: hp / second
    [SerializeField] private bool canRegenerate = true;
    private Coroutine regenCoroutine;
    [SerializeField] private float RegenCooldown = 2f;
    private float CurrentRegenCooldown;
    public bool isInvincible = false;
    private bool IsInvincibilityCoututineRunning = false;

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


        rb = GetComponent<Rigidbody2D>();

        current_HP = Max_HP;

        deathDMGmat = new Material(originalDeathMat);
        deathDMGmat = GetComponent<SpriteRenderer>().material;
        playerMovement.WaterMat = deathDMGmat;
        deathDMGmat.SetColor("_FlashColor", new Color(0.616f, 0.38f, 1f));

        CurrentRegenCooldown = RegenCooldown;
    }

    private void FixedUpdate()
    {   
        CurrentInvincibilityTimer -= Time.deltaTime; 
        
        CurrentRegenCooldown -= Time.deltaTime;

        CurrentInvincibilityTimer = Mathf.Clamp(CurrentInvincibilityTimer, 0, invincibilityTimer);
        CurrentRegenCooldown = Mathf.Clamp(CurrentRegenCooldown, -1, RegenCooldown);

        if (canRegenerate && regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(Regeneration());
        }
        else if (!canRegenerate && regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null; // Clear the reference
        }

        if (CurrentRegenCooldown < 0)
            canRegenerate = true;
        else
            canRegenerate = false;

        if (isInvincible && !IsInvincibilityCoututineRunning)
        {
            StartCoroutine(InvisibleFrames());
        }
        if (CurrentInvincibilityTimer > 0)
            isInvincible = true;
        else
            isInvincible = false;
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
                StartCoroutine(RemoveRbNdie());
                current_HP = 0;
            }
            else
            {
                current_HP -= dmg;
            }
            StartCoroutine(updateHealthBar());
            CurrentInvincibilityTimer = invincibilityTimer;
            CurrentRegenCooldown = RegenCooldown;
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
                if (GameManager.instance.playerObstructor != null)
                    Destroy(GameManager.instance.playerObstructor);
                else
                    Debug.Log("Obstructor is null or already destroyed.");

                StartCoroutine(RemoveRbNdie());
                current_HP = 0;
            }
            else
            {
                current_HP -= dmg;
            }
            StartCoroutine(updateHealthBar());
            CurrentInvincibilityTimer = invincibilityTimer;
            CurrentRegenCooldown = RegenCooldown;
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

    private IEnumerator InvisibleFrames()
    {
        IsInvincibilityCoututineRunning = true;
        deathDMGmat.SetFloat("_Visible", 0);
        yield return new WaitForSeconds(0.1f);
        deathDMGmat.SetFloat("_Visible", 1);
        yield return new WaitForSeconds(0.1f);
        IsInvincibilityCoututineRunning = false;
    }

    IEnumerator RemoveRbNdie()
    {
        GameManager.instance.RemovePlayerObstructor();

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

        HPSlider.fillAmount = healthPercentage;
        yield return null;
    }

    private IEnumerator Regeneration()
    {
        while (current_HP < Max_HP)
        {
            yield return new WaitForSeconds(1f);

            current_HP += RegenerationAmount;
            current_HP = Mathf.Clamp(current_HP, 0, Max_HP);

            StartCoroutine(updateHealthBar());


            if (current_HP >= Max_HP)
            {
                regenCoroutine = null; // Clear the reference so the scrupt thiunks its no longer running
                yield break; // Stop the while loop
            }
        }

        regenCoroutine = null; // Reset coroutine reference when done
    }
}