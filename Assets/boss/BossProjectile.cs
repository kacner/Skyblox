using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float moveSpeed = 3;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float knockbackAmount = 10f;
    private Rigidbody2D rb;
    public Vector2 moveDir;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (collision.gameObject.TryGetComponent(out PlayerHp playerHP) != null)
            playerHP.TakeDmg(damage, transform.position, knockbackAmount);
    }
}
