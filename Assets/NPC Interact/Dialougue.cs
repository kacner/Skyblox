using UnityEngine;
[CreateAssetMenu(fileName = "New Dialouhgue", menuName = "Dialougue/Dialougue Asset")]
public class Dialougue : ScriptableObject
{
    public DialougueNode RootNode;
    public float TalkSpeed = 0.05f;
}
