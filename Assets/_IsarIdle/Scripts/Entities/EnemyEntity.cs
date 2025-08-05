using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyEntity : Entity
{
    [SerializeField] private EnemyConfig m_enemyConfig;

    [SerializeField, ReadOnly] private int m_currentLevel;

    private void Start()
    {
        if (TryGetModule(out HealthEM _healthEM))
        {
            _healthEM.OnDeath += OnDeath;
        }
    }

    private void OnDeath(Entity _deadEnemy)
    {
		GameData.Instance.GainExperience(m_enemyConfig.xpGiven.Evaluate(m_currentLevel));
    }
}