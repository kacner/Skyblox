using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Item))]
public class Collectibal : MonoBehaviour
{
    [Header("Main PickupSettings")]
    private InventoryUI inventoryUI;
    private Player player;
    private SpriteRenderer CollectableSpriterenderer;
    [Space(10)]

    [Header("Bob-Settings")]
    public bool shouldbob = true;
    public float amplitude = 0.1f;
    public float frequency = 2f;
    private Vector3 startPos;

    [Space(10)]

    [Header("LightFxSettings")]
    public float fadeDuration = 1;
    private SpriteRenderer RaySpriterenderer;
    private Light2D LightSource;

    [Space(10)]

    [Header("RaritySettings")]
    private bool isResource = false;
    private Rarity Rarity;
    public GameObject[] RarityKit;
    public GameObject[] ResourceRarityKit;

    private GameObject createdParent;

    [Header("ItemData")]
    private ItemData ItemData;
    void Start()
    {
        TuneSettings();

        if (transform.parent == null)
        {
            CreateParent(this.gameObject);
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();

        startPos = transform.position;

        CollectableSpriterenderer = GetComponent<SpriteRenderer>();

        FixRarityKit();

        RaySpriterenderer = GetRaySpriterenderer(CollectableSpriterenderer);

        player = GameManager.instance.player;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null) //pickuplogic
        {
            StartCoroutine(ExitMusicForAFilm(player));
        }
    }

    IEnumerator ExitMusicForAFilm(Player player)
    {

        Item item = GetComponent<Item>();
        if (item != null)
        {
            StartCoroutine(PickUpAnimation());

            player.inventoryManager.AddBasedOnItem(item);

            Destroy(GetComponent<BoxCollider2D>()); //removes collition detec
        }
        else
        {
            print("failed");
        }

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

            progress += (rate * Time.deltaTime) / 2;

            yield return null;
        }

        Color finalColor = RaySpriterenderer.color;
        finalColor.a = 0;
        RaySpriterenderer.color = finalColor;
        LightSource.intensity = 0;
        LightSource.gameObject.SetActive(false);

        yield return new WaitForSeconds(4.5f);
        Destroy(createdParent); //suicide
    }

    private IEnumerator PickUpAnimation()
    {
        shouldbob = false;
        CollectableSpriterenderer.sortingLayerName = "Walk in Front of";

        float timer = 0;
        float Duration = 0.25f;

        Vector3 startpos = transform.position;
        Vector3 startScale = transform.localScale;

        while (timer < Duration)
        {
            timer += Time.deltaTime;

            transform.position = Vector3.Lerp(startpos, player.transform.position, timer / Duration);
            transform.localScale = Vector3.Lerp(startScale, startScale / 2, timer / Duration);

            yield return null;
        }
        transform.localScale = startScale / 2;
        transform.position = player.transform.position;

        GetComponent<SpriteRenderer>().sprite = null;

        inventoryUI.Refresh();

        if (ItemData is WeapondData)
            inventoryUI.RefreshHotBarWeapond();
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

    public void FixedUpdate()
    {
        if (shouldbob)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }

        if (transform.parent != null)
        {
            if (player.transform.position.y < transform.parent.transform.position.y - 0.2f)
            {
                SpriteRenderer[] thisSpriterenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer renderer in thisSpriterenderers)
                {
                    renderer.sortingLayerName = "Walk in Front of";
                    transform.parent.GetComponentInChildren<ParticleSystemRenderer>().sortingLayerName = "Walk in Front of";
                }
            }
            else
            {
                SpriteRenderer[] thisSpriterenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer renderer in thisSpriterenderers)
                {
                    renderer.sortingLayerName = "Walk behind";
                    transform.parent.GetComponentInChildren<ParticleSystemRenderer>().sortingLayerName = "Walk behind";
                }
            }
        }

    }

    public void FixRarityKit()
    {
        if (!isResource)
        {

            if (Rarity == Rarity.Ledgendairy)
            {
                GameObject RarityKitInstance = Instantiate(RarityKit[4], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == Rarity.Epic)
            {
                GameObject RarityKitInstance = Instantiate(RarityKit[3], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == Rarity.Rare)
            {
                GameObject RarityKitInstance = Instantiate(RarityKit[2], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == Rarity.Uncommon)
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
            if (Rarity == Rarity.Ledgendairy)
            {
                GameObject RarityKitInstance = Instantiate(ResourceRarityKit[4], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == Rarity.Epic)
            {
                GameObject RarityKitInstance = Instantiate(ResourceRarityKit[3], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == Rarity.Rare)
            {
                GameObject RarityKitInstance = Instantiate(ResourceRarityKit[2], transform.position, Quaternion.identity);
                RarityKitInstance.transform.SetParent(this.transform.parent);
            }
            else if (Rarity == Rarity.Uncommon)
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
    private void CreateParent(GameObject child)
    {
        string parentName = transform.parent != null ? transform.parent.name : (gameObject.name + "Parent");

        GameObject parent = new GameObject(parentName);

        parent.transform.position = child.transform.position;
        parent.transform.rotation = child.transform.rotation;

        child.transform.SetParent(parent.transform);

        createdParent = parent;
    }

    private void TuneSettings()
    {
        ItemData = GetComponent<Item>().data;
        //standard settings applyed for all data
        Rarity = ItemData.Rarity;
        GetComponent<SpriteRenderer>().sprite = ItemData.icon;

        //specific settings
        if (ItemData is ArmorData armorData)
        {
            //nothing to do yet
        }
        else if (ItemData is WeapondData weapondData)
        {
            //nothing to do yet
        }
        else if (ItemData is CollectableData)
        {
            isResource = true;
        }
    }
}
