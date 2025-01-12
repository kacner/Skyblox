using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TextArray : MonoBehaviour
{
    [Header("Text Array Settings")]
    [SerializeField] private GameObject textPrefab; 
    [SerializeField] private GameObject textPrefab2; 
    [SerializeField] private Transform initialSpawnPoint;
    [SerializeField] private Transform EndPoint;
    [SerializeField] private float spawnOffset = 7.5f;

    private RectTransform lastSpawnedText;
    public List<Quest> AllQuests;

    int currentItteration = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnText();
        }
    }
    void SpawnText()
    {
        // Determine spawn pos
        Vector3 instantiatePos;

        if (currentItteration >= AllQuests.Count)
        {
            Debug.LogWarning("No more quests to display.");
            return;
        }
        if (lastSpawnedText == null)
        {
            instantiatePos = initialSpawnPoint.position;
        }
        else
        {
            instantiatePos = CalculateNextPos(lastSpawnedText);
        }

        if (instantiatePos.y < EndPoint.position.y)
        {
            Debug.LogWarning("Cannot spawn more text objects: position below EndPoint.");
            return;
        }

        GameObject newTextObject = Instantiate(textPrefab, transform);

        RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
        rectTransform.position = instantiatePos;

        Quest currentQuest = AllQuests[currentItteration];
        if (currentQuest != null)
        {
            rectTransform.GetComponentInChildren<TextMeshProUGUI>().text = currentQuest.Description;
        }
        else
        {
            Debug.LogWarning($"Quest at index {currentItteration} is null.");
        }

        lastSpawnedText = rectTransform;

        currentItteration++;
    }
    private float GetRenderedTextHeight(TextMeshProUGUI tmp)
    {
        if (tmp == null) return 0;

        tmp.ForceMeshUpdate();

        int lineCount = tmp.textInfo.lineCount;

        float fontSize = tmp.fontSize;
        float lineSpacing = tmp.lineSpacing;

        float renderedHeight = (fontSize + (lineSpacing * fontSize / 100)) * lineCount;

        return renderedHeight * tmp.rectTransform.lossyScale.y;
    }

    private Vector3 CalculateNextPos(RectTransform previousText)
    {
        TextMeshProUGUI tmp = previousText.GetComponentInChildren<TextMeshProUGUI>();
        float previousTextHeight = GetRenderedTextHeight(tmp);

        return previousText.position + new Vector3(0, -(previousTextHeight + spawnOffset), 0);
    }
    private void OnValidate()
    {
        UpdateGizmos();
    }

    private void UpdateGizmos()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.SceneView.RepaintAll();
        }
    }

    private void OnDrawGizmos()
    {
        if (initialSpawnPoint == null || EndPoint == null)
            return;

        Gizmos.color = Color.green;

        if (lastSpawnedText != null)
        {
            TextMeshProUGUI tmp = lastSpawnedText.GetComponentInChildren<TextMeshProUGUI>();
            float currentHeight = GetRenderedTextHeight(tmp);
            Gizmos.DrawWireCube(
                lastSpawnedText.position,
                new Vector3(lastSpawnedText.rect.width, currentHeight, 0)
            );
        }

        Vector3 nextPosition;
        float nextHeight;
        if (lastSpawnedText != null)
        {
            nextPosition = CalculateNextPos(lastSpawnedText);
            TextMeshProUGUI tmp = lastSpawnedText.GetComponentInChildren<TextMeshProUGUI>();
            nextHeight = GetRenderedTextHeight(tmp);
        }
        else
        {
            nextPosition = initialSpawnPoint.position;
            nextHeight = 1f;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(nextPosition, new Vector3(1, nextHeight, 0)); 

        Gizmos.color = Color.green;
        if (lastSpawnedText != null)
        {
            Vector3 offsetStart = lastSpawnedText.position;
            Vector3 offsetEnd = offsetStart + new Vector3(0, -spawnOffset, 0);
            Gizmos.DrawLine(offsetStart, offsetEnd);
        }
    }
    public void AddQuestToDo()
    {
        SpawnText();
    }
}
