using UnityEngine;

public class HotbarScript : MonoBehaviour
{
    private PlayerMovement playermovement;
    public bool canChangeSlot = true;

    public GameObject EmptyWHands;

    public GameObject[] Weaponds;

    private PlayerHp playerHp;
    private void Start()
    {
        playerHp = GetComponent<PlayerHp>();
        playermovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        canChangeSlot = playermovement.CanMove; //baseing the bool on if the player is allowed to move
    }

    public void UpdateWeapond(int selectedSlot, GameObject newWeapond)
    {
        if (canChangeSlot)
        {
            GameObject currentWeapond = GetCurrentWeapond();

            if (currentWeapond != null && AreWeaponsSame(currentWeapond, newWeapond))
            {
                return;
            }

            destroyCurrentWeapond();

            if (newWeapond != null)
                InstantiateNewWeapond(newWeapond);
            else
            {
                InstantiateNewWeapond(EmptyWHands);
                print("Diddy0");
                playerHp.changeHandMat();
            }

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

    private GameObject GetCurrentWeapond()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("__Weapond__"))
            {
                return obj;
            }
        }
        return null;
    }

    private void InstantiateNewWeapond(GameObject Weapond)
    {
        Instantiate(Weapond, transform);
        playerHp.changeHandMat();
    }

    private bool AreWeaponsSame(GameObject currentWeapond, GameObject newWeapond)
    {
        if (newWeapond == null || currentWeapond == null)
            return false;

        string currentWeapondName = currentWeapond.name;
        if (currentWeapondName.EndsWith("(Clone)"))
        {
            currentWeapondName = currentWeapondName.Substring(0, currentWeapondName.Length - 7); // Remove "(Clone)" which is 7 characters long
        }

        return currentWeapondName == newWeapond.name;
    }
}
