using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color color;
    private float alpha;
    private float fadeSpeed;

    public void Init(Sprite sprite, Vector3 position, Quaternion rotation, Vector3 scale, Color startColor, float speed, int sortingOrder)
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();

        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder - 1; // Luôn hiển thị phía sau nhân vật thật

        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;

        color = startColor;
        fadeSpeed = speed;
        alpha = color.a;
        sr.color = color;
    }

    void Update()
    {
        alpha -= fadeSpeed * Time.deltaTime;
        color.a = alpha;
        sr.color = color;

        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}