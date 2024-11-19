using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public SpriteRenderer interactButton;
    [SerializeField] private float distance;
    [SerializeField] private float InteractRange = 3f;
    public Sprite unclicked;
    public Sprite clicked;
    [SerializeField] private bool isWithingDistance = false;
    [SerializeField] private float InteractTime = 1f;
    [SerializeField] private float CurrentInteractTime;
    [SerializeField] private bool isInteracting = false;

    [SerializeField] private float TalkSpeed = 0.05f;


    private void Start()
    {
        CurrentInteractTime = InteractTime;
        InvokeRepeating("UpdateDistance", 1, 0.3f);
    }

    // Update is called once per frame
    void UpdateDistance()
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
    private void Update()
    {
        if (CurrentInteractTime > -1)
        CurrentInteractTime -= Time.deltaTime;

        if (isWithingDistance && Input.GetKeyDown(KeyCode.E) && CurrentInteractTime < 0)
        {
            PressInteract();
            CurrentInteractTime = InteractTime;
            switchInteractState();
        }
        else if (isWithingDistance && Input.GetKeyUp(KeyCode.E))
        {
            resetInteract();
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
        isInteracting = false;
    }

    void PressInteract()
    {
        interactButton.sprite = clicked;
        ChatBubbel.Create(transform, new Vector3(-2f, 1.62f), "Help me vith the suringe, vill ya?", TalkSpeed);
    }

    void resetInteract()
    {
        interactButton.sprite = unclicked;
    }

    void switchInteractState()
    {
        isInteracting = !isInteracting;
    }
}
