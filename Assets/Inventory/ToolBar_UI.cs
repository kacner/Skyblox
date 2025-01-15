using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> toolbarSlots = new List<Slot_UI>();

    [SerializeField] private Slot_UI selectedSlot;

    [SerializeField] private HotbarScript hotbarScript;

    public int selectedSlotNumber;

    public UI_Manager UI_manager;

    private void Start()
    {
        hotbarScript = GameManager.instance.player.GetComponent<HotbarScript>();
        UI_manager = GameManager.instance.ui_Manager;
        StartCoroutine(WaitForSeconds(0.1f));
    }

    private void Update()
    {
        checkAlphaNumericKeys();
    }

    public void selectSlot(int index)
    {
        if (toolbarSlots.Count == 9) //9 = toolbarslots
        {
            if (selectedSlot != null)
            {
                selectedSlot.SetHighLight(false);

            }
            selectedSlot = toolbarSlots[index];
            selectedSlot.SetHighLight(true);

            updateHotbarContent(index, hotbarScript.Weaponds);
        }
    }

    private void checkAlphaNumericKeys()
    {
        if (!UI_manager.isInventoryToggled)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectSlot(0);
                selectedSlotNumber = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectSlot(1);
                selectedSlotNumber = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectSlot(2);
                selectedSlotNumber = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectSlot(3);
                selectedSlotNumber = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                selectSlot(4);
                selectedSlotNumber = 4;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                selectSlot(5);
                selectedSlotNumber = 5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                selectSlot(6);
                selectedSlotNumber = 6;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                selectSlot(7);
                selectedSlotNumber = 7;
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                selectSlot(8);
                selectedSlotNumber = 8;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                int currentIndex = toolbarSlots.IndexOf(selectedSlot);
                if (currentIndex == -1)
                {
                    currentIndex = 0;
                }
                int newIndex = (currentIndex + (scroll > 0 ? 1 : -1) + toolbarSlots.Count) % toolbarSlots.Count;
                selectSlot(newIndex);
                selectedSlotNumber = newIndex;
            }
        }
        if (UI_manager == null)
        {
            print("UI_Manager = null");
            
        }
    }
    public void updateHotbarContent(int index, GameObject[] weapondsArray)
    {
        Inventory inventory = GameManager.instance.player.inventoryManager.GetInventoryByName("Toolbar");
        hotbarScript.UpdateWeapond(index, FindGameObjectContaining(inventory.FindItemNameInSlot(index).ToString(), weapondsArray));
    }

    GameObject FindGameObjectContaining(string WeapondPrefabName, GameObject[] array)
    {
        foreach (GameObject obj in array)
        {
            if (obj.name.Contains(WeapondPrefabName))
            {
                return obj;
            }
        }
        return null;
    }

    IEnumerator WaitForSeconds(float timer)
    {
        yield return new WaitForSeconds(timer);
        selectSlot(0);
        selectedSlotNumber = 0;
    }
}
