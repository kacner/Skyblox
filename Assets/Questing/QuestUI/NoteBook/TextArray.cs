using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
public class TextArray : MonoBehaviour
{
    [Header("Text Array Settings")]
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Transform initialSpawnPoint;
    [SerializeField] private Transform EndPoint;
    [SerializeField] private float spawnOffset = 7.5f;

    private RectTransform lastSpawnedText;
    public List<Quest> AllQuests;
    public List<GameObject> SpawnedTextObjects;

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
        SpawnedTextObjects.Add(newTextObject);

        RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
        rectTransform.position = instantiatePos;

        Quest currentQuest = AllQuests[currentItteration];
        if (currentQuest != null)
        {
            TextMeshProUGUI tmp = rectTransform.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = currentQuest.Description;

            tmp.ForceMeshUpdate();
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

        //int lineCount = tmp.textInfo.lineCount;
        int lineCount = 1;

        float fontSize = tmp.fontSize;
        float lineSpacing = tmp.lineSpacing;

        float renderedHeight = (fontSize + (lineSpacing * fontSize / 100)) * lineCount;
        return renderedHeight * tmp.rectTransform.lossyScale.y;
    }

    private Vector3 CalculateNextPos(RectTransform previousText)
    {
        if (previousText == null)
        {
            Debug.LogWarning("Previous text is null during position calculation.");
            return initialSpawnPoint.position;
        }

        TextMeshProUGUI tmp = previousText.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogWarning("No TextMeshProUGUI found in the previous text object.");
            return previousText.position + new Vector3(0, -spawnOffset, 0);
        }

        float previousTextHeight = GetRenderedTextHeight(tmp);
        return previousText.position + new Vector3(0, -(previousTextHeight + spawnOffset), 0);
    }

    public void AddQuestToDo()
    {
        foreach (GameObject item in SpawnedTextObjects)
        {
            Destroy(item);
        }
        SpawnedTextObjects.Clear();
        lastSpawnedText = null;
        currentItteration = 0;

        ReconstructTexts();
    }

    private void ReconstructTexts()
    {
        while (currentItteration < AllQuests.Count)
        {
            SpawnText();
        }
    }
}