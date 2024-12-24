using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGoal : Goal
{
    public int EnemyID { get; set; }
    
    public KillGoal(int enemyID, string description, bool completed, int currentAmount, int requiredAmount)
    {
        this.EnemyID = enemyID;
        this.description = description;
        this.completed = completed;
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
