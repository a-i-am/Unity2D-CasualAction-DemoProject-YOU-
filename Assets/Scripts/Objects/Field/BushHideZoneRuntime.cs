using UnityEngine;

public class BushHideZoneRuntime : MonoBehaviour
{
    [SerializeField] private float hiddenAlpha = 0.55f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hide(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Hide(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SpriteRenderer renderer = other.GetComponent<SpriteRenderer>();
        if (renderer == null) return;
        if (renderer.color.a < hiddenAlpha) return;

        SetAlpha(renderer, 1f);
    }

    private void Hide(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SpriteRenderer renderer = other.GetComponent<SpriteRenderer>();
        if (renderer == null) return;

        SetAlpha(renderer, Mathf.Min(renderer.color.a, hiddenAlpha));
    }

    private void SetAlpha(Collider2D other, float alpha)
    {
        SpriteRenderer renderer = other.GetComponent<SpriteRenderer>();
        if (renderer != null) SetAlpha(renderer, alpha);
    }

    private void SetAlpha(SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }
}
