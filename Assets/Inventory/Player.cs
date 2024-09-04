using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;

    private void Awake()
    {
        inventory = new Inventory(15);
    }

    public void dropItem(GameObject item, int slotID)
    {
        print("dropped");
        Vector2 spawnlocation = transform.position;

        Vector2 spawnOffset = Random.insideUnitCircle.normalized * 3.25f;

        GameObject droppedItem = Instantiate(item, spawnlocation + spawnOffset, Quaternion.identity);
        droppedItem.transform.localScale = new Vector3(1, 1, 1);

        RarityLevel InvRariry = inventory.GetRarityFromSlot(slotID - 1);

        droppedItem.GetComponentInChildren<Collectibal>().Rarity = InvRariry;

    }
}
