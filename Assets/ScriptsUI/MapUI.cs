using UnityEngine;

public class MapUI : MonoBehaviour
{
    bool isnearby = false;
    [SerializeField] GameObject map;

    private void Update()
    {
        if (isnearby)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                map.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isnearby = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isnearby = false;
        map.SetActive(false);
    }
}
