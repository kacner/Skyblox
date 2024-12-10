using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialougueManager : MonoBehaviour
{
    private Dialougue Dialouge;
    public static DialougueManager Instance { get; private set; }

    // UI references
    public GameObject DialogueParent; // Main container for dialogue UI
    public TextMeshProUGUI DialogTitleText, DialogBodyText; // Text components for title and body
    public GameObject responseButtonPrefab; // Prefab for generating response buttons
    public Transform responseButtonContainer; // Container to hold response buttons

    private bool hasFinnishedTypeOut = false;
    private bool wantToSkip = false;
    private List<GameObject> buttons;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of DialogueManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        buttons = new List<GameObject>(); // Initialize the list
        // Initially hide the dialogue UI
        HideDialogue();
    }

    // Starts the dialogue with given title and dialogue node
    public void StartDialogue(string title, DialougueNode node, Dialougue dialouge, AdvancedNPCInteract advancedNpc)
    {
        advancedNpc.ChangeState(node.Emotion);
        Dialouge = dialouge;


        // Display the dialogue UI
        ShowDialogue(); 

        // Set dialogue title and body text
        DialogTitleText.text = title;
        StartCoroutine(TypeEffect(node.DialougueText, dialouge.TalkSpeed));

        // Remove any existing response buttons
        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }

        buttons.Clear();

        // Create and setup response buttons based on current dialogue node
        foreach (DialougueResponse response in node.responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;

            // Setup button to trigger SelectResponse when clicked
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, title, advancedNpc));
            buttons.Add(buttonObj);
        }
    }

    // Handles response selection and triggers next dialogue node
    public void SelectResponse(DialougueResponse response, string title, AdvancedNPCInteract advancedNpc)
    {
        // Check if there's a follow-up node
        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(title, response.nextNode, Dialouge, advancedNpc); // Start next dialogue
        }
        else
        {
            // If no follow-up node, end the dialogue
            HideDialogue();
        }
    }

    // Hide the dialogue UI
    public void HideDialogue()
    {
        DialogueParent.SetActive(false);
    }

    // Show the dialogue UI
    private void ShowDialogue()
    {
        DialogueParent.SetActive(true);
    }
    private void hideButtons()
    {
        foreach (GameObject button in buttons)
        {
            button?.SetActive(false);
            print("HIT");
        }
    }
    private void ShowButtons()
    {
        foreach (GameObject button in buttons)
        {
            button?.SetActive(true);
            print("Show");
        }
    }

    // Check if dialogue is currently active
    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasFinnishedTypeOut)
            wantToSkip = true;
    }
    private IEnumerator TypeEffect(string text, float talkspeed)
    {
        if (buttons.Count > 0)
            hideButtons();

        hasFinnishedTypeOut = false;
        DialogBodyText.text = "";

        foreach (char letter in text)
        {
            DialogBodyText.text += letter;

            yield return new WaitForSeconds(talkspeed);

            //if (buttons.Count > 0)
                //hideButtons();

            if (wantToSkip)
            {
                DialogBodyText.text = text;
                break;
            }
        }
        wantToSkip = false;
        hasFinnishedTypeOut = true;

        if (buttons.Count > 0)
            ShowButtons();
    }
}