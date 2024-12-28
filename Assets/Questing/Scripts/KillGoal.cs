using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGoal : Goal
{
    public int EnemyID { get; set; }
    
    public KillGoal(Quest quest, int enemyID, string description, bool completed, int currentAmount, int requiredAmount)
    {
        this.Quest = quest;
        this.EnemyID = enemyID;
        this.description = description;
        this.Completed = completed;
        this.currentAmount = currentAmount;
        this.requiredAmount = requiredAmount;
    }

    public override void Init()
    {
        base.Init();
        CombatEvents.OnEnemyDeath += EnemyDied;
    }

    void EnemyDied(IEnemy enemy) //has killed enemy
    {
        if (enemy.ID == this.EnemyID)
        {
            this.currentAmount++;
            evaluate();
        }
    }
}
