using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AbilityData : ScriptableObject
{
    public ParticleSystemPoolRef fxPoolRef;
    [HideIf("@castType == CastType.Projectile")]public float hitBoxRadius;
    public float range;
    public CastType castType;
    [ShowIf("@castType == CastType.TimeBased")] public int nbTick;
    [ShowIf("@castType == CastType.TimeBased")] public float duration = 1f;
    [SerializeReference] public List<AbilityEffect> effects = new List<AbilityEffect>();
    [ShowIf("@castType == CastType.Projectile")] public ProjectilePoolRef projectilePoolRef;
    public float manaCost;
    public TargetAcuisitionType targetAcuisitionType;
    public float cooldown;
    public Sprite icon;
    public Sprite shortcutIcon;
}

public enum CastType
{
    Instant,
    TimeBased,
    Projectile
}

public enum TargetAcuisitionType
{
    Closest,
    Furthest,
    Random,
}