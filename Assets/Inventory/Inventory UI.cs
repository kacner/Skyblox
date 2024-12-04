using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public string inventoryName;

    public List<Slot_UI> slots = new List<Slot_UI>();

    [SerializeField] private Canvas canvas;

    [SerializeField] private Inventory inventory;
    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private HashSet<Slot_UI> previouslyRaycastedSlots = new HashSet<Slot_UI>();
    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
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
                    slots[i].itemIcon.enabled = true;

                    Color TempColor = slots[i].RarityBackLight.color;
                    TempColor.a = 1;
                    slots[i].RarityBackLight.color = TempColor;
                }
                else
                {
                    slots[i].setEmpty();
                }
            }
        }
        else
        {
            print(slots.Count + "      " + inventory.slots.Count);

        }
    }

    public void Remove()
    {
        if(UI_Manager.draggedSlot != null)
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
                canvas.GetComponentInChildren<ToolBar_UI>().selectSlot(canvas.GetComponentInChildren<ToolBar_UI>().selectedSlotNumber);
            }

            UI_Manager.draggedSlot = null;
        }
    }

    public void SlotsBeginDrag(Slot_UI slot)
    {
        UI_Manager.draggedSlot = slot;
        UI_Manager.draggedIcon = Instantiate(UI_Manager.draggedSlot.itemIcon);
        UI_Manager.draggedIcon.transform.SetParent(canvas.transform);
        UI_Manager.draggedIcon.raycastTarget = false;
        StartCoroutine(OnlerpPickedUpItemScale());

        MoveToMousePos(UI_Manager.draggedIcon.gameObject);
        slot.itemIcon.enabled = false;

        Color TempColor = slot.RarityBackLight.color;
        TempColor.a = 0;
        slot.RarityBackLight.color = TempColor;
    }
    public void slotDrag()
    {
        if (UI_Manager.draggedIcon != null)
        {
            MoveToMousePos(UI_Manager.draggedIcon.gameObject);

            DestroySlotHighlight();

            // Perform raycast
            pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            CreateSlotHighlight(results);

        }
    }
    public void slotEndDrag()
    {
        if (UI_Manager.draggedIcon != null)
        {
            StartCoroutine(HandleSlotEndDrag());
        }
    }
    public void slotDrop(Slot_UI slot)
    {
        if (slot != null)
        {
            if (UI_Manager.dragSingle)
            {
                UI_Manager.draggedSlot.inventory.MoveSlot(UI_Manager.draggedSlot.slotID, slot.slotID, slot.inventory);
            }
            else
            {
                UI_Manager.draggedSlot.inventory.MoveSlot(UI_Manager.draggedSlot.slotID, slot.slotID, slot.inventory, UI_Manager.draggedSlot.inventory.slots[UI_Manager.draggedSlot.slotID].count);

            }
        }
        GameManager.instance.ui_Manager.RefreshAll();
        canvas.GetComponentInChildren<ToolBar_UI>().selectSlot(canvas.GetComponentInChildren<ToolBar_UI>().selectedSlotNumber);
    }

    private void MoveToMousePos(GameObject toMove)
    {
        if (canvas != null && toMove != null && Input.mousePosition != null)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
            
            if (position != null)
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
    void CreateSlotHighlight(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            Slot_UI slot = result.gameObject.GetComponent<Slot_UI>();
            if (slot != null && slot.RarityBackLight.color.a == 0)
            {
                Image image = slot.GetComponent<Image>();
                Color tempColor = image.color;
                tempColor.a = 0.05f;
                image.color = tempColor;


                previouslyRaycastedSlots.Add(slot);
            }
        }
    }
    void DestroySlotHighlight()
    {
        foreach (Slot_UI slot in previouslyRaycastedSlots)
        {
            if (slot != null && slot.GetComponent<Image>() != null)
            {
                Image image = slot.GetComponent<Image>();
                Color tempColor = image.color;
                tempColor.a = 0;
                image.color = tempColor;
            }
        }
        previouslyRaycastedSlots.Clear();
    }
    
    private IEnumerator HandleSlotEndDrag()
    {
        StartCoroutine(OfflerpPickedUpItemScale()); //lerp slot size insted of dragged icon size
        yield return null;
        
    }

    private IEnumerator OnlerpPickedUpItemScale()
    {
        if (UI_Manager.draggedIcon.rectTransform != null)
        {

            float timer = 0;
            float duration = 0.15f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                UI_Manager.draggedIcon.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(120, 120), new Vector2(165, 165), timer / duration);
                yield return null;
            }
            UI_Manager.draggedIcon.rectTransform.sizeDelta = new Vector2(155, 155);

        }
        else
            print("Erhmmmm.. something went very wrong");
    }

    private IEnumerator OfflerpPickedUpItemScale()
    {
        if (UI_Manager.draggedIcon.rectTransform != null)
        {
            float timer = 0;
            float duration = 0.15f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                UI_Manager.draggedIcon.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(165, 165), new Vector2(120, 120), timer / duration);
                yield return null;
            }
            UI_Manager.draggedIcon.rectTransform.sizeDelta = new Vector2(120, 120);
            DestroySlotHighlight();
            Destroy(UI_Manager.draggedIcon.gameObject);
            UI_Manager.draggedIcon = null;
            GameManager.instance.ui_Manager.RefreshAll();
        }
        else
            print("Erhmmmm.. something went very wrong 2");
    }
}
