using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    public List<LootTableItem> lootTableItems;

     public void GenerateDrop()
     {
        float totalWeight = 0f;
        foreach (var item in lootTableItems)
        {
            totalWeight += item.Droppchanse;
        }

        // Generate a random number between 0 and the total weight
        float randomValue = Random.Range(0f, totalWeight);

        // Determine which item corresponds to the random value
        float CorrespondingWeight = 0f;
        foreach (LootTableItem lootTableItem in lootTableItems)
        {
            CorrespondingWeight += lootTableItem.Droppchanse;
            if (randomValue <= CorrespondingWeight)
            {
                // Drop the selected item);
                if (lootTableItem != null)
                {
                    for (int i = 0; i < lootTableItem.Amount; i++)
                    {
                        GameManager.instance.player.dropItem(lootTableItem.Item.GetComponentInChildren<Item>(), transform.position, 0.5f);
                    }
                }
                else
                {
                    Debug.LogError($"Item '{lootTableItem.Item}' couldent find item.");
                }
                return; // Only one drop is allowed, so exit after dropping
            }
        }

        Debug.LogError("No item was selected. Check drop chances and weights.");
    }
}
[System.Serializable]
public class LootTableItem
{
    public GameObject Item;
    public float Droppchanse;
    public int Amount = 1;
}