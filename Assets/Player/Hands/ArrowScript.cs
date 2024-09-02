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
        velocity = rb.velocity.magnitude;
        WindFxAlpha = Mathf.Clamp((velocity * 0.013f) + 0.016f, 0, 1);
        WindFxSpriterenderer.color = new Color(1, 1, 1, WindFxAlpha);

        WindFx.transform.localScale = new Vector3(0.025f * velocity + 1, 0.025f * velocity + 1, 0.025f * velocity + 1);

        if (rb.velocity.magnitude < .1f)
            UpdateCondition();
    }

    private void UpdateCondition()
    {
        if (velocity < 0.1f)
        {
            Destroy(WindFxSpriterenderer);
            Destroy(rb);
            Destroy(this);
        }
    }
}
