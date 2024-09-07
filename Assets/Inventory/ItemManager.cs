using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemManager : MonoBehaviour
{
    public List<AssetReference> items;

    [SerializeField]
    public Dictionary<string, Item> nameToItemDict = 
       new Dictionary<string, Item>();

    private void Awake()
    {
        foreach (AssetReference itemRef in items)
        {
            LoadItem(itemRef);
        }
    }

    private void LoadItem(AssetReference itemRef)
    {
        itemRef.LoadAssetAsync<GameObject>().Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject itemObj = handle.Result;
                Item loadedItem = itemObj.GetComponentInChildren<Item>(); // Get the Item component from the loaded GameObject

                if (loadedItem != null && !nameToItemDict.ContainsKey(loadedItem.data.itemName))
                {
                    nameToItemDict.Add(loadedItem.data.itemName, loadedItem);
                }
                else
                    print("LoadFailed");
            }
            else
            {
                Debug.LogError("Failed to load item from addressables.");
            }
        };
    }

    private void AddItem(Item item)
    {
        if (nameToItemDict.ContainsKey(item.data.itemName))
        {
            nameToItemDict.Add(item.data.itemName, item);
        }
    }

    public Item GetItemByName(string key)
    {
        if (nameToItemDict.ContainsKey(key))
        {
            return nameToItemDict[key];
        }
        print(nameToItemDict.ContainsKey(key));
        Debug.Log("Items loaded: " + nameToItemDict.Count);
        return null;
    }
    private void OnDestroy() //some shit needed for referencing
    {
        // Release loaded assets
        foreach (var itemRef in items)
        {
            itemRef.ReleaseAsset();
        }
    }
}
