using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarScript : MonoBehaviour
{
    public GameObject Bow;
    public GameObject Sword;
    [SerializeField]
    private int selectedSlot = 1;
    private PlayerMovement playermovement;

    private void Start()
    {
        playermovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        //slot 1 or 1key is Bow
        if (Input.GetKey(KeyCode.Alpha1) && selectedSlot != 1)
        {
            selectedSlot = 1;
            destroyCurrentWeapond();

            InstantiateNewWeapond(Bow);
            playermovement.useMousePos = true;
        }
        if (Input.GetKey(KeyCode.Alpha2) && selectedSlot != 2)
        {
            selectedSlot = 2;
            destroyCurrentWeapond();

            InstantiateNewWeapond(Sword);
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
