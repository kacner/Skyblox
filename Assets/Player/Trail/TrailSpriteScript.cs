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

        Transform childTransform = transform.Find(childName);

        Material childMaterial = childTransform.GetComponent<SpriteRenderer>().material;

        spriteRenderer.material = childMaterial;

        Destroy(transform.Find("Shadow").gameObject);

        Transform[] childrenObj = GetComponentsInChildren<Transform>();
        foreach (Transform obj in childrenObj)
        {
            if (obj.name.Contains("__Weapond__"))
            {
                Destroy(obj);
                break;
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
