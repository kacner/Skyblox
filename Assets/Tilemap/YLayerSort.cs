using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YLayerSort : MonoBehaviour
{
    private Transform player;

    [SerializeField] private float offset = 0.2f;

    private SpriteRenderer[] spriteRenderers;
    private ParticleSystemRenderer particleRenderer;

    private void Start()
    {
        player = GameManager.instance.player.transform ;
        spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        particleRenderer = transform.GetComponentInChildren<ParticleSystemRenderer>();
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing!");
            return;
        }

        if (player.position.y < transform.position.y - offset)
        {
            SetSortingLayer("Walk in Front of");
        }
        else
        {
            SetSortingLayer("Walk behind");
        }
    }

    private void SetSortingLayer(string layerName)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.sortingLayerName = layerName;
        }

        if (particleRenderer != null)
        {
            particleRenderer.sortingLayerName = layerName;
        }
    }
}
