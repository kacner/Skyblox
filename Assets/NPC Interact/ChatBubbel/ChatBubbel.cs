using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class ChatBubbel : MonoBehaviour
{

    public SpriteRenderer BackgroundSpriteRenderer;
    public SpriteRenderer iconSpriterenderer;
    public TextMeshPro ChatTextTmp;
    public GameObject ChatBubblePrefab;
    public Vector2 padding = new Vector2(1f, 0.5f);

    public void Create(Transform parent, Vector3 localPosition, string text)
    {
        GameObject chatBubbleTransform = Instantiate(ChatBubblePrefab, parent);

        chatBubbleTransform.transform.localPosition = localPosition;

        chatBubbleTransform.GetComponent<ChatBubbel>().Setup(text);
    }

    private void Setup(string text)
    {
        ChatTextTmp.text = text;
        ChatTextTmp.ForceMeshUpdate();
        Vector2 textSize = ChatTextTmp.GetRenderedValues(false);

        BackgroundSpriteRenderer.size = textSize + padding;

        BackgroundSpriteRenderer.transform.localPosition = new Vector3(BackgroundSpriteRenderer.size.x / 2f, 0f);
    }
}
