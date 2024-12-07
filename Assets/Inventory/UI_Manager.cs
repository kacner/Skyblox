using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, InventoryUI> inventoryUIByName = new Dictionary<string, InventoryUI>();

    public GameObject inventorypanel;

    public List<InventoryUI> inventoryUIs;

    public PlayerMovement playerMovement;

    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;

    public bool isInventoryToggeld = false;

    public GameObject InteractionMenu;
    public bool isinteractionmenuOpen = false;
    public TextMeshProUGUI interactionmenuDialougeTmp;
    [HideInInspector] public AdvancedNPCInteract lastknownInteractScript;
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
            closeInventory();
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
                playerMovement.AllCanAttack = false;
            }
            else
            {
                inventorypanel.SetActive(false);
                isInventoryToggeld = false;
                Cursor.visible = false;
                playerMovement.AllCanAttack = true;
            }
        }
    }

    public void closeInventory()
    {
        if (inventorypanel != null)
        {
            inventorypanel.SetActive(false);
            isInventoryToggeld = false;
            Cursor.visible = false;
            playerMovement.AllCanAttack = true;
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

    public void toggleInteractionMenu(AdvancedNPCInteract LastKnownInteractScript)
    {
        lastknownInteractScript = LastKnownInteractScript;
        if (isinteractionmenuOpen)
        {
            closeInteractionMenu();
            isinteractionmenuOpen = false;
        }
        else
        {
            OpenInteractionMenu();
            isinteractionmenuOpen = true;
        }
    }
    private void OpenInteractionMenu()
    {
        InteractionMenu.SetActive(true);
        isinteractionmenuOpen = true;
    }
    private void closeInteractionMenu()
    {
        InteractionMenu.SetActive(false);
        isinteractionmenuOpen = false;
    }
}