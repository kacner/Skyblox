using UnityEngine;

public class FadeOutTilemaps : MonoBehaviour
{
    [SerializeField] private LayerMask layer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((layer.value & (1 << collision.transform.gameObject.layer)) > 0)
        {
            collision.GetComponent<Tree>().FadeOut();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((layer.value & (1 << collision.transform.gameObject.layer)) > 0)
        {
            collision.GetComponent<Tree>().FadeIn();
        }
    }
}
