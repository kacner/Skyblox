using UnityEngine;
public class Collectibal : MonoBehaviour
{
    public Collectabletype type;
    public Sprite icon;
    private InventoryUI inventoryUI;

    [Header("Bob-Settings")]
    public bool shouldbob = true;
    public float amplitude = 0.25f;
    public float frequency = 2f;
    private Vector3 startPos;

    private void Start()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();
        startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            player.inventory.Add(this);
            inventoryUI.Refresh();
            Destroy(this.gameObject);
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
}

public enum Collectabletype
{
    NONE, Standard_Sword, Standard_Bow
}