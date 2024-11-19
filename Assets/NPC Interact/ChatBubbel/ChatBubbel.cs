using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class ChatBubbel : MonoBehaviour
{
    public static void Create(Transform parent, Vector3 localPosition, string text, float talkspeed)
    {
        Transform chatBubbleTransform = Instantiate(GameAssets.i.pfChatBubble, parent);
         
        chatBubbleTransform.localPosition = localPosition;

        chatBubbleTransform.GetComponent<ChatBubbel>().Setup(text, talkspeed, chatBubbleTransform);

    }

    public SpriteRenderer BackgroundSpriteRenderer;
    public SpriteRenderer iconSpriterenderer;
    public TextMeshPro ChatTextTmp;
    public Vector2 padding = new Vector2(1.2f, 0.5f);

    private void Setup(string text, float talkspeed, Transform chatBubbleTransform)
    {
        StartCoroutine(TypeEffect(text, talkspeed));
        StartCoroutine(SpawnAndDespawnAnim(chatBubbleTransform, talkspeed, text));
    }

    private IEnumerator TypeEffect(string text, float talkspeed)
    {
        ChatTextTmp.text = "";

        foreach (char letter in text)
        {
            ChatTextTmp.text += letter;

            ChatTextTmp.ForceMeshUpdate();
            Vector2 textSize = ChatTextTmp.GetRenderedValues(false);

            BackgroundSpriteRenderer.size = textSize + padding;

            BackgroundSpriteRenderer.transform.localPosition = new Vector3(BackgroundSpriteRenderer.size.x / 2f, 0f);

            yield return new WaitForSeconds(talkspeed);
        }
    }

    private IEnumerator SpawnAndDespawnAnim(Transform chatBubbleTransform, float talkspeed, string text)
    {
        yield return new WaitForSeconds(3f + talkspeed * text.Length);

        GetComponent<Animator>().SetTrigger("Despawn");

        yield return new WaitForSeconds(1f);

        Destroy(chatBubbleTransform.gameObject);
    }
}
