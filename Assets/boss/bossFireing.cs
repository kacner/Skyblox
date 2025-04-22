using System.Collections;
using UnityEngine;

public class bossFireing : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int BurstAmount;
    [SerializeField] private int projectilesPerBurst;
    [SerializeField, Range(0, 359)] private float angleSpread;
    [SerializeField] private float timeBetweenBursts;
    [SerializeField] private float restTime;
    private bool isShooting = false;
    [SerializeField, Range(0, 10)] private float randomAngleRange;
    private BossHealth bossHealth;

    private void Start()
    {
        bossHealth = GetComponent<BossHealth>();
    }
    private void Attack()
    {
        if (!isShooting)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        Vector2 targetDir = Vector2.zero;
        if (GameManager.instance.player != null)
        targetDir = GameManager.instance.player.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        float startAngle = targetAngle;
        float endAngle = targetAngle;
        float currentAngle = targetAngle;
        float halfAngleSpread = 0;
        float angleStep = 0;

        if (angleSpread != 0)
        {
            angleStep = angleSpread / (projectilesPerBurst - 1);
            halfAngleSpread = angleSpread / 2;
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            currentAngle = startAngle;
        }

        for (int i = 0; i < BurstAmount; i++)
        {
            for (int j = 0; j < projectilesPerBurst; j++)
            {
                GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                if (bossHealth.hasTriggerdSecondPhase)
                    newBullet.GetComponent<SpriteRenderer>().color = new Color(0, 1f, 0);

                float baseAngle = startAngle + j * angleStep;

                float randomOffset = Random.RandomRange(-randomAngleRange, randomAngleRange);
                float modifiedAngle = baseAngle + randomOffset;

                float radianAngle = modifiedAngle * Mathf.Deg2Rad;
                Vector2 shootDirection = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
                //newBullet.transform.right = shootDirection;

                newBullet.GetComponent<BossProjectile>().moveSpeed = bulletSpeed;
                newBullet.GetComponent<BossProjectile>().moveDir = shootDirection.normalized;

                currentAngle += angleStep;
            }

            currentAngle = startAngle;

            yield return new WaitForSeconds(timeBetweenBursts);
        }

        yield return new WaitForSeconds(restTime);
        isShooting = false;
    }

    public void Burst()
    {
        bulletSpeed = 7;
        BurstAmount = 3;
        projectilesPerBurst = 3;
        angleSpread = 67;
        timeBetweenBursts = 0.3f;
        restTime = 0.5f;
        randomAngleRange = 10f;
        Attack();
    }
    public void Circle()
    {
        bulletSpeed = 5;
        BurstAmount = 3;
        projectilesPerBurst = 20;
        angleSpread = 359;
        timeBetweenBursts = 0.4f;
        restTime = 0.5f;
        randomAngleRange = 10f;
        Attack();
    }
    public void Barrier()
    {
        bulletSpeed = 3;
        BurstAmount = 2;
        projectilesPerBurst = 10;
        angleSpread = 40;
        timeBetweenBursts = 0.1f;
        restTime = 0.5f;
        randomAngleRange = 2f;
        Attack();
    }
}
