using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, InventoryUI> inventoryUIByName = new Dictionary<string, InventoryUI>();

    public GameObject inventorypanel;

    public List<InventoryUI> inventoryUIs;

    [SerializeField] public PlayerMovement playerMovement;

    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;

    public bool isInventoryToggeld = false;

    private void Awake()
    {
        getAllReferences();

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

    void getAllReferences()
    {
        /*GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Check if the object has the desired tag
            if (obj.CompareTag("ToggleInvPanel"))
            {
                inventorypanel = obj;
                print("Found [InventoryPanel]");
                break;
            }

            if (obj.CompareTag("Player"))
            {
                print("Found [Player]");
                playerMovement = obj.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                    print("Found [PlayerMovement]");
                else
                    print("failed to find [PlayerMovement]");
                break;
            }   
        }

        if (inventorypanel == null)
            print("Failed to find [InventoryPanel]");
        if (playerMovement == null)
            print("Failed to find [PlayerMovement]");*/

        GameObject obj = GameObject.FindWithTag("ToggleInvPanel");
        if (obj != null)
        {
            inventorypanel = obj;
            print("Found [InventoryPanel]");
            print(inventorypanel);
        }
        else
        {
            print("Failed to find [InventoryPanel]");
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                print("Found [PlayerMovement]");
                print(playerMovement);
            }
            else
                print("Failed to find [PlayerMovement]");
        }
        else
        {
            print("Failed to find [Player]");
        }
    }
}