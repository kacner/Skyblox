using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferencingScript : MonoBehaviour
{
    public GameObject inventorypanel;

    public List<InventoryUI> inventoryUIs;

    public PlayerMovement playerMovement;

    private void Start()
    {
        GameManager.instance.ui_Manager.inventorypanel = inventorypanel;
        GameManager.instance.ui_Manager.inventoryUIs = inventoryUIs;
        GameManager.instance.ui_Manager.playerMovement = playerMovement;
    }

}
