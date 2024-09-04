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

    public void dropItem(GameObject item)
    {
        print("dropped");
        Vector3 spawnlocation = transform.position;

        float randX = Random.RandomRange(Random.RandomRange(-5, -2), Random.RandomRange(5, 2));
        float randY = Random.RandomRange(Random.RandomRange(-5,-2), Random.RandomRange(5, 2));

        Vector3 spawnOffset = new Vector3(randX, randY, 0).normalized;

        GameObject droppedItem = Instantiate(item, spawnlocation + spawnOffset, Quaternion.identity);
        droppedItem.transform.localScale = new Vector3(1, 1, 1);
    }
}
