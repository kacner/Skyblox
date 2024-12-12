using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractionManager : MonoBehaviour
{
    private List<Interactable> ObjectsWithinRange;
    public Interactable ClosestObject;
    [SerializeField] private float interactionRange = 2f;
    public Interactable UsableClosetsObject;
    private void Start()
    {
        ObjectsWithinRange = new List<Interactable>();
    }
    private void Update()
    {
        SearchArea();
        UpdateClosestInteractable();
    }
    void SearchArea()
    {
        ObjectsWithinRange.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        foreach (Collider2D hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                ObjectsWithinRange.Add(interactable);
            }
        }
    }
    private void UpdateClosestInteractable()
    {
        float closestDistance = float.MaxValue;
        Interactable newClosestObject = null;

        foreach (Interactable interactable in ObjectsWithinRange)
        {
            float distance = Vector2.Distance(transform.position, interactable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                newClosestObject = interactable;
            }
        }
        if (ClosestObject != newClosestObject)
        {
            if (ClosestObject != null)
            {
                ClosestObject.IsWithingRange = false; // Set the previous closest object out of range
            }

            ClosestObject = newClosestObject;

            if (ClosestObject != null)
            {
                ClosestObject.IsWithingRange = true; // Set the new closest object in range
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
