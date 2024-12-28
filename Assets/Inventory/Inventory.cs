using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
        public Rarity itemRarity;
        public ArmorType HasArmorType;
        public ItemData itemData;


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
           // Debug.Log(ToSlot_UI.WantsArmorType == GetSlotArmorType(UI_Manager.draggedSlot.slotID, ToSlot_UI.inventory));
            if ((this.itemName == itemName && count < maxAllowed) || (ToSlot != null && ToSlot.IsEmpty))
            { // om man kan stacka items
                if (ToSlot_UI != null && ToSlot_UI.isArmorSlot)
                {
                    if (ToSlot_UI != null && ToSlot.IsEmpty && ToSlot_UI.WantsArmorType == GetSlotArmorType(UI_Manager.draggedSlot.slotID, ToSlot_UI.inventory))
                        return true;
                    else
                        return false;
                }
                else
                   return true;
            }
            else if (ToSlot_UI != null && ToSlot.IsEmpty && ToSlot_UI.isArmorSlot && ToSlot_UI.WantsArmorType == GetSlotArmorType(UI_Manager.draggedSlot.slotID, ToSlot_UI.inventory))
            { //om inget finns i slotten och vill ha armor armortype machar
                return true;
            }
                
            return false;
        }

        ArmorType GetSlotArmorType(int index, Inventory inventory)
        {
            if (UI_Manager.draggedSlot != null && UI_Manager.draggedSlot.SlothasArmorType != null)
            {
                return UI_Manager.draggedSlot.SlothasArmorType;
            }
            else
            {
                Debug.Log("DraggedSlot == Null!!!");
                return ArmorType.None;
            }

        }

        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            itemRarity = item.data.Rarity;
            this.maxAllowed = item.data.StackAmount;

            if (item.data is ArmorData armorData)
            HasArmorType = armorData.armorType;

            itemData = item.data;

            count++;

            ItemEvents.ItemPickedUp(item.data.itemName);
        }

        public void AddItem(string itemName, Sprite icon, int maxAllowed, Rarity Rarity, ArmorType ArmorType, ItemData itemData)
        {
            this.itemName = itemName;
            this.icon = icon;
            this.itemRarity = Rarity;
            this.HasArmorType = ArmorType;
            count++;
            this.maxAllowed = maxAllowed;
            this.itemData = itemData;
            this.maxAllowed = itemData.StackAmount;
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
                    itemRarity = Rarity.None;
                    HasArmorType = ArmorType.None;
                    itemData = null;
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

    public Rarity GetRarityFromSlot(int index)
    {
        return slots[index].itemRarity;
    }

    public void MoveSlot(int fromIndex, int ToIndex, Inventory toInventory, Slot_UI toSlot_UI, int numToMove = 1)
    {
        Slot fromslot = slots[fromIndex];
        Slot Toslot = toInventory.slots[ToIndex];

        if (Toslot.CanAddItem(fromslot.itemName, toSlot_UI, Toslot))
        {
            for (int i = 0; i < numToMove; i++)
            {   
                Toslot.AddItem(fromslot.itemName, fromslot.icon, fromslot.maxAllowed, fromslot.itemRarity, fromslot.HasArmorType, fromslot.itemData);
                fromslot.RemoveItem();
            }
        }
    }
    public string FindItemNameInSlot(int index)
    {
        Slot slot = slots[index];
        return slot.itemName.ToString();
    }
    public ItemData FindItemDataInSlot(int index)
    {
        Slot slot = slots[index];
        return slot.itemData;
    }
}
