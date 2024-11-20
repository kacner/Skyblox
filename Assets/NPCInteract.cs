using System.Collections;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    [Header("InteractionSettings")]
    public bool canInteract = true;
    public SpriteRenderer interactButton;
    [SerializeField] private float distance;
    [SerializeField] private float InteractRange = 3f;
    public Sprite unclicked;
    public Sprite clicked;
    [SerializeField] private float MinKeyPressTime = 1f;
    [SerializeField] private float CurrentInteractTime;
    [SerializeField] private bool isWithingDistance = false;
    [SerializeField] private float CanInteractTimer = 0f;
    [SerializeField] private float interactCooldown = 0.5f;
    private float interactCooldownTimer = 0f;

    [Space(10)]

    [Header("Writing Settings")]
    [SerializeField] private float TalkSpeed = 0.05f;
    
    [Space(10)]

    [Header("Dialouge")]
    [SerializeField] private string[] DialougeArr;
    [SerializeField] private int CurrentDialouge = 0;
    [SerializeField] private float CurrentDialougeTime = 0;
    private ChatBubbel currentChatBubble;


    public Animator E_Animator;

    private bool DistanceOverload = false;
    private bool hasSkipped = false;

    private void Start()
    {
        CurrentInteractTime = MinKeyPressTime;
        InvokeRepeating("UpdateDistance", 1, 0.3f);
    }

    void UpdateDistance()
    {
        if (CanInteractTimer < 0 && !DistanceOverload)
        {
            distance = Vector2.Distance(transform.position, GameManager.instance.player.transform.position);
            if (distance < InteractRange)
            {
                EnterRange();
            }
            else
            {
                ExitRange();
            }
        }
        else
        {
            ExitRange();
        }
    }
    private void Update()
    {
        if (interactCooldownTimer > 0)
            interactCooldownTimer -= Time.deltaTime;

        if (CanInteractTimer > 0)
            CanInteractTimer -= Time.deltaTime;


        if (interactCooldownTimer > 0)
            canInteract = false;
        else
            canInteract = true;


        if (canInteract)
        {
            if (CurrentDialougeTime > -1)
                CurrentDialougeTime -= Time.deltaTime;

            if (CurrentInteractTime > -1)
                CurrentInteractTime -= Time.deltaTime;

            if (isWithingDistance && interactCooldownTimer <= 0)
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (CurrentDialougeTime > 0 && hasSkipped)
                    {
                        CurrentDialougeTime = 0;
                        interactCooldownTimer = interactCooldown / 2;
                        hasSkipped = true;
                    }
                    else if (CurrentInteractTime < 0 && CanInteractTimer < 0)
                    {
                        PressInteract();
                        CurrentInteractTime = MinKeyPressTime;
                        interactCooldownTimer = interactCooldown / 2;
                    }
                }
                else
                {
                    resetInteract();
                }
            }
        }
    }

    void EnterRange()
    {
        isWithingDistance = true;
        interactButton.enabled = true;
    }

    void ExitRange()
    {
        isWithingDistance = false;
        interactButton.enabled = false;

        resetInteract();
    }

    void PressInteract()
    {
        interactButton.sprite = clicked;
        playDialouge();
    }

    void resetInteract()
    {
        interactButton.sprite = unclicked;
    }

    /*void playDialouge()
    {
        if (CurrentDialouge == DialougeArr.Length - 1) //lastDialougeDetection
        {
            Debug.Log("This is the last dialogue!");
            StartCoroutine(Disable_EforTime(TalkSpeed * DialougeArr[CurrentDialouge].Length + 3.5f));
        }

        if (CurrentDialouge < DialougeArr.Length)
        {
            if (currentChatBubble != null) 
            {
                Destroy(currentChatBubble.gameObject);
            }

            currentChatBubble = ChatBubbel.Create(transform, new Vector3(-2f, 1.62f), DialougeArr[CurrentDialouge], TalkSpeed, interactButton.gameObject, this);

            CurrentDialougeTime = DialougeArr[CurrentDialouge].Length * TalkSpeed + 3f;

            CurrentDialouge++;
        }
        else
        {
            CurrentDialouge = 0;
            canInteract = false;
            CanInteractTimer = 5;
            interactCooldownTimer = interactCooldown;
        }
    }*/
    void playDialouge()
    {
        if (CurrentDialouge == DialougeArr.Length - 1) //lastDialougeDetection
        {
            Debug.Log("This is the last dialogue!");
            StartCoroutine(Disable_EforTime(TalkSpeed * DialougeArr[CurrentDialouge].Length + 3.5f));
        }

        if (currentChatBubble != null)
        {
            Destroy(currentChatBubble.gameObject);
        }

        if (CurrentDialouge < DialougeArr.Length)
        {
            currentChatBubble = ChatBubbel.Create(transform, new Vector3(-2f, 1.62f), DialougeArr[CurrentDialouge], TalkSpeed, interactButton.gameObject, this);

            CurrentDialougeTime = DialougeArr[CurrentDialouge].Length * TalkSpeed + 3f;

            // Check if it's the last dialogue
            if (CurrentDialouge == DialougeArr.Length - 1)
            {
                Debug.Log("This is the last dialogue!");
                StartCoroutine(HandleEndOfDialogue());
            }

            CurrentDialouge++;
        }
    }

    private IEnumerator HandleEndOfDialogue()
    {
        yield return new WaitForSeconds(DialougeArr[CurrentDialouge].Length * TalkSpeed + 3f); // Wait for dialogue to finish
        CurrentDialouge = 0;
        canInteract = false;
        CanInteractTimer = 5;
        interactCooldownTimer = interactCooldown;
    }

    public void DespawnInteractButton()
    {
        StartCoroutine(InteractionButtenAnimator());
    }

    private IEnumerator InteractionButtenAnimator()
    {
        E_Animator.SetTrigger("ButtonOff");
        yield return new WaitForSeconds(2f);
        E_Animator.SetTrigger("ButtonOn");
    }

    private IEnumerator Disable_EforTime(float time)
    {
        DistanceOverload = true;
        interactButton.enabled = false;
        yield return new WaitForSeconds(time);
        interactButton.enabled = true;
        DistanceOverload = false;
    }
}