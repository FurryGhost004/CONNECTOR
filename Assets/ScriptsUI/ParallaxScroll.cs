using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    public float scrollSpeed = 2f;      // tốc độ cuộn
    public float backgroundWidth = 20f; // độ rộng 1 background (đặt cho đúng ảnh)

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Di chuyển nền sang phải
        transform.position += Vector3.right * scrollSpeed * Time.deltaTime;

        // Nếu di chuyển quá xa thì reset về ban đầu để lặp
        if (transform.position.x >= startPosition.x + backgroundWidth)
        {
            transform.position -= new Vector3(backgroundWidth * 2, 0, 0);
        }
    }
}
