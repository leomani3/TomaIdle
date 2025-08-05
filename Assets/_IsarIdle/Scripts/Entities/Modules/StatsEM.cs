using System;
using BreakInfinity;
using UnityEngine;

public class StatsEM : AEntityModule
{
	[field: SerializeField] public BigStat MaxHealth { get; protected set; } = new BigStat(100f, true);
	[field: SerializeField] public BigStat Damages { get; protected set; } = new BigStat(20f, true);
	[field: SerializeField] public Stat AttackSpeed { get; protected set; } = new Stat(1f, true);
	[field: SerializeField] public Stat MaxMana { get; protected set; } = new Stat(1f, true);
	[field: SerializeField] public Stat ManaRegen { get; protected set; } = new Stat(1f, true);
	[field: SerializeField] public Stat HealthRegen { get; protected set; } = new Stat(1f, true);
	[field: SerializeField] public Stat XpPerSecond { get; protected set; } = new Stat(1f, true);

	[SerializeField] private FloatVariable m_maxManaVariable;
	[SerializeField] private BigDoubleVariable m_maxHealthVariable;

	private void Start()
	{
		if (m_maxManaVariable != null)
		{
			m_maxManaVariable.Value = MaxMana.Value;
			MaxMana.OnValueChanged += (float previousValue, float newValue) =>
			{
				m_maxManaVariable.Value = MaxMana.Value;
			};
		}

		if (m_maxHealthVariable != null)
		{
			m_maxHealthVariable.Value = MaxHealth.Value;
			MaxHealth.OnValueChanged += (BigDouble previousValue, BigDouble newValue) =>
			{
				m_maxHealthVariable.Value = MaxHealth.Value;
			};
		}
	}
}