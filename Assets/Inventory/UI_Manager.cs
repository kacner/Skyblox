using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, InventoryUI> inventoryUIByName = new Dictionary<string, InventoryUI>();

    public GameObject inventorypanel;

    public List<InventoryUI> inventoryUIs;

    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;

    public static bool isInventoryToggeld = false;

    private void Awake()
    {
        Initialize();
        if (inventorypanel != null)
            inventorypanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            dragSingle = false;
        }
        else
        {
            dragSingle = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventorypanel != null)
                inventorypanel.SetActive(false);
        }
    }
    public void ToggleInventoryUI()
    {
        if (inventorypanel != null)
        {

            if (!inventorypanel.activeSelf)
            {
                inventorypanel.SetActive(true);
                RefreshInventoryUI("Backpack");
                isInventoryToggeld = true;
                Cursor.visible = true;
            }
            else
            {
                inventorypanel.SetActive(false);
                isInventoryToggeld = false;
                Cursor.visible = false;
            }
        }
    }

    public void RefreshInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            inventoryUIByName[inventoryName].Refresh();
        }
    }

    public void RefreshAll()
    {
        foreach(KeyValuePair<string, InventoryUI> keyValuePair in inventoryUIByName)
        {
            keyValuePair.Value.Refresh();
        }
    }

    public InventoryUI GetInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            return inventoryUIByName[inventoryName];
        }
        return null;
    }

    void Initialize()
    {
        foreach(InventoryUI ui in inventoryUIs)
        {
            if(!inventoryUIByName.ContainsKey(ui.inventoryName))
            {
                inventoryUIByName.Add(ui.inventoryName, ui);
            }
        }
    }
}
