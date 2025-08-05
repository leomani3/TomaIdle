using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class PlayerEntity : Entity
{
    [SerializeField] private ParticleSystemPoolRef m_levelUpFxPoolRef;

    protected override void Awake()
    {
        base.Awake();
        GameData.Instance.onPlayerLevelUp += OnPlayerLevelUp;
    }

    public override void OnUnregistered()
    {
        base.OnUnregistered();
        GameData.Instance.onPlayerLevelUp -= OnPlayerLevelUp;
    }

    private void OnPlayerLevelUp(int obj)
    {
        m_levelUpFxPoolRef.pool.Spawn(transform.position, quaternion.identity, m_levelUpFxPoolRef.pool.transform);
    }
}