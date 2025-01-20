using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

public class AdvancedNPCInteract : MonoBehaviour
{
    public Dialougue Dialouge;
    public string Name;

    [Space(10)]

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
    [SerializeField] private bool isInteracting = false;

    [HideInInspector] public Animator E_Animator;

    private bool DistanceOverload = false;

    public AnimationState currentState = AnimationState.Idle;
    [HideInInspector] public Animator animator;
    private UI_Manager UI_manager;
    [HideInInspector] public bool hasFinishedTypeOut = false;
    [HideInInspector] public bool wantToSkip = false;
    private Interactable interactable;

    [Header("Quests")]
    public string questType;
    public bool AssignedQuest = false, Healped = false;
    public Quest Quest;

    [System.Serializable]
    public enum AnimationState
    {
        Angry,
        Pointing,
        HandsOut,
        HandInPocket,
        Idle
    }
    public void totalReset()
    {
        interactCooldownTimer = interactCooldown / 2;
        CurrentInteractTime = MinKeyPressTime;
    }
    private void Start()
    {
        interactable = GetComponent<Interactable>();
        CurrentInteractTime = MinKeyPressTime;
        InvokeRepeating("UpdateDistance", 1, 0.3f);

        animator = GetComponent<Animator>();
        UI_manager = GameManager.instance.ui_Manager;
    }

    void UpdateDistance()
    {
        if (CanInteractTimer < 0 && !DistanceOverload && !isInteracting && UI_manager.currentState == UI_Manager.UIState.None && GameManager.instance.InteractionManager.ClosestObject == interactable)
        {
            if (interactable.IsWithingRange)
            {
                EnterRange();
            }
            else
            {
                ExitRange();
                ChangeState(AnimationState.Idle);
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




        if (canInteract && GameManager.instance.ui_Manager.currentState == UI_Manager.UIState.None)
        {

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
                    if (CurrentInteractTime < 0 && CanInteractTimer < 0)
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
        isInteracting = false;
        resetInteract();
    }
    void PressInteract()
    {
        playDialouge();
    }

    void resetInteract()
    {
        interactButton.sprite = unclicked;
    }
    void playDialouge()
    {
        DialougueManager.Instance.StartDialogue(Name, Dialouge.RootNode, Dialouge, this);
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

    public void ChangeState(AnimationState newState)
    {
        if (currentState == newState) return;

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
}