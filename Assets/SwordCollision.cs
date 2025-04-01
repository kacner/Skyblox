using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private float SwordDamage;
    private void Start()
    {
        SwordDamage = GetComponentInParent<SwordBase>().ThisSwordsWeapondDataSheet.Damage;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyHp enemyHP = collision.GetComponent<EnemyHp>();

        if (enemyHP != null)
        {
            Debug.Log("Hit detected on enemy!");

            enemyHP.TakeDmg(SwordDamage, transform.position, 20f);
        }
    }
}
