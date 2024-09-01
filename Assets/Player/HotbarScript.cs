using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HotbarScript : MonoBehaviour
{
    //public GameObject Bow;
    //public GameObject Sword;
    [SerializeField]
    private int selectedSlot = 1;
    private PlayerMovement playermovement;
    public bool canChangeSlot = true;

    public GameObject[] Weaponds = new GameObject[9];

    private void Start()
    {
        playermovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        canChangeSlot = playermovement.CanMove; //baseing the bool on if the player is allowed to move

        //slot 1 or 1key is Bow
        if (Input.GetKey(KeyCode.Alpha1) && selectedSlot != 1 && canChangeSlot) //Bow
        {
            selectedSlot = 1;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
                InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = true;
        }
        if (Input.GetKey(KeyCode.Alpha2) && selectedSlot != 2 && canChangeSlot) //Sword
        {
            selectedSlot = 2;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
                InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = false;
        }
        if (Input.GetKey(KeyCode.Alpha3) && selectedSlot != 3 && canChangeSlot) //Empty
        {
            selectedSlot = 3;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
            InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = false;
        }
        if (Input.GetKey(KeyCode.Alpha4) && selectedSlot != 4 && canChangeSlot) //Empty
        {
            selectedSlot = 4;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
                InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = false;
        }
        if (Input.GetKey(KeyCode.Alpha5) && selectedSlot != 5 && canChangeSlot) //Empty
        {
            selectedSlot = 5;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
                InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = false;
        }
        if (Input.GetKey(KeyCode.Alpha6) && selectedSlot != 6 && canChangeSlot) //Empty
        {
            selectedSlot = 6;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
                InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = false;
        }
        if (Input.GetKey(KeyCode.Alpha7) && selectedSlot != 7 && canChangeSlot) //Empty
        {
            selectedSlot = 7;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
                InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = false;
        }
        if (Input.GetKey(KeyCode.Alpha8) && selectedSlot != 8 && canChangeSlot) //Empty
        {
            selectedSlot = 8;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
                InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = false;
        }
        if (Input.GetKey(KeyCode.Alpha9) && selectedSlot != 9 && canChangeSlot) //Empty
        {
            selectedSlot = 9;
            destroyCurrentWeapond();

            if (Weaponds[selectedSlot] != null)
                InstantiateNewWeapond(Weaponds[selectedSlot - 1]);
            playermovement.useMousePos = false;
        }
    }

    public void destroyCurrentWeapond()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("__Weapond__"))
            {
                Destroy(obj);
                break;
            }
        }
    }
    
    private void InstantiateNewWeapond(GameObject Weapond)
    {
            Instantiate(Weapond, transform);
    }
}
