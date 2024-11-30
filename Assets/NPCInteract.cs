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
    [SerializeField] private AnimationState[] Emotion;
    [SerializeField] private int CurrentDialouge = 0;
    [SerializeField] private float CurrentDialougeTime = 0;
    private ChatBubbel currentChatBubble;

    public Animator E_Animator;

    private bool DistanceOverload = false;
    private bool hasSkipped = false;

    private Transform playerTransform;
    private CameraScript camerascript;

    public AnimationState currentState = AnimationState.Idle;
    private Animator animator;
    public enum AnimationState
    {
        Angry,
        Pointing,
        HandsOut,
        HandInPocket,
        Idle
    }
    private void Start()
    {
        CurrentInteractTime = MinKeyPressTime;
        Invoke("ConnectReferences", 0.25f);
        InvokeRepeating("UpdateDistance", 1, 0.3f);

        animator = GetComponent<Animator>();
    }

    void UpdateDistance()
    {
        if (CanInteractTimer < 0 && !DistanceOverload)
        {
            distance = Vector2.Distance(transform.position, playerTransform.position);
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

                if (Input.GetKey(KeyCode.E))
                {
                    interactButton.sprite = clicked;
                }
                else
                {
                    resetInteract();
                }

                if (Input.GetKeyUp(KeyCode.E))
                {
                    if (CurrentDialougeTime > 0 && hasSkipped)
                    {
                        CurrentDialougeTime = 0;
                        interactCooldownTimer = interactCooldown / 2;
                        hasSkipped = true;
                    }
                    else if (CurrentInteractTime < 0 && CanInteractTimer < 0)
                    {
                        CurrentInteractTime = MinKeyPressTime;
                        interactCooldownTimer = interactCooldown / 2;
                        PressInteract();
                    }
                }
            }
        }
    }

    void EnterRange()
    {
        isWithingDistance = true;
        interactButton.enabled = true;
    }

    private void ExitRange()
    {
        isWithingDistance = false;
        interactButton.enabled = false;

        resetInteract();

        StartCoroutine(camerascript.Zoom(3f, 320));

        camerascript.FollowingTarget = playerTransform;
        StartCoroutine(camerascript.ChangeFollowSpeedAfterTime(10f, 1f));
    }
    void PressInteract()
    {
        playDialouge();
        StartCoroutine(camerascript.ChangeFollowSpeedAfterTime(1f, 0.01f));
        StartCoroutine(camerascript.Zoom(3f, 212));
        camerascript.FollowingTarget = transform;
    }

    void resetInteract()
    {
        interactButton.sprite = unclicked;
    }
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
            ChangeState(Emotion[CurrentDialouge]);

            CurrentDialougeTime = DialougeArr[CurrentDialouge].Length * TalkSpeed + 3f;

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
        ChangeState(AnimationState.Idle);

        StartCoroutine(camerascript.ChangeFollowSpeedAfterTime(10f, 1f));
        StartCoroutine(camerascript.Zoom(3f, 320));
        camerascript.FollowingTarget = playerTransform;
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

    private void ConnectReferences()
    {
        playerTransform = GameManager.instance.player.transform;
        camerascript = GameManager.instance.camerScript;
    }







    public void ChangeState(AnimationState newState)
    {
        if (currentState == newState) return;

        ExitState(currentState);

        EnterState(newState);

        currentState = newState;
    }

    private void EnterState(AnimationState state)
    {
        switch (state)
        {
            case AnimationState.Idle:
                animator.Play("CaptainNPCIdle");
                break;
            case AnimationState.Pointing:
                animator.Play("CaptainPointing");
                break;
            case AnimationState.HandInPocket:
                animator.Play("CaptainHandInPocket");
                break;
            case AnimationState.HandsOut:
                animator.Play("CaptainHandsOut");
                break;
            case AnimationState.Angry:
                animator.Play("CaptainAngry");
                break;
        }
    }

    private void ExitState(AnimationState state)
    {
    }
}