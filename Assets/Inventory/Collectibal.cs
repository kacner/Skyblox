using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class Collectibal : MonoBehaviour
{
    [Header("Main Pickup Settings")]
    public Collectabletype type;
    public Sprite icon;
    private InventoryUI inventoryUI;

    [Space(10)]

    [Header("Bob-Settings")]
    public bool shouldbob = true;
    public float amplitude = 0.1f;
    public float frequency = 2f;
    private Vector3 startPos;

    [Space(10)]

    [Header("LightRaySettings")]
    public SpriteRenderer Spriterenderer;
    public float fadeDuration = 1.0f;
    public SpriteRenderer bowSpriteRenderer;
    public BoxCollider2D bowCollider;
    public Light2D LightSource;

    private void Start()
    {
        bowCollider = GetComponent<BoxCollider2D>();
        GameObject canvasObject = GameObject.Find("Canvas");
        inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();
        startPos = transform.position;
        bowSpriteRenderer.sprite = icon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            StartCoroutine(FadeOutLight(player));
        }
    }
    public void Update()
    {
        if (shouldbob)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }
    IEnumerator FadeOutLight(Player player)
    {
        Destroy(bowCollider);
        bowSpriteRenderer.sprite = null;
        player.inventory.Add(this);
        inventoryUI.Refresh();
        
        //var emission = transform.parent.GetComponent<ParticleSystem>().emission;
        //emission.enabled = false;

        yield return new WaitForSeconds(0.5f);

        float startIntensity = LightSource.intensity;

        float startAlpha = Spriterenderer.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = Spriterenderer.color;
            tmpColor.a = Mathf.Lerp(startAlpha, 0, progress);
            Spriterenderer.color = tmpColor;

            LightSource.intensity = Mathf.Lerp(startIntensity, 0, progress);

            progress += (rate * Time.fixedDeltaTime) / 2;

            yield return null;
        }

        Color finalColor = Spriterenderer.color;
        finalColor.a = 0;
        Spriterenderer.color = finalColor;
        LightSource.intensity = 0;
        LightSource.gameObject.SetActive(false);

        yield return new WaitForSeconds(4.5f);
        Destroy(transform.parent.gameObject);
        Destroy(transform.parent.transform.parent.gameObject);
        Destroy(this.gameObject);
    }
}

public enum Collectabletype
{
    NONE, Standard_Sword, Standard_Bow
}