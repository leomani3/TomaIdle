using System;
using MyBox;
using UnityEngine;

public class EnemyTargetingEM : TargetingEM
{
    private void Start()
    {
        if (m_linkedEntity != null && m_linkedEntity.TryGetModule(out HealthEM _healthEM))
        {
            _healthEM.OnDamaged += OnReceiveHit;
        }
    }

    private void OnReceiveHit(bool _damageWillKill)
    {
        if (EntityManager.Instance.Player != null)
        {
            m_mainTarget = EntityManager.Instance.Player;
        }
    }
}