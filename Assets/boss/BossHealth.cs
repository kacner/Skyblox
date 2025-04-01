using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : EnemyHp
{
    [SerializeField] private Image bossBar;

    private BossMovement bossMovement;
    public override void Start()
    {
        base.Start();
        bossMovement = GetComponent<BossMovement>();
    }
    public override void TakeDmg(float dmg, Vector3 AttackerPos, float KnockBackAmount, GameObject Arrow = null)
    {
        if (CurrentInvincibilityTimer <= 0)
        {
            if (Arrow)
                Arrow.GetComponent<SpriteRenderer>().material = deathDMGmat;

            dmgSystem.Play();

            applyKnockback(AttackerPos, KnockBackAmount);
            StartCoroutine(flashDMGcolor());
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

            if (current_HP <= Max_HP / 2)
               StartCoroutine(bossMovement.SecondPhase());

            CurrentInvincibilityTimer = invincibilityTimer;
        }
        else
        {
            DisableCollider();
        }
    }
}
