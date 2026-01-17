using UnityEngine;

public enum EnemyHeightType
{
    Ground,
    Air
}

public class EnemyInfo : MonoBehaviour
{
    public EnemyHeightType heightType = EnemyHeightType.Ground;
}