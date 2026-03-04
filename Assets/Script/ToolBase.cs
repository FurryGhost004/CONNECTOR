using UnityEngine;

public enum ToolTargetHeight
{
    GroundOnly,
    AirOnly,
    Both
}

public class ToolBase : MonoBehaviour
{
    public ToolTargetHeight targetHeight = ToolTargetHeight.GroundOnly;

    private bool CanAffect(EnemyHeightType enemyHeight)
    {
        if (this.targetHeight == ToolTargetHeight.Both) return true;
        if (this.targetHeight == ToolTargetHeight.GroundOnly && enemyHeight == EnemyHeightType.Ground) return true;
        if (this.targetHeight == ToolTargetHeight.AirOnly && enemyHeight == EnemyHeightType.Air) return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyInfo info = other.GetComponent<EnemyInfo>();
        if (info == null) return;

        if (!this.CanAffect(info.heightType)) return;

        EnemyAI ai = other.GetComponent<EnemyAI>();
        if (ai != null)
        {
            // Gọi hàm đã có trong EnemyAI và truyền thời gian choáng (ví dụ 2 giây)
            ai.StunDuration(2.0f);
        }
    }
}