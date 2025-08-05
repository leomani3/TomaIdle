using System;
using UnityEngine;

[Serializable]
public class DamageEffect : AbilityEffect
{
    [SerializeField] public float baseDamage;
    public override void ApplyEffect(Entity _originEntity, Entity _targetEntity)
    {
        if (_targetEntity != null && _targetEntity.TryGetModule(out HealthEM _healthEM))
        {
            if (_originEntity.TryGetModule(out StatsEM _statsEM))
            {
                _healthEM.TakeDamage(baseDamage * _statsEM.Damages.Value);
            }
        }
    }
}