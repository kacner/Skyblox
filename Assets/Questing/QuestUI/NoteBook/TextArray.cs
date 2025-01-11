using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextArray : MonoBehaviour
{
    [Header("Text Array Settings")]
    [SerializeField] private GameObject textPrefab; 
    [SerializeField] private Transform initialSpawnPoint;
    [SerializeField] private Transform EndPoint;
    [SerializeField] private float spawnOffset = 7.5f;

    private RectTransform lastSpawnedText;
    private int currentIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnText();
        }
    }
    void SpawnText()
    {
        // Determine spawn position
        Vector3 instantiatePos;
        if (lastSpawnedText == null)
        {
            instantiatePos = initialSpawnPoint.position;
        }
        else
        {
            instantiatePos = CalculateNextPos(lastSpawnedText);
        }

        // Check if the text would spawn below the EndPoint
        if (instantiatePos.y < EndPoint.position.y)
        {
            Debug.LogWarning("Cannot spawn more text objects: position below EndPoint.");
            return;
        }

        // Instantiate the new text object
        GameObject newTextObject = Instantiate(textPrefab, transform);
        RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
        rectTransform.position = instantiatePos;

        // Store reference to the last spawned text
        lastSpawnedText = rectTransform;
    }
    private float GetRenderedTextHeight(TextMeshProUGUI tmp)
    {
        if (tmp == null) return 0;

        tmp.ForceMeshUpdate();
        return tmp.preferredHeight * tmp.rectTransform.lossyScale.y;
    }

    private Vector3 CalculateNextPos(RectTransform previousText)
    {
        float previousTextHeight = GetRenderedTextHeight(previousText.GetComponentInChildren<TextMeshProUGUI>());
        return previousText.position + new Vector3(0, -(previousTextHeight + spawnOffset), 0);
    }
}
