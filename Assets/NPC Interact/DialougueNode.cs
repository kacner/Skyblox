using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialougueNode
{
    public string DialougueText;
    public List<DialougueResponse> Responses;
    [SerializeField] public AdvancedNPCInteract.AnimationState Emotion;
    public bool ApplyQuestTrigger = false;

    internal bool IsLastNode()
    {
        return Responses.Count <= 0;
    }
}
