using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float moveSpeed = 3;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float knockbackAmount = 10f;
    public Vector2 moveDir;
    [SerializeField] private float rotationSpeed = 2;
    private void Start()
    {
        Destroy(gameObject, 20f);
    }
    private void Update()
    {
        MoveProjectile();
    }
    private void MoveProjectile()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)moveDir * moveSpeed, moveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHp playerHp = collision.gameObject.GetComponent<PlayerHp>();
        if (playerHp != null)
        {
            if (!playerHp.isInvincible)
            {
                playerHp.PlayerTakeDmg(damage, transform.position, knockbackAmount);
                Destroy(gameObject);
            }
        }
    }
}
