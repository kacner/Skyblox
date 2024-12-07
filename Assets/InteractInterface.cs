using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractInterface : MonoBehaviour
{
    [SerializeField] private Image Button;
    [SerializeField] private Sprite Unclicked;
    [SerializeField] private Sprite clicked;
    private AdvancedNPCInteract advancedNpcInteract;
    private int buttonPressed;
    public void GotoNextDialouge()
    {
        advancedNpcInteract = GameManager.instance.ui_Manager.lastknownInteractScript;
        if (!advancedNpcInteract.HasChoise && buttonPressed == 1)
        {
            if (advancedNpcInteract.hasFinishedTypeOut)
            {
                advancedNpcInteract.nextdialouge();
            }
            else
            {
                advancedNpcInteract.wantToSkip = true;
            }
        }
        else
        {

        }
    }

    public void Button1()
    {
        buttonPressed = 1;
    }
    public void Button2()
    {
        buttonPressed = 2;
    }
    public void Button3()
    {
        buttonPressed = 3;
    }
}
