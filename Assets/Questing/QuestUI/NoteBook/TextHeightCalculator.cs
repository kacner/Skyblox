using TMPro;
using UnityEngine;

public class TextHeightCalculator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI QuestText1; // Reference to your TextMeshProUGUI object
    public Color gizmoColor = Color.green; // Color for the Gizmos
    public Vector2 currentTextOffset = Vector2.zero; // Offset for the current text box
    public Vector2 nextTextOffset = Vector2.zero; // Offset for the next text box
    [SerializeField] private GameObject TextInstantiatePrefab; // Prefab to instantiate new text objects
    [SerializeField] private Vector2 prefabspawningOffset;
    [SerializeField] private GameObject iniSpawnPoint;
    private TextMeshProUGUI previousGameobject;
    private float verticalOffset;
    void Start()
    {
        if (QuestText1 == null)
        {
            Debug.LogError("TextMeshProUGUI reference is missing.");
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CreateNewQuest();
        }
    }

    void CreateNewQuest()
    {
        if (QuestText1 != null)
        {
            // Get the current bounds of the last spawned text
            Rect TextBounds = GetRenderedTextBounds(QuestText1);
            float previousHeight = TextBounds.height;

            // Instantiate a new text prefab as a child of this object
            GameObject spawnedText = Instantiate(TextInstantiatePrefab, transform);

            // Update QuestText1 to reference the newly spawned TextMeshProUGUI
            QuestText1 = spawnedText.GetComponentInChildren<TextMeshProUGUI>();
            RectTransform spawnedRectTransform = spawnedText.GetComponent<RectTransform>();

            if (spawnedRectTransform != null)
            {
                // Update the vertical offset cumulatively (subtracting the previous height)
                nextTextOffset.y -= (previousHeight + prefabspawningOffset.y);

                // Set the new local position
                Vector2 newTextLocalPosition = new Vector2(prefabspawningOffset.x, nextTextOffset.y - previousHeight);
                spawnedRectTransform.localPosition = newTextLocalPosition;

                Debug.Log("New text instantiated at: " + newTextLocalPosition);
            }
            else
            {
                Debug.LogError("Spawned prefab is missing a RectTransform component.");
            }
        }
        else
        {
            // Handle initial quest text instantiation
            Debug.Log("Attempted to instantiate initial quest");
            GameObject spawnedText = Instantiate(TextInstantiatePrefab, iniSpawnPoint.transform);
            QuestText1 = spawnedText.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void OnDrawGizmos()
    {
        if (QuestText1 != null)
        {
            // Get the RectTransform of the TextMeshProUGUI object
            RectTransform rectTransform = QuestText1.rectTransform;

            // Get world position of the RectTransform
            Vector3 worldPosition = rectTransform.position;

            // Get the rendered bounds of the text
            Rect textBounds = GetRenderedTextBounds(QuestText1);

            // Adjust for world coordinates
            textBounds.position += new Vector2(worldPosition.x, worldPosition.y) + currentTextOffset;

            // Draw the bounds of the current text
            Gizmos.color = gizmoColor;
            DrawRectGizmo(textBounds);

            // Calculate the next position for another textbox
            Rect nextBounds = new Rect(
                textBounds.xMin,
                textBounds.yMin - textBounds.height,
                textBounds.width,
                textBounds.height
            );

            // Apply offset for the next bounds
            nextBounds.position += nextTextOffset;

            // Draw a box where the next textbox could go
            Gizmos.color = Color.blue;
            DrawRectGizmo(nextBounds);
        }
    }

    Rect GetRenderedTextBounds(TextMeshProUGUI tmp)
    {
        tmp.ForceMeshUpdate(); // Ensure the mesh is updated
        Bounds textBounds = tmp.textBounds; // Get the bounds of the rendered text

        // Convert bounds to a Rect for easier handling
        Vector3 size = textBounds.size;
        Vector3 center = textBounds.center;

        return new Rect(
            center.x - size.x / 2,
            center.y - size.y / 2,
            size.x,
            size.y
        );
    }

    void DrawRectGizmo(Rect rect)
    {
        Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMin, 0);
        Vector3 bottomRight = new Vector3(rect.xMax, rect.yMin, 0);
        Vector3 topLeft = new Vector3(rect.xMin, rect.yMax, 0);
        Vector3 topRight = new Vector3(rect.xMax, rect.yMax, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}