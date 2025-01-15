using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCheckmark : MonoBehaviour
{
    public Image check;
    public Image cross;
    public void Check()
    {
        check.enabled = true;
        cross.enabled = false;
    }
    public void Cross()
    {
        cross.enabled = true;
        check.enabled = false;
    }
}
