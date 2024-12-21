using System.Collections.Generic;
using UnityEngine;
using static Inventory;
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
        public ArmorType HasArmorType;


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

        public bool CanAddItem(string itemName, Slot_UI ToSlot_UI = null, Slot ToSlot = null)
        {
            if ((this.itemName == itemName && count < maxAllowed) || ToSlot.IsEmpty)
            { // om man kan stacka items
                if (ToSlot_UI && ToSlot_UI.WantsArmorItem)
                {
                    if (ToSlot_UI != null && ToSlot.IsEmpty && ToSlot_UI.WantsArmorType == GetSlotArmorType(UI_Manager.draggedSlot.slotID, ToSlot_UI.inventory))
                        return true;
                    else
                        return false;
                }
                else
                   return true;
            }
            else if (ToSlot_UI != null && ToSlot.IsEmpty && ToSlot_UI.WantsArmorItem && ToSlot_UI.WantsArmorType == GetSlotArmorType(UI_Manager.draggedSlot.slotID, ToSlot_UI.inventory))
            { //om inget finns i slotten och vill ha armor armortype machar
                return true;
            }
                
            return false;
        }

        ArmorType GetSlotArmorType(int index, Inventory inventory)
        {
            Debug.Log(inventory.slots[index].HasArmorType);
            return inventory.slots[index].HasArmorType;
        }

        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            itemRarity = item.data.Rarity;

            if (item.data is ArmorData armorData)
            HasArmorType = armorData.armorType;

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

    public void MoveSlot(int fromIndex, int ToIndex, Inventory toInventory, Slot_UI Toslot_UI, int numToMove = 1)
    {
        Slot fromslot = slots[fromIndex];
        Slot Toslot = toInventory.slots[ToIndex];

            Debug.Log(Toslot.CanAddItem(fromslot.itemName, Toslot_UI, Toslot));
        if (Toslot.CanAddItem(fromslot.itemName, Toslot_UI, Toslot))
        {
            for (int i = 0; i < numToMove; i++)
            {   
                Toslot.AddItem(fromslot.itemName, fromslot.icon, fromslot.maxAllowed, fromslot.itemRarity);
                fromslot.RemoveItem();
            }
        }
    }

    public string FindItemInSlot(int index)
    {
        Slot slot = slots[index];
        return slot.itemName.ToString();
        
    }
}
