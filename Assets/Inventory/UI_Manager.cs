using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, InventoryUI> inventoryUIByName = new Dictionary<string, InventoryUI>();

    public GameObject inventorypanel;

    public List<InventoryUI> inventoryUIs;

    public PlayerMovement playerMovement;

    [Space]

    [Header("Inventories")]
    public GameObject PauseMenu;
    public GameObject QuestMenu;

    [Space]

    public GameObject CursorSprite;

    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;

    public bool isInventoryToggled = false;
    public bool isDialougeWindowToggled = false;

    private List<GameObject> OtherUi;

    public enum UIState
    {
        Inventory,
        DialougeManager,
        None,
        PauseMenu
    }
    public UIState currentState = UIState.None;
    private void Awake()
    {
        Initialize();
        if (inventorypanel != null)
            inventorypanel.SetActive(false);

        ChangeState(UIState.None);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) //multiple draging in inventory
        {
            dragSingle = false;
        }
        else
        {
            dragSingle = true;
        }

        if (currentState == UIState.None && Input.GetKeyDown(KeyCode.Tab)) //opening inventory
        {
            ChangeState(UIState.Inventory);
        }
        else if (currentState == UIState.Inventory && Input.GetKeyDown(KeyCode.Tab)) //closinginventory
        {
            ChangeState(UIState.None);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //pausemenu or clousemenu
        {
            if (currentState != UIState.None)
            {
                exitState();
            }
            else
            {
                ChangeState(currentState == UIState.PauseMenu ? UIState.None : UIState.PauseMenu);
            }
        }
    }

    public void closeInventory()
    {
        if (inventorypanel != null)
        {
            inventorypanel.SetActive(false);
            Cursor.visible = false;

        }
    }
    public void openInventory()
    {
        if (inventorypanel != null)
        {
            inventorypanel.SetActive(true);
            Cursor.visible = true;
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
        foreach (KeyValuePair<string, InventoryUI> keyValuePair in inventoryUIByName)
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
        foreach (InventoryUI ui in inventoryUIs)
        {
            if (!inventoryUIByName.ContainsKey(ui.inventoryName))
            {
                inventoryUIByName.Add(ui.inventoryName, ui);
            }
        }
    }

    public void ChangeState(UIState newState)
    {
        DeactivateCurrentState();

        currentState = newState;

        ActivateNewState();
    }

    private void DeactivateCurrentState()
    {
        switch (currentState)
        {
            case UIState.Inventory:
                CursorSprite.SetActive(true);
                inventoryUIs[0].slotEndDrag();
                closeInventory();
                break;

            case UIState.PauseMenu:
                PauseMenu.SetActive(false);
                CursorSprite.SetActive(true);
                Time.timeScale = 1;
                break;

            case UIState.DialougeManager:
                GameManager.instance.DialougeManager.HideDialogue();
                CursorSprite.SetActive(true);
                Time.timeScale = 1;
                break;

            case UIState.None:
                //this dosent have to do anything
                break;
        }
    }
    private void ActivateNewState()
    {
        switch (currentState)
        {
            case UIState.Inventory:
                openInventory();
                CursorSprite.SetActive(false);
                Cursor.visible = true;
                break;

            case UIState.PauseMenu:
                PauseMenu.SetActive(true);
                CursorSprite.SetActive(false);
                Cursor.visible = true;
                Time.timeScale = 0;
                break;

            case UIState.DialougeManager:
                GameManager.instance.DialougeManager.ShowDialogue();
                CursorSprite.SetActive(false);
                Cursor.visible = true;
                Time.timeScale = 0;
                break;

            case UIState.None:
                DisableAllUI();
                break;
        }
    }
    private void DisableAllUI() //call exitstate insted
    {
        closeInventory();
        GameManager.instance.DialougeManager.HideDialogue();
        PauseMenu.SetActive(false);
        Cursor.visible = false;

        CursorSprite.SetActive(true);
        inventoryUIs[0].slotEndDrag();
        QuestMenu.SetActive(false);
    }

    public void exitState()
    {
        ChangeState(UIState.None);
        Cursor.visible = false;
    }

    public void TryOpenQuestMenu()
    {
        if (currentState == UIState.Inventory)
        {
            QuestMenu.SetActive(true);
        }
    }

    public void TryClouseQuestMenu()
    {
        if (QuestMenu.activeSelf)
            QuestMenu.SetActive(false);
    }
}