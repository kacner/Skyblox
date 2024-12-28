using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    public void dropItem(Item item, Vector2? spawnlocation = null, float SpawnOffset = 1.5f)
    {
        Vector2 spawnOffset = Random.insideUnitCircle.normalized * SpawnOffset;

        Vector2 finalSpawnLocation = spawnlocation ?? (Vector2)transform.position;

        Item droppedItem = Instantiate(item, finalSpawnLocation + spawnOffset, Quaternion.identity);
        //droppedItem.transform.parent = item.transform;
        droppedItem.transform.localScale = new Vector3(1, 1, 1);

    }
    public void dropItem(Item item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            dropItem(item);
        }
    }
}