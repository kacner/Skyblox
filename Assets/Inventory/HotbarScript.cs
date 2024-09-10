using UnityEngine;

public class HotbarScript : MonoBehaviour
{
    private PlayerMovement playermovement;
    public bool canChangeSlot = true;

    public GameObject EmptyWHands;

    public GameObject[] Weaponds;
    private void Start()
    {
        playermovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        canChangeSlot = playermovement.CanMove; //baseing the bool on if the player is allowed to move
    }

    public void UpdateWeapond(int selectedSlot, GameObject wepond)
    {
        if (canChangeSlot)
        {
            destroyCurrentWeapond();

            if (wepond != null)
                InstantiateNewWeapond(wepond);
            else
                InstantiateNewWeapond(EmptyWHands);

            GameManager.instance.ui_Manager.RefreshInventoryUI("Backpack");
            GameManager.instance.ui_Manager.RefreshInventoryUI("Toolbar");
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
