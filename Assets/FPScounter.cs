using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Drawing;

public class FPScounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    private float deltaTime = 0.0f;

    private void Start()
    {
        fpsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float fps = 1.0f / deltaTime;
        int fpsInt = Mathf.RoundToInt(fps);

        fpsText.text = $"${fpsInt.ToString("D7")}";
            //string.Format("$FPS< {0:0.}", fps);

    }
}
