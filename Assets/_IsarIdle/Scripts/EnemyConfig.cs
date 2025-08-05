using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "ScriptableObjects/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Title("Stats")]
    public BigBalanceFunction xpGiven;
}