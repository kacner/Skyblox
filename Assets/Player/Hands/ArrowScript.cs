using System.Collections;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;
    Vector3 direction;
    public GameObject WindFx;
    private SpriteRenderer WindFxSpriterenderer;
    private float WindFxAlpha = 1f;
    private float velocity;
    public GameObject ArrowCollectibalPrefab;
    public Sprite CutArrow;
    
    [HideInInspector] public Vector3 latePlayerPos;
    [HideInInspector] public ItemData TheBowsItemDataSheet;
    private bool hasHitEnemy = false;


    void Start()
    {
        WindFxSpriterenderer = WindFx.GetComponent<SpriteRenderer>();

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    private void Update()
    {
        if (WindFxSpriterenderer != null)
        {
            velocity = rb.velocity.magnitude;
            WindFxAlpha = Mathf.Clamp((velocity * 0.013f) + 0.016f, 0, 1);
            WindFxSpriterenderer.color = new Color(1, 1, 1, WindFxAlpha);

            WindFx.transform.localScale = new Vector3(0.025f * velocity + 1, 0.025f * velocity + 1, 0.025f * velocity + 1);
        }

        if (rb.velocity.magnitude < .1f && !hasHitEnemy)
            StartCoroutine(SpawnArrow(1f));
    }

    IEnumerator SpawnArrow(float timer)
    {
        Destroy(WindFxSpriterenderer);

        yield return new WaitForSeconds(timer);

        GameObject droppedItem =  Instantiate(ArrowCollectibalPrefab, transform.position, Quaternion.identity);
        droppedItem.transform.localScale = new Vector3(1, 1, 1);
        Destroy(rb);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHp enemyHP = collision.GetComponent<EnemyHp>();

        if (enemyHP != null)
        {
            hasHitEnemy = true;

            Debug.Log("Hit detected on enemy!");

            enemyHP.TakeDmg(TheBowsItemDataSheet.Damage, latePlayerPos, 20f);

            StartCoroutine(attachArrow(collision));
        }
    }

    IEnumerator attachArrow(Collider2D collision)
    {
        yield return new WaitForSeconds(0.1f);

        transform.SetParent(collision.gameObject.transform);

        SpriteRenderer thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisSpriteRenderer.sortingOrder = 0;
        thisSpriteRenderer.sprite = CutArrow;

        rb.velocity = new Vector2(0, 0);
        
        Destroy(rb);
        Destroy(WindFxSpriterenderer);
        Destroy(this);
    }
}
