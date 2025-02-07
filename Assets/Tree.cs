using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private List<SpriteRenderer> objectsToFadeOut = new List<SpriteRenderer>();
    [SerializeField] private Color transparent;
    [SerializeField] private float fadeSpeed = 0.2f;
    private Coroutine fadeOutCorotine;
    private Coroutine fadeInCorotine;
    private List<Color> InitialColors = new List<Color>();
    private void Start()
    {
        objectsToFadeOut.AddRange(GetComponentsInChildren<SpriteRenderer>());

        for (int i = 0; i < objectsToFadeOut.Count; i++)
        {
            InitialColors.Add(objectsToFadeOut[i].color);
        }
    }
    IEnumerator FadeOutObject()
    {
        if (objectsToFadeOut.Count > 0)
        {
            float duration = fadeSpeed;
            float timer = 0;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                for (int i = 0; i < objectsToFadeOut.Count; i++)
                {
                    objectsToFadeOut[i].color = Color.Lerp(objectsToFadeOut[i].color, transparent, timer / duration);
                }
                yield return null;
            }
            for (int i = 0; i < objectsToFadeOut.Count; i++)
            {
                objectsToFadeOut[i].color = transparent;
            }
        }
    }
    IEnumerator FadeInObject()
    {
        if (objectsToFadeOut.Count > 0)
        {
            float duration = fadeSpeed;
            float timer = 0;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                for (int i = 0; i < objectsToFadeOut.Count; i++)
                {
                    objectsToFadeOut[i].color = Color.Lerp(objectsToFadeOut[i].color, InitialColors[i], timer / duration);
                }
                yield return null;
            }
            for (int i = 0; i < objectsToFadeOut.Count; i++)
            {
                objectsToFadeOut[i].color = InitialColors[i];
            }
        }
    }

    public void FadeOut()
    {
        if (fadeInCorotine != null)
        {
            StopCoroutine(fadeInCorotine);
            fadeInCorotine = null;
        }
        fadeOutCorotine = StartCoroutine(FadeOutObject());
    }

    public void FadeIn()
    {
        if (fadeOutCorotine != null)
        {
            StopCoroutine(fadeOutCorotine);
            fadeOutCorotine = null;
        }
        fadeInCorotine = StartCoroutine(FadeInObject());
    }
}
