using System;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform eyeTransform;
    public float viewDistance = 5f;
    public float viewAngle = 60f;
    public LayerMask obstacleMask;

    public event Action<Transform> OnPlayerSpotted;

    private Transform playerTransform;

    private void Start()
    {
        if (this.eyeTransform == null)
        {
            this.eyeTransform = this.transform;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            this.playerTransform = playerObject.transform;
        }
    }

    private void Update()
    {
        if (this.playerTransform == null)
        {
            return;
        }

        bool canSee = this.CanSeePlayer();
        if (canSee)
        {
            if (this.OnPlayerSpotted != null)
            {
                this.OnPlayerSpotted(this.playerTransform);
            }
        }
    }

    private bool CanSeePlayer()
    {
        Vector2 origin = this.eyeTransform.position;
        Vector2 toPlayer = (Vector2)this.playerTransform.position - origin;
        float distance = toPlayer.magnitude;

        if (distance > this.viewDistance)
        {
            return false;
        }

        Vector2 forward = this.eyeTransform.right;
        float angle = Vector2.Angle(forward, toPlayer.normalized);
        if (angle > this.viewAngle * 0.5f)
        {
            return false;
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, toPlayer.normalized, this.viewDistance, this.obstacleMask);
        if (hit.collider != null && hit.collider.transform != this.playerTransform)
        {
            return false;
        }

        return true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Gọi sự kiện báo enemy thấy player
            if (this.OnPlayerSpotted != null)
            {
                this.OnPlayerSpotted(collision.transform);
            }
        }
    }

}
