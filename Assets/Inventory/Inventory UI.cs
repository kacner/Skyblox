using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventorypanel;

    public Player player;

    public List<Slot_UI> slots = new List<Slot_UI>();

    private void Start()
    {
        inventorypanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
            Refresh(); //refreshed the inventory
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inventorypanel.SetActive(false);
        }
    }
    public void ToggleInventory()
    {
        if (!inventorypanel.activeSelf)
        {
            inventorypanel.SetActive(true);
        }
        else
        {
            inventorypanel.SetActive(false);
        }
    }
    public void Refresh()
    {
        if (slots.Count == player.inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (player.inventory.slots[i].type != Collectabletype.NONE)
                {
                    slots[i].SetItem(player.inventory.slots[i]);
                }
                else
                {
                    slots[i].setEmpty();
                }
            }
        }
    }

    public void Remove(int slotID)
    {
        GameObject itemToDropObject = GameManager.instance.itemManager.GetItemByType(player.inventory.slots[slotID - 1].type);

        if (itemToDropObject != null)
        {
            // Pass the GameObject directly to the dropItem method
            player.dropItem(itemToDropObject, slotID);
            player.inventory.Remove(slotID - 1);
            Refresh();
        }
    }
    
    public void ForceRemove(int slotID)
    {
        GameObject itemToDropObject = GameManager.instance.itemManager.GetItemByType(player.inventory.slots[slotID - 1].type);

        if (itemToDropObject != null)
        {
            player.inventory.Remove(slotID);
            Debug.Log(slotID);
            Refresh();
        }
    }
}
