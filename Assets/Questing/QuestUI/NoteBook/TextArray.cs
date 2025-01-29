using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextArray : MonoBehaviour
{
    [Header("Text Array Settings")]
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Transform initialSpawnPoint;
    [SerializeField] private Transform EndPoint;
    [SerializeField] private float spawnOffset = 7.5f;

    private RectTransform lastSpawnedText;
    public List<Quest> AllQuests;
    public List<GameObject> CurrentlySpawnedTextObjects;

    public List<List<GameObject>> pages = new List<List<GameObject>>();
    public int CurrentPage = 0;

    int currentItteration = 0;

    public Image NextButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnText();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextPage();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousPage();
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
            Debug.Log("End of page reached, creating new page.");
            CreateNewPage();
            instantiatePos = initialSpawnPoint.position;
        }

        GameObject newTextObject = Instantiate(textPrefab, transform);
        CurrentlySpawnedTextObjects.Add(newTextObject);

        RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
        rectTransform.position = instantiatePos;

        Quest currentQuest = AllQuests[currentItteration];
        if (currentQuest != null)
        {
            TextMeshProUGUI tmp = rectTransform.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = currentQuest.Goals[0].description + " - " + currentQuest.Npc.Name;
            tmp.ForceMeshUpdate();
        }

        lastSpawnedText = rectTransform;
        currentItteration++;


        /*Debug.Log($"Total pages: {pages.Count}");
        for (int i = 0; i < pages.Count; i++)
        {
            Debug.Log($"Page {i + 1} contains {pages[i].Count} items.");
        }*/
    }

    private float GetRenderedTextHeight(TextMeshProUGUI tmp)
    {
        if (tmp == null) return 0;

        tmp.ForceMeshUpdate();

        //int lineCount = tmp.textInfo.lineCount;
        int lineCount = 2;
        //int lineCount = 3;

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
    private void CreateNewPage()
    {
        if (CurrentlySpawnedTextObjects.Count > 0)
        {
            pages.Add(new List<GameObject>(CurrentlySpawnedTextObjects));
        }
        CurrentlySpawnedTextObjects.Clear();
        lastSpawnedText = null;
    }

    public void RedrawNoteBook()
    {
        foreach (GameObject item in CurrentlySpawnedTextObjects) //destroy text
        {
            Destroy(item);
        }
        CurrentlySpawnedTextObjects.Clear();
        lastSpawnedText = null;
        currentItteration = 0;

        while (currentItteration < AllQuests.Count) //Reconstruct text
        {
            SpawnText();
        }
    }
    private void DisplayPage(int pageIndex)
    {
        foreach (GameObject obj in CurrentlySpawnedTextObjects)
        {
            Destroy(obj);
        }

        CurrentlySpawnedTextObjects.Clear();

        foreach (GameObject obj in pages[pageIndex])
        {
            GameObject newObj = Instantiate(obj, transform);
            CurrentlySpawnedTextObjects.Add(newObj);
        }

        lastSpawnedText = CurrentlySpawnedTextObjects[^1].GetComponent<RectTransform>();

        if (CurrentPage < pages.Count - 1)
        {
            NextButton.enabled = true;
        }
        else
            NextButton.enabled = false;
    }
    public void NextPage()
    {
        print(pages.Count);
        if (CurrentPage + 1 < pages.Count)
        {
            print("nextpage");
            CurrentPage++;
            DisplayPage(CurrentPage);
        }
        else
        {
            Debug.LogWarning("Already on the last page.");
        }
    }

    public void PreviousPage()
    {
        if (CurrentPage > 0)
        {
            print("Prevouspage");
            CurrentPage--;
            DisplayPage(CurrentPage);
        }
        else
        {
            Debug.LogWarning("Already on the first page.");
        }
    }
    public static void RemoveEmptySlots<T>(ref List<T> list)
    {
        list = list.Where(item => item != null).ToList();
    }

    public void RemoveQuest(Quest QuestToRemove)
    {
        if (AllQuests.Contains(QuestToRemove))
        {
            AllQuests.Remove(QuestToRemove);

            RedrawNoteBook();
            RemoveEmptySlots(ref AllQuests);
            print("Didd everythings");
        }
        else
        {
            Debug.LogWarning("Quest not found in the list.");
        }
    }
}