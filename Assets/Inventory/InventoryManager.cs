using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, Inventory> inventoryByName = new Dictionary<string, Inventory>();

    [Header("Backpack")]
    public Inventory backpack;
    public int backpackSlotCount;


    [Header("HotBar")]
    public Inventory toolbar;
    public int toolbarSlotCount;
    
    [Header("ArmorSlots")]
    public Inventory ArmorSlot;
    public int ArmorSlotsCount;

    private void Awake()
    {
        backpack = new Inventory(backpackSlotCount);
        toolbar = new Inventory(toolbarSlotCount);
        ArmorSlot = new Inventory(ArmorSlotsCount);

        inventoryByName.Add("Backpack", backpack);
        inventoryByName.Add("Toolbar", toolbar);
        inventoryByName.Add("ArmorSlots", ArmorSlot);
    }

    public void Add(string inventoryName, Item item)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            inventoryByName[inventoryName].Add(item);
        }
    }

    public void AddBasedOnItem(Item item)
    {
        if (item.data is WeapondData)
        {
            toolbar.Add(item);

            GameManager.instance.ui_Manager.RefreshInventoryUI("Toolbar");
        }
        else
        {
            backpack.Add(item);
        }
    }

    public Inventory GetInventoryByName(string inventoryName)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            return inventoryByName[inventoryName];
        }
        return null;
    }
}