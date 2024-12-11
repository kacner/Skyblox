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
    }

    public void StartDialogue(string title, DialougueNode node, Dialougue dialouge, AdvancedNPCInteract advancedNpc)
    {
        this.advancedNpc = advancedNpc;
        advancedNpc.ChangeState(node.Emotion);
        Dialouge = dialouge;


        GameManager.instance.ui_Manager.ChangeState(UI_Manager.UIState.DialougeManager); // same as showdialouge

        DialogTitleText.text = title;
        typeEffect = StartCoroutine(TypeEffect(node.DialougueText, dialouge.TalkSpeed));

        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }

        buttons.Clear();

        foreach (DialougueResponse response in node.responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, title, advancedNpc));
            buttons.Add(buttonObj);
        }
        hideButtons();
    }

    public void SelectResponse(DialougueResponse response, string title, AdvancedNPCInteract advancedNpc)
    {
        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(title, response.nextNode, Dialouge, advancedNpc);
        }
        else
        {
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

        if ((text.Length * talkspeed) > 1.5f)
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

            if (wantToSkip)
            {
                if (shouldShowSkip)
                skipTextAnimator.SetTrigger("FadeOut");

                DialogBodyText.text = text; 
                yield break;
            }

            if (shouldShowSkip && i >= text.Length * 0.6f)
            {
                skipTextAnimator.SetTrigger("FadeOut");
            }

            yield return new WaitForSeconds(talkspeed);

            
        }
        wantToSkip = false;
        hasFinnishedTypeOut = true;

        if (shouldShowSkip)
        skipTextAnimator.SetTrigger("FadeOut");

        SkipText.enabled = false;

        if (buttons.Count > 0)
            ShowButtons();
    }
}