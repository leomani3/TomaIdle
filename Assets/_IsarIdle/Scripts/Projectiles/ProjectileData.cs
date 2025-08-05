using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Projectile Data", fileName = "ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float projectileSpeed = 10f;
    public ParticleSystemPoolRef hitVFXPoolRef;
}