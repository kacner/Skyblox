using System.Collections.Generic;
using Unity.Loading;
using Unity.Profiling.Editor;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class Inventory
{

    [System.Serializable]
    public class Slot
    {
        public string itemName;
        public int count;
        public Sprite icon;
        public int maxAllowed = 64;
        public string itemRarity;


        public Slot()
        {
            itemName = "";
            count = 0;
        }

        public bool IsEmpty
        {
            get
            {
                if (itemName == "" && count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanAddItem(string itemName)
        {
                if (this.itemName == itemName && count < maxAllowed)
                {
                    return true;
                }
                return false;
            
        }

        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            itemRarity = item.data.Rarity;
            count++;
        }

        public void AddItem(string itemName, Sprite icon, int maxAllowed, string Rarity)
        {
            this.itemName = itemName;
            this.icon = icon;
            this.itemRarity = Rarity;
            count++;
            this.maxAllowed = maxAllowed;
        }

        public void RemoveItem()
        {
            if (count > 0)
            {
                count--;

                if (count == 0)
                {
                    icon = null;
                    itemName = "";
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
            if (slot.itemName == "Arrow")
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
            if (slots[i].itemName == "Arrow")
            {
                Remove(i);
                break;
            }
        }
    }

    public void Add(Item item)
    {
        foreach (Slot slot in slots)
        {
            if(slot.itemName == item.data.itemName && slot.CanAddItem(item.data.itemName))
            {
                slot.AddItem(item);
                return;
            }
        }
        foreach (Slot slot in slots)
        {
            if (slot.itemName == "")
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
    public void Remove(int index, int numToRemove)
    {
        if (slots[index].count >= numToRemove)
        {
            for (int i = 0; i < numToRemove; i++)
            {
                Remove(index);
            }
        }
    }

    public string GetRarityFromSlot(int index)
    {
        return slots[index].itemRarity;
    }

    public void MoveSlot(int fromIndex, int ToIndex, Inventory toInventory, int numToMove = 1)
    {
        Slot fromslot = slots[fromIndex];
        Slot Toslot = toInventory.slots[ToIndex];

        if (Toslot.IsEmpty || Toslot.CanAddItem(fromslot.itemName))
        {
            for (int i = 0; i < numToMove; i++)
            {
                Toslot.AddItem(fromslot.itemName, fromslot.icon, fromslot.maxAllowed, fromslot.itemRarity);
                fromslot.RemoveItem();
            }
        }
    }
}
