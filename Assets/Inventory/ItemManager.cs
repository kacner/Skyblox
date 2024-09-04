using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject[] collectableItems;

    public Dictionary<Collectabletype, GameObject> collectableItemsDict = new Dictionary<Collectabletype, GameObject>();

    private void Awake()
    {

        foreach(GameObject item in collectableItems)
        {
            AddItem(item);
        }
    }
    private void AddItem(GameObject item)
    {
        Collectibal collectibal = item.GetComponentInChildren<Collectibal>();
        if (collectibal != null && !collectableItemsDict.ContainsKey(collectibal.type))
        {
            collectableItemsDict.Add(collectibal.type, item);
        }
    }

    public GameObject GetItemByType(Collectabletype type)
    {
        if (collectableItemsDict.ContainsKey(type))
        {
            return collectableItemsDict[type];
        }
        return null;
    }
}
