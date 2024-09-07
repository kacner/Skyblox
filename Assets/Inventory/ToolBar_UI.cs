using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> toolbarSlots = new List<Slot_UI>();

    private Slot_UI selectedSlot;

    private void Start()
    {
        selectSlot(0);
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
            Debug.Log("selected slot : " + selectedSlot.name);
        }
    }

    private void checkAlphaNumericKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectSlot(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectSlot(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectSlot(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectSlot(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectSlot(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            selectSlot(8);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            int currentIndex = toolbarSlots.IndexOf(selectedSlot);
            if (currentIndex == -1)
            {
                currentIndex = 0; // Fallback if no slot is selected
            }
            int newIndex = (currentIndex + (scroll > 0 ? 1 : -1) + toolbarSlots.Count) % toolbarSlots.Count;
            selectSlot(newIndex);
        }
    }
}
