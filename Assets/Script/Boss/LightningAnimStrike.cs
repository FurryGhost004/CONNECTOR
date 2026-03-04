using UnityEngine;
using System.Collections;

public class LightningAnimStrike : MonoBehaviour
{
    public float lifeTime = 0.5f;
    void OnEnable()
    {
        // Tự huỷ sau khi animation xong
        Destroy(gameObject, lifeTime);
    }

    
}
