using UnityEngine;
using UnityEngine.UI;

public class OffscreenArrows : MonoBehaviour
{
    public Transform enemy;
    public float margin = 50f;
    public RectTransform arrowUI;
    private Camera camera;
    private void Start()
    {
        camera = Camera.main;
        arrowUI.gameObject.SetActive(true);
    }
    void Update()
    {
        Vector3 screenPos = camera.WorldToScreenPoint(enemy.position);

        bool isOffScreen = screenPos.z > 0 && (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height);

        if (isOffScreen)
        {
            float clampedX = Mathf.Clamp(screenPos.x, margin, Screen.width - margin);
            float clampedY = Mathf.Clamp(screenPos.y, margin, Screen.height - margin);
            Vector3 arrowPos = new Vector3(clampedX, clampedY, 0f);

            arrowUI.position = arrowPos;
            arrowUI.gameObject.SetActive(true);

            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 direction = ((Vector2)arrowPos - screenCenter).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowUI.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else
        {
            arrowUI.gameObject.SetActive(false);
        }
    }
}
