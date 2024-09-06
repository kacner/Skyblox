using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemManager : MonoBehaviour
{
    public AssetReference[] collectableItems;

    [SerializeField]
    public Dictionary<Collectabletype, GameObject> collectableItemsDict = new Dictionary<Collectabletype, GameObject>();

    private void Awake()
    {
        StartCoroutine(LoadItemsAsync());
    }

    private IEnumerator LoadItemsAsync()
    {
        foreach (var itemReference in collectableItems)
        {
            var handle = itemReference.LoadAssetAsync<GameObject>();

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AddItem(handle.Result);
            }
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
    private void OnDestroy()
    {
        foreach (var item in collectableItems)
        {
            item.ReleaseAsset();
        }
    }
}
