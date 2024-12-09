using System.Collections.Generic;

[System.Serializable]
public class DialougueNode
{
    public string DialougueText;
    public List<DialougueResponse> responses;

    internal bool IsLastNode()
    {
        return responses.Count <= 0;
    }
}
