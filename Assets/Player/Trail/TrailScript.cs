using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GameObject clonedObject = Instantiate(gameObject);

            Component[] components = clonedObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component is SpriteRenderer || component is Transform)
                {
                    continue;
                }
                else
                {
                    Destroy(component);
                }
            }
            clonedObject.AddComponent<TrailSpriteScript>();
        }
    }
}
