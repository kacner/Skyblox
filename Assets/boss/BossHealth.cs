using UnityEngine;
using UnityEngine.UI;

public class BossHealth : EnemyHp
{
    [SerializeField] private Image bossBar;

    private BossMovement bossMovement;
    private bool hasTriggerdSecondPhase = false;
    public override void Start()
    {
        base.Start();
        bossMovement = GetComponent<BossMovement>();
    }
    public override void TakeDmg(float dmg, Vector3 AttackerPos, float KnockBackAmount, GameObject Arrow = null)
    {
        if (this.isActiveAndEnabled == false)
            return;

        if (CurrentInvincibilityTimer <= 0)
        {
            if (Arrow)
                Arrow.GetComponent<SpriteRenderer>().material = deathDMGmat;

            dmgSystem.Play();

            ApplyKnockback(AttackerPos, KnockBackAmount);
            StartCoroutine(FlashDMGcolor());
            SpawnDmgPopUp(dmg);

            bossBar.fillAmount = (current_HP - dmg)/ Max_HP;
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

            if (current_HP <= Max_HP / 2 && !hasTriggerdSecondPhase)
            {
                StartCoroutine(bossMovement.SecondPhase());
                hasTriggerdSecondPhase = true;
            }

            CurrentInvincibilityTimer = invincibilityTimer;
        }
        else
        {
            DisableCollider();
        }
    }
}
