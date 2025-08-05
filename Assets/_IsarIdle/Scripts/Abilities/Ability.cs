using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AbilityContextBuilder
{
    private AbilityContext m_abilityContext;

    public AbilityContextBuilder()
    {
        m_abilityContext = new AbilityContext();
    }
    
    public AbilityContextBuilder SetTargetPosition(Vector3 _targetPos)
    {
        m_abilityContext.targetPosition = _targetPos;
        return this;
    }
    
    public AbilityContextBuilder SetTargetEntity(Entity _targetEntity)
    {
        m_abilityContext.targetEntity = _targetEntity;
        return this;
    }
    
    public AbilityContextBuilder SetOwnerEntity(Entity _ownerEntity)
    {
        m_abilityContext.ownerEntity = _ownerEntity;
        return this;
    }
    
    public AbilityContext Build()
    {
        return m_abilityContext;
    }
}

public class AbilityContext
{
    public Vector3 targetPosition;
    public Entity targetEntity;
    public Entity ownerEntity;
}

public class Ability : MonoBehaviour
{ 
    private AbilityData m_abilityData;
    private AbilityContext m_abilityContext;
    private LayerMask m_layerMask;
    
    private Tween m_abilityCastTween;

    public void UseAbility(AbilityData _abilityData, AbilityContext _abilityContext, LayerMask _layerMask)
    {
        m_abilityData = _abilityData;   
        m_abilityContext = _abilityContext;
        m_layerMask = _layerMask;
        
        ApplyAbility();
    }

    private void ApplyAbility()
    {
        // fx
        ParticleSystem _ps = m_abilityData.fxPoolRef.pool.Spawn(m_abilityContext.targetPosition, transform.rotation, m_abilityData.fxPoolRef.pool.transform);
        _ps.transform.localScale = Vector3.one * (m_abilityData.hitBoxRadius * 2);
        switch (m_abilityData.castType)
        {
            case CastType.Instant:
                ApplyEffectsToTargets();
                break;
            case CastType.TimeBased:
                float tickInterval = m_abilityData.duration / m_abilityData.nbTick;
                int tickCount = 0;

                m_abilityCastTween.Kill();
                m_abilityCastTween = DOVirtual.DelayedCall(0f, () =>
                {
                    m_abilityCastTween = DOTween.Sequence()
                        .AppendCallback(() =>
                        {
                            ApplyEffectsToTargets();
                            tickCount++;
                        })
                        .AppendInterval(tickInterval)
                        .SetLoops(m_abilityData.nbTick);
                });
                break;
            case CastType.Projectile:
                Projectile projectile = m_abilityData.projectilePoolRef.pool.Spawn(m_abilityContext.targetPosition, transform.rotation, m_abilityData.fxPoolRef.pool.transform);
                projectile.LaunchAtPosition(m_abilityContext.ownerEntity, m_abilityContext.targetPosition, m_abilityData.effects);
                break;
        }
    }
    
    private void ApplyEffectsToTargets()
    {
        if (m_abilityContext == null || m_abilityContext.ownerEntity == null)
            return;
        
        foreach (Entity entity in GetTargets())
        {
            foreach (AbilityEffect abilityEffect in m_abilityData.effects)
                abilityEffect.ApplyEffect(m_abilityContext.ownerEntity, entity);
        }
    }

    
    private List<Entity> GetTargets()
    {
        List<Entity> _targets = new List<Entity>();
        
        Collider[] _colliders = Physics.OverlapSphere(m_abilityContext.targetPosition, m_abilityData.hitBoxRadius, m_layerMask);
        foreach (Collider collider in _colliders)
        {
            Entity _touchedEntity = EntityManager.Instance.GetEntityByCollider(collider);
            _targets.Add(_touchedEntity);
        }
        
        
        return _targets;
    }
}