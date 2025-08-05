using System;
using BreakInfinity;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerHealthEM : HealthEM
{
    [SerializeField] private BigDoubleVariable m_currentHealthFloatVariable;

    protected override void Start()
    {
        base.Start();
        
        if (m_currentHealthFloatVariable != null)
            m_currentHealthFloatVariable.Value = m_currentHealth;
    }

    public override void TakeDamage(BigDouble amount)
    {
        base.TakeDamage(amount);
        
        if (m_currentHealthFloatVariable != null)
            m_currentHealthFloatVariable.Value = m_currentHealth;
    }
    
    public void AddMaxHealth()
    {
        m_linkedEntity.GetModule<StatsEM>().MaxHealth.AddFlat(new BigDouble(10, 0));
    }
    
    public void testtakedamage()
    {
        m_linkedEntity.GetModule<HealthEM>().TakeDamage(10);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddMaxHealth();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            testtakedamage();
        }
    }

    public override void Heal(BigDouble amount)
    {
        base.Heal(amount);
        
        if (m_currentHealthFloatVariable != null)
            m_currentHealthFloatVariable.Value = m_currentHealth;
    }

    protected override void OnMaxHealthChanged(BigDouble prevMaxHealth, BigDouble newMaxHealth)
    {
        base.OnMaxHealthChanged(prevMaxHealth, newMaxHealth);
        if (m_currentHealthFloatVariable != null)
            m_currentHealthFloatVariable.Value = m_currentHealth;
    }
}