using System;
using System.Collections.Generic;

public class StatModifier<T> where T : IComparable<T>
{
	public Action<StatModifier<T>, T, T> OnValueChange;
	

	public T CurrentValue { get; protected set; }
	public ModifierType Type { get; protected set; }
	public int Order { get; set; } = 0;

	public StatModifier(T value, ModifierType modifier)
	{
		CurrentValue = value;
		Type = modifier;
		switch (Type)
		{
			case ModifierType.Flat:
				Order = Stat.FlatDefaultOrder;
				break;
			case ModifierType.Percent:
				Order = Stat.PercentDefaultOrder;
				break;
			default:
				break;
		}
	}
	
	public StatModifier(T value, ModifierType modifier, int order)
	{
		CurrentValue = value;
		Type = modifier;
		Order = order;
	}

	public StatModifier(StatModifier<T> other)
	{
		CurrentValue = other.CurrentValue;
		Type = other.Type;
	}

	public void UpdateValue(T newValue)
	{
		T prevValue = CurrentValue;
		CurrentValue = newValue;
		OnValueChange?.Invoke(this, prevValue, newValue);
	}
}

public enum ModifierType
{
	Flat,
	Percent
}
