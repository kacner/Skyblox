using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class DialougueManager : MonoBehaviour
{
    private Dialougue Dialouge;
    public static DialougueManager Instance { get; private set; }

    public GameObject DialogueParent; 
    public TextMeshProUGUI DialogTitleText, DialogBodyText, SkipText; 
    public GameObject responseButtonPrefab; 
    public Transform responseButtonContainer;

    private bool hasFinnishedTypeOut = false;
    private bool wantToSkip = false;
    private List<GameObject> buttons;
    private AdvancedNPCInteract advancedNpc;
    private Coroutine typeEffect;

    private AdvancedNPCInteract LastKnownNpcReference;
    private Animator skipTextAnimator;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        buttons = new List<GameObject>();
    }

    private void Start()
    {
        GameManager.instance.ui_Manager?.exitState(); //same as hideDialouge

        skipTextAnimator = SkipText.GetComponent<Animator>();
    }

    public void StartDialogue(string title, DialougueNode node, Dialougue dialouge, AdvancedNPCInteract advancedNpc)
    {
        LastKnownNpcReference = advancedNpc;
        this.advancedNpc = advancedNpc;
        advancedNpc.ChangeState(node.Emotion);
        Dialouge = dialouge;

        /*if (node.nextNode.ApplyQuestTrigger && LastKnownNpcReference != null && LastKnownNpcReference.questType != "")
        {
            print(node.DialougueText);
            StopCoroutine(typeEffect);
            typeEffect = StartCoroutine(TypeEffect(LastKnownNpcReference.Quest.Description, dialouge.TalkSpeed));


            AtemptToAssignQuest();
        }
        else
            typeEffect = StartCoroutine(TypeEffect(node.DialougueText, dialouge.TalkSpeed));*/

        LastKnownNpcReference.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        skipTextAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        GameManager.instance.ui_Manager.ChangeState(UI_Manager.UIState.DialougeManager); // same as showdialouge

        DialogTitleText.text = title;

        if (!CheckForQuestOverride(node.DialougueText))
        {
            typeEffect = StartCoroutine(TypeEffect(node.DialougueText, dialouge.TalkSpeed));
        }
        else
            typeEffect = StartCoroutine(TypeEffect(ReturnQuestOverride(node.DialougueText), dialouge.TalkSpeed));

        


        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }

        buttons.Clear();

        foreach (DialougueResponse response in node.Responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, title, advancedNpc, node));
            buttons.Add(buttonObj);
        }
        hideButtons();
    }

    public void SelectResponse(DialougueResponse response, string title, AdvancedNPCInteract advancedNpc, DialougueNode node)
    {
        if (response.nextNode.ApplyQuestTrigger && LastKnownNpcReference != null && LastKnownNpcReference.questType != "")
        {

            AtemptToAssignQuest();
        }

        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(title, response.nextNode, Dialouge, advancedNpc);
        }
        else
        {
            LastKnownNpcReference.animator.updateMode = AnimatorUpdateMode.Normal;
            GameManager.instance.ui_Manager.exitState(); //same as hideDialouge
        }
    }

    public void HideDialogue()
    {
        DialogueParent.SetActive(false);
        if (typeEffect != null)
        StopCoroutine(typeEffect);

        if (advancedNpc != null)
        {
            advancedNpc.ChangeState(AdvancedNPCInteract.AnimationState.Idle);
            skipTextAnimator.updateMode = AnimatorUpdateMode.Normal;
            advancedNpc.totalReset();
        }
    }

    public void ShowDialogue()
    {
        DialogueParent.SetActive(true);
    }
    private void hideButtons()
    {
        foreach (GameObject button in buttons)
            button?.SetActive(false);
    }
    private void ShowButtons()
    {
        foreach (GameObject button in buttons)
            button?.SetActive(true);
    }
    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasFinnishedTypeOut)
            wantToSkip = true;
    }

    IEnumerator TypeEffect(string text, float talkspeed)
    {
        hasFinnishedTypeOut = false;
        DialogBodyText.text = "";
        bool shouldShowSkip = false;

        Animator skipTextAnimator = SkipText.GetComponent<Animator>();

        if ((text.Length * talkspeed) > 1.5f) //handle skip button
        {
            shouldShowSkip = true;
            SkipText.enabled = true;
            skipTextAnimator.SetTrigger("FadeIn");
        }

        DialogBodyText.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            char letter = text[i];
            DialogBodyText.text += letter;

            if (letter != ' ')
            {
                yield return new WaitForSecondsRealtime(talkspeed);
            }


            if (wantToSkip)
            {
                if (shouldShowSkip)
                skipTextAnimator.SetTrigger("FadeOut");

                DialogBodyText.text = text; 
                break;
            }

            if (shouldShowSkip && i >= text.Length * 0.6f)
            {
                skipTextAnimator.SetTrigger("FadeOut");
            }

            if (letter != ' ')
            {
                yield return new WaitForSecondsRealtime(talkspeed);
            }
        }

        wantToSkip = false;
        hasFinnishedTypeOut = true;

        if (shouldShowSkip)
        skipTextAnimator.SetTrigger("FadeOut");

        SkipText.enabled = false;

        if (buttons.Count > 0)
            ShowButtons();
    }

    private void AtemptToAssignQuest()
    {
        if (!LastKnownNpcReference.AssignedQuest && !LastKnownNpcReference.Healped)
        {
            AssignQuest();
        }
        else if (LastKnownNpcReference.AssignedQuest && !LastKnownNpcReference.Healped)
        {
            CheckQuest();
        }
        else
        {

        }
    }
    private void AssignQuest()
    {
        LastKnownNpcReference.Quest = (Quest)GameManager.instance.QuestObject.AddComponent(System.Type.GetType(LastKnownNpcReference.questType));
        LastKnownNpcReference.AssignedQuest = true;
    }

    private void CheckQuest()
    {
        if (LastKnownNpcReference.Quest.Completed)
        {
            LastKnownNpcReference.Quest.GiveReward();
            LastKnownNpcReference.Healped = true;
            LastKnownNpcReference.AssignedQuest = false;
            print("Yippie du gjorde det");
        }
        else
            print("du har inte gjort klart questen än");
    }

    private bool CheckForQuestOverride(string text)
    {
        if (text.Contains("{"))
            return true;
        else
            return false;
    }
    private string ReturnQuestOverride(string text)
    {
        AtemptToAssignQuest();

        if (text.Contains("{QuestName}"))
            text = text.Replace("{QuestName}", LastKnownNpcReference.Quest.QuestName);
        if (text.Contains("{QuestDescription}"))
            text = text.Replace("{QuestDescription}", LastKnownNpcReference.Quest.Description);

        return text;
    }
}