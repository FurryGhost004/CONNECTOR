using UnityEngine;

public class Wire : MonoBehaviour
{
    public LineRenderer Line;
    public Transform EndWire;
    bool Dragging = false;  

    Vector3 OriginalPosition;
    bool Connected = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OriginalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Dragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 convertedMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            convertedMousePosition.z = 0;
            SetPosition(convertedMousePosition);

            Vector3 endWireDifference = convertedMousePosition - EndWire.position;
            if (endWireDifference.magnitude < .5f)
            {
                SetPosition(EndWire.position);
                Dragging = false;
                Connected = true;
            }
        }
    }

    void SetPosition(Vector3 pPosition)
    {
        transform.position = pPosition;

        Vector3 positionDifference = pPosition - Line.transform.position;
        Line.SetPosition(2, positionDifference - new Vector3(.75f, 0, 0));
        Line.SetPosition(3, positionDifference - new Vector3(.15f, 0, 0));
    }

    void ResetPosition()
    {
        SetPosition(OriginalPosition);
    }
    private void OnMouseDown()
    {
        Dragging = true;
    }

    private void OnMouseUp()
    {
        Dragging = false;
        if (!Connected)
        { 
            ResetPosition();
        }   
    }

    public bool IsConnected()
    { 
        return Connected;
    }

    public void SetConnected(bool pConnected)
    {
        Connected = pConnected;
        if (!Connected)
        {
            ResetPosition();
        }
    }
}
