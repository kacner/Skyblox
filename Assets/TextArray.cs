using TMPro;
using UnityEngine;

public class TextArray : MonoBehaviour
{
    [Header("Text Array Settings")]
    [SerializeField] private GameObject textPrefab; 
    [SerializeField] private int maxItems = 10; 
    [SerializeField] private Transform initialSpawnPoint;
    [SerializeField] private float spawnOffset = -50f;

    private RectTransform[] spawnedTexts;
    private int currentIndex = 0;

    void Start()
    {
        spawnedTexts = new RectTransform[maxItems];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnText();
        }
    }
    void SpawnText()
    {
        if (currentIndex >= maxItems)
        {
            Debug.LogWarning("Maximum number of items reached. Cannot spawn more text objects.");
            return;
        }

        GameObject newTextObject = Instantiate(textPrefab, transform);

        RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();

        if (currentIndex == 0)
        {
            rectTransform.position = initialSpawnPoint.position;
        }
        else
        {
            RectTransform previousText = spawnedTexts[currentIndex - 1];

            float previousTextHeight = GetRenderedTextHeight(previousText.GetComponentInChildren<TextMeshProUGUI>());
            Vector3 dynamicOffset = new Vector3(0, -GetRenderedTextHeight(previousText.GetComponentInChildren<TextMeshProUGUI>()) + spawnOffset, 0);

            rectTransform.position = previousText.position + dynamicOffset;
        }
        spawnedTexts[currentIndex] = rectTransform;
        currentIndex++;
    }
    private float GetRenderedTextHeight(TextMeshProUGUI tmp)
    {
        if (tmp == null) return 0;

        tmp.ForceMeshUpdate();
        return tmp.preferredHeight * tmp.rectTransform.lossyScale.y;
    }
}
