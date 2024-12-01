using System.Collections;
using TMPro;
using UnityEngine;

public class ChatBubbel : MonoBehaviour
{
    public static ChatBubbel Create(Transform parent, Vector3 localPosition, string text, float talkspeed, GameObject interactButton, NPCInteract npcInteract)
    {
        Transform chatBubbleTransform = Instantiate(GameAssets.i.pfChatBubble, parent);
        chatBubbleTransform.localPosition = localPosition;

        ChatBubbel chatBubble = chatBubbleTransform.GetComponent<ChatBubbel>();
        chatBubble.Setup(text, talkspeed, chatBubbleTransform, interactButton, npcInteract);

        return chatBubble;
    }

    public SpriteRenderer BackgroundSpriteRenderer;
    public SpriteRenderer iconSpriterenderer;
    public TextMeshPro ChatTextTmp;
    public Vector2 padding = new Vector2(1.2f, 0.5f);
    private Coroutine typingCoroutine;
    public Vector3 rightOffset = new Vector3(0.5f, 0f, 0f);
    public Transform E_transformPos;
    public Vector2 E_InitialOffset;

    private void Setup(string text, float talkspeed, Transform chatBubbleTransform, GameObject InteractButton, NPCInteract npcInteract)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeEffect(text, talkspeed, InteractButton));
        StartCoroutine(DespawnTrigger(chatBubbleTransform, talkspeed, text, InteractButton, npcInteract));
    }

    private IEnumerator TypeEffect(string text, float talkspeed, GameObject interactButton)
    {
        ChatTextTmp.text = "";

        foreach (char letter in text)
        {
            ChatTextTmp.text += letter;

            ChatTextTmp.ForceMeshUpdate();
            Vector2 textSize = ChatTextTmp.GetRenderedValues(false);

            BackgroundSpriteRenderer.size = textSize + padding;

            BackgroundSpriteRenderer.transform.localPosition = new Vector3(BackgroundSpriteRenderer.size.x / 2f, 0f);
            
            UpdateRightSideObjectPosition(E_transformPos, interactButton);

            yield return new WaitForSeconds(talkspeed);

            if (Input.GetKeyDown(KeyCode.E))
            {
                ChatTextTmp.text = text;

                ChatTextTmp.ForceMeshUpdate();
                textSize = ChatTextTmp.GetRenderedValues(false);

                BackgroundSpriteRenderer.size = textSize + padding;
                BackgroundSpriteRenderer.transform.localPosition = new Vector3(BackgroundSpriteRenderer.size.x / 2f, 0f);
                UpdateRightSideObjectPosition(E_transformPos, interactButton);
                break;
            }
        }
    }

    private IEnumerator DespawnTrigger(Transform chatBubbleTransform, float talkspeed, string text, GameObject InteractButton, NPCInteract npcInteract)
    {
        yield return new WaitForSeconds(npcInteract.dialougeExtraTime + talkspeed * text.Length);

        GetComponent<Animator>().SetTrigger("Despawn");

        npcInteract.DespawnInteractButton();

        yield return new WaitForSeconds(1f);

        InteractButton.transform.localPosition = E_InitialOffset;

        Destroy(chatBubbleTransform.gameObject);
    }

    private void UpdateRightSideObjectPosition(Transform targetObject, GameObject InteractButton)
    {
        Vector2 backgroundSize = BackgroundSpriteRenderer.size;

        Vector3 rightmostPosition = new Vector3(backgroundSize.x, 0f, 0f);

        Vector3 adjustedPosition = rightmostPosition + rightOffset;

        targetObject.localPosition = adjustedPosition;

        InteractButton.transform.position = E_transformPos.position;
    }
}