using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Collectibal : MonoBehaviour
{
    [Header("Main PickupSettings")]
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

    [Header("LightFxSettings")]
    private SpriteRenderer RaySpriterenderer;
    public float fadeDuration = 1;
    private Light2D LightSource;

    [Space(10)]

    [Header("RaritySettings")]
    public bool isResource = false;
    public RarityLevel Rarity;
    public GameObject[] RarityKit;
    public GameObject[] ResourceRarityKit;

    void Start()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();

        startPos = transform.position;

        SpriteRenderer collectibalSpriterenderer = GetComponent<SpriteRenderer>();
        collectibalSpriterenderer.sprite = icon;

        FixRarityKit();

        RaySpriterenderer = GetRaySpriterenderer(collectibalSpriterenderer);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        Player player = collision.GetComponent<Player>();
        if (player) //pickuplogic
        {
            StartCoroutine(ExitMusicForAFilm(player));
        }
    }
    
    IEnumerator ExitMusicForAFilm(Player player)
    {
        GetComponent<SpriteRenderer>().sprite = null;
        player.inventory.Add(this);
        Destroy(GetComponent<BoxCollider2D>());
        inventoryUI.Refresh();

        var emission = transform.parent.GetComponentInChildren<ParticleSystem>().emission;
        emission.enabled = false;

        LightSource = transform.parent.GetComponentInChildren<Light2D>();

        yield return new WaitForSeconds(0.5f);

        float startIntensity = LightSource.intensity;

        float startAlpha = RaySpriterenderer.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = RaySpriterenderer.color;
            tmpColor.a = Mathf.Lerp(startAlpha, 0, progress);
            RaySpriterenderer.color = tmpColor;

            LightSource.intensity = Mathf.Lerp(startIntensity, 0, progress);

            progress += (rate * Time.fixedDeltaTime) / 2;

            yield return null;
        }

        Color finalColor = RaySpriterenderer.color;
        finalColor.a = 0;
        RaySpriterenderer.color = finalColor;
        LightSource.intensity = 0;
        LightSource.gameObject.SetActive(false);

        yield return new WaitForSeconds(4.5f);
        Destroy(transform.parent.gameObject); //suicide
    }

    private SpriteRenderer GetRaySpriterenderer(SpriteRenderer CollectableSpriterendere)
    {
        SpriteRenderer[] spriterenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spriterenderers.Length; i++)
        {
            if (spriterenderers[i].gameObject != CollectableSpriterendere.gameObject)
            {
                return spriterenderers[i];
            }
        }
        return null;
    }

    public void Update()
    {
        if (shouldbob)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }

    public void FixRarityKit()
    {
        if (!isResource)
        {

            if (Rarity == RarityLevel.Ledgendairy)
            {
                GameObject RarityKitInstance = Instantiate(RarityKit[4], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == RarityLevel.Epic)
            {
                GameObject RarityKitInstance = Instantiate(RarityKit[3], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == RarityLevel.Rare)
            {
                GameObject RarityKitInstance = Instantiate(RarityKit[2], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == RarityLevel.Uncommon)
            {
                GameObject RarityKitInstance = Instantiate(RarityKit[1], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else //Rarity = Common
            {
                GameObject RarityKitInstance = Instantiate(RarityKit[0], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
        }
        else
        {
            if (Rarity == RarityLevel.Ledgendairy)
            {
                GameObject RarityKitInstance = Instantiate(ResourceRarityKit[4], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == RarityLevel.Epic)
            {
                GameObject RarityKitInstance = Instantiate(ResourceRarityKit[3], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == RarityLevel.Rare)
            {
                GameObject RarityKitInstance = Instantiate(ResourceRarityKit[2], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == RarityLevel.Uncommon)
            {
                GameObject RarityKitInstance = Instantiate(ResourceRarityKit[1], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else //Rarity = Common
            {
                GameObject RarityKitInstance = Instantiate(ResourceRarityKit[0], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
        }
    }
}
public enum Collectabletype
{
    NONE, Standard_Sword, Standard_Bow, Arrow
}

public enum RarityLevel
{
    Common, Uncommon, Rare, Epic, Ledgendairy
}
