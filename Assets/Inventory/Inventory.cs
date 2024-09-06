using System.Collections.Generic;
using Unity.Loading;
using Unity.Profiling.Editor;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public class Inventory
{

    [System.Serializable]
    public class Slot
    {
        public Collectabletype type;
        public int count;
        public Sprite icon;

        public RarityLevel itemRarity;


        public Slot()
        {
            type = Collectabletype.NONE;
            count = 0;
        }

        public bool CanAddItem()
        {
            if (type == Collectabletype.Standard_Bow)
            {
                if (count < 1)
                {
                    return true;
                }
                return false;
            }
            else if (type == Collectabletype.Arrow)
            {
                if (count < 16)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (count < 64)
                {
                    return true;
                }
                return false;
            }
        }

        public void AddItem(Collectibal item)
        {
            this.type = item.type;
            this.icon = item.icon;
            itemRarity = item.Rarity;
            count++;
        }

        public void RemoveItem()
        {
            if (count > 0)
            {
                count--;

                if (count == 0)
                {
                    icon = null;
                    type = Collectabletype.NONE;
                }
            }
        }
    }
    public List<Slot> slots = new List<Slot>();

    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    public int GetArrowCount()
    {
        foreach (Slot slot in slots)
        {
            if (slot.type == Collectabletype.Arrow)
            {
                return slot.count;
            }
        }
        return 0;
    }
    public void RemoveArrow()
    {
        for (int i = 0; i < slots.Count; i++)
        {   
            if (slots[i].type == Collectabletype.Arrow)
            {
                Remove(i);
                break;
            }
        }
    }
    public void CheckForEmptySlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].type == Collectabletype.Arrow && slots.Count <= 0)
            {
                GameObject canvasObject = GameObject.Find("Canvas");
                InventoryUI inventoryUI = canvasObject.GetComponentInChildren<InventoryUI>();
                inventoryUI.ForceRemove(i);
                Debug.Log(i);
            }
        }
    }

    public void Add(Collectibal item)
    {
        foreach (Slot slot in slots)
        {
            if(slot.type == item.type && slot.CanAddItem())
            {
                slot.AddItem(item);
                return;
            }
        }
        foreach (Slot slot in slots)
        {
            if (slot.type == Collectabletype.NONE)
            {
                slot.AddItem(item);
                return;
            }
        }
    }

    public void Remove(int index)
    {
        slots[index].RemoveItem();
    }

    public RarityLevel GetRarityFromSlot(int index)
    {
        return slots[index].itemRarity;
    }
}
