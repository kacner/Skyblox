using System.Collections;
using UnityEngine;

public class TrailSpriteScript : MonoBehaviour
{
    public float Duration = 0.7f;
    private SpriteRenderer spriteRenderer;
    public string childName = "IGNORE MATERIAL OBJECT";

    public Color startColor = new Color(1f, 1f, 1f, 0.4f);
    public Color endColor = new Color(0.43f, 0.26f, 0.76f, 0f);
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -2;



        Transform[] childrenObj = GetComponentsInChildren<Transform>();
        foreach (Transform obj in childrenObj)
        {
            if (obj.name.Contains(childName))
            {
                Transform childTransform = obj;

                Material childMaterial = childTransform.GetComponent<SpriteRenderer>().material;

                spriteRenderer.material = childMaterial;
            }
            if (obj.name.Contains("WaterCheck") || obj.name.Contains("DmgParticleSystem") || obj.name.Contains("Shadow") || obj.name.Contains("Camera") || obj.name.Contains("__Weapond__"))
            {
                Destroy(obj.gameObject);
            }
        }

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float Time = 0f;

        while (Time < Duration)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, Time / Duration);
            Time += UnityEngine.Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = endColor;

        Destroy(gameObject);
    }
}
