using System;

public static class CombatEvents
{
    public static event Action<IEnemy> OnEnemyDeath;

    public static void EnemyDied(IEnemy enemy)
    {
        OnEnemyDeath?.Invoke(enemy);
    }
}