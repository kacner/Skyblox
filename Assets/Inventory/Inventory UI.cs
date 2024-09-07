using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public string inventoryName;

    public List<Slot_UI> slots = new List<Slot_UI>();

    [SerializeField] private Canvas canvas;

    private Inventory inventory;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
    }

    private void Start()
    {
        inventory = GameManager.instance.player.inventory.GetInventoryByName(inventoryName);

        SetupSlots();
        Refresh();
    }
    public void Refresh()
    {
        if (slots.Count == inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(inventory.slots[i]);
                }
                else
                {
                    slots[i].setEmpty();
                }
            }
        }
    }

    public void Remove()
    {
        Item itemToDrop = GameManager.instance.itemManager.GetItemByName(inventory.slots[UI_Manager.draggedSlot.slotID].itemName);

        if (itemToDrop != null)
        {
            if (UI_Manager.dragSingle)
            {
                GameManager.instance.player.dropItem(itemToDrop);
                inventory.Remove(UI_Manager.draggedSlot.slotID);
            }
            else
            {
                GameManager.instance.player.dropItem(itemToDrop, inventory.slots[UI_Manager.draggedSlot.slotID].count);
                inventory.Remove(UI_Manager.draggedSlot.slotID, inventory.slots[UI_Manager.draggedSlot.slotID].count);
            }
            Refresh();
        }
        else
            print($" item to drop failed at {inventory.slots[UI_Manager.draggedSlot.slotID].itemName}");

        UI_Manager.draggedSlot = null;
    }

    public void SlotsBeginDrag(Slot_UI slot)
    {
        UI_Manager.draggedSlot = slot;
        UI_Manager.draggedIcon = Instantiate(UI_Manager.draggedSlot.itemIcon);
        UI_Manager.draggedIcon.transform.SetParent(canvas.transform);
        UI_Manager.draggedIcon.raycastTarget = false;
        UI_Manager.draggedIcon.rectTransform.sizeDelta = new Vector2(180, 180);

        MoveToMousePos(UI_Manager.draggedIcon.gameObject);
    }

    public void slotDrag()
    {
        MoveToMousePos(UI_Manager.draggedIcon.gameObject);
    }
    public void slotEndDrag()
    {
        Destroy(UI_Manager.draggedIcon.gameObject);
        UI_Manager.draggedIcon = null;

        //Debug.Log("Done draging : " + draggedslot.name);
    }
    public void slotDrop(Slot_UI slot)
    {
        if (UI_Manager.dragSingle)
        {
            UI_Manager.draggedSlot.inventory.MoveSlot(UI_Manager.draggedSlot.slotID, slot.slotID, slot.inventory);
        }
        else
        {
            UI_Manager.draggedSlot.inventory.MoveSlot(UI_Manager.draggedSlot.slotID, slot.slotID, slot.inventory, UI_Manager.draggedSlot.inventory.slots[UI_Manager.draggedSlot.slotID].count);
            
        }
        GameManager.instance.ui_Manager.RefreshAll();
    }

    private void MoveToMousePos(GameObject toMove)
    {
        if (canvas != null)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);

            toMove.transform.position = canvas.transform.TransformPoint(position);
        }
    }
    private void SetupSlots()
    {
        int counter = 0;
        foreach (Slot_UI slot in slots)
        {
            slot.slotID = counter;
            counter++;
            slot.inventory = inventory;
        }
    }
}
