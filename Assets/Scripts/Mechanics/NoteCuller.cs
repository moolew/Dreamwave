using UnityEngine;

public class NoteCuller : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var sr = collision.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var sr = collision.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
    }
}
