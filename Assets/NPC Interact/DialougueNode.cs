using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialougueNode
{
    public string DialougueText;
    public List<DialougueResponse> responses;
    [SerializeField] public AdvancedNPCInteract.AnimationState Emotion;

    internal bool IsLastNode()
    {
        return responses.Count <= 0;
    }
}
