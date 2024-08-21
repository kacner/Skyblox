using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrailSpriteScript : MonoBehaviour
{
    public float fadeDuration = 0.7f;
    private SpriteRenderer spriteRenderer;
    public string childName = "IGNORE MATERIAL OBJECT";

    public Color startColor = new Color(1f, 1f, 1f, 0.4f);
    public Color endColor = new Color(0.43f, 0.26f, 0.76f, 0f);
    void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -2;

        Transform childTransform = transform.Find(childName);

        SpriteRenderer childSpriteRenderer = childTransform.GetComponent<SpriteRenderer>();

        Material childMaterial = childSpriteRenderer.material;

        spriteRenderer.material = childMaterial;

        Transform shadow = transform.Find("Shadow");

        if (shadow != null)
            Destroy(shadow.gameObject);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
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
        float elapsedTime = 0f;

        //startColor = new Color(1f, 1f, 1f, 0.4f); // Initial color (solid blue)
        //endColor = new Color(0f, 0.1f, 0.5f, 0f); // Final color (transparent blue)

        // Fade out over the duration
        while (elapsedTime < fadeDuration)
        {
            // Lerp the color over time
            spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = endColor;

        Destroy(gameObject);
    }
}
