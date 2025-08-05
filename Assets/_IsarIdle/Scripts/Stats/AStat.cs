using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AStat<T> where T : IComparable<T>, IEquatable<T>
{
	// Static properties
	public static int FlatDefaultOrder = 100;
	public static int PercentDefaultOrder = 200;

	// Public events
	public Action<T, T> OnValueChanged;

	// Protected properties
	protected Dictionary<StatModifier<T>, int> modifiers = new Dictionary<StatModifier<T>, int>();
	protected List<OrderedValue> orderedValues = new List<OrderedValue>();
	protected Dictionary<int, OrderedValue> orderedValuesDict = new Dictionary<int, OrderedValue>();
	protected Dictionary<int, StatModifier<T>> localModifiers = new Dictionary<int, StatModifier<T>>();

	// Value Getter – now computes on demand
	public T Value
	{
		get
		{
			T value = GetBaseValue();
			foreach (var orderedValue in orderedValues)
			{
				if (orderedValue.Type == ModifierType.Flat)
				{
					Add(ref value, orderedValue.Value);
				}
				else if (orderedValue.Type == ModifierType.Percent)
				{
					Mult(ref value, orderedValue.Value);
				}
			}
			return value;
		}
	}

	// Constructors
	public AStat() { }

	// Abstract methods to implement
	public abstract T GetBaseValue();
	protected abstract void Add(ref T a, T b);
	protected abstract T Add(T a, T b);
	protected abstract void Mult(ref T a, T b);
	protected abstract void Divide(ref T a, T b);
	protected abstract void Substract(ref T a, T b);
	protected abstract bool Equal(T a, T b);
	protected abstract T FromNumber(float value);
	protected abstract T FromNumber(double value);
	protected abstract T FromNumber(int value);

	// Modifier APIs
	public virtual void AddFlat(T value) => AddFlat(value, FlatDefaultOrder);
	public virtual void AddFlat(T value, int order)
	{
		if (localModifiers.TryGetValue(order, out var existingModifier))
		{
			if (existingModifier.Type != ModifierType.Flat)
				throw new Exception($"Cannot add flat modifier with order {order} because it already has a {existingModifier.Type} modifier.");
			existingModifier.UpdateValue(Add(existingModifier.CurrentValue, value));
		}
		else
		{
			var newModifier = new StatModifier<T>(value, ModifierType.Flat, order);
			localModifiers.Add(order, newModifier);
			AddModifier(newModifier);
		}
	}

	public virtual void AddPercent(T value) => AddPercent(value, PercentDefaultOrder);
	public virtual void AddPercent(T value, int order)
	{
		if (localModifiers.TryGetValue(order, out var existingModifier))
		{
			if (existingModifier.Type != ModifierType.Percent)
				throw new Exception($"Cannot add percent modifier with order {order} because it already has a {existingModifier.Type} modifier.");
			existingModifier.UpdateValue(Add(existingModifier.CurrentValue, value));
		}
		else
		{
			var newModifier = new StatModifier<T>(value, ModifierType.Percent, order);
			localModifiers.Add(order, newModifier);
			AddModifier(newModifier);
		}
	}

	public virtual void AddModifier(StatModifier<T> modifier)
	{
		if (!modifiers.TryAdd(modifier, 1))
			modifiers[modifier]++;
		modifier.OnValueChange += OnModifierValueChange;

		if (!orderedValuesDict.TryGetValue(modifier.Order, out var orderedValue))
		{
			orderedValue = new OrderedValue(modifier.Order, modifier.Type, this);
			orderedValuesDict.Add(modifier.Order, orderedValue);
			orderedValues.Add(orderedValue);
		}
		orderedValue.RegisteredModifiersCount++;
		OnModifierValueChange(modifier, default, modifier.CurrentValue);
	}

	public virtual void RemoveModifier(StatModifier<T> modifier)
	{
		if (modifiers.TryGetValue(modifier, out int count))
		{
			modifier.OnValueChange -= OnModifierValueChange;
			OnModifierValueChange(modifier, modifier.CurrentValue, default);

			if (count == 1)
				modifiers.Remove(modifier);
			else
				modifiers[modifier]--;

			if (orderedValuesDict.TryGetValue(modifier.Order, out var orderedValue))
			{
				orderedValue.RegisteredModifiersCount--;
				if (orderedValue.RegisteredModifiersCount == 0)
				{
					orderedValues.Remove(orderedValue);
					orderedValuesDict.Remove(modifier.Order);
				}
			}
			else throw new Exception($"Modifier not found in ordered values with order {modifier.Order}");
		}
	}

	public virtual void ClearModifiers()
	{
		foreach (var kvp in modifiers)
		{
			var modifier = kvp.Key;
			for (int i = 0; i < kvp.Value; i++)
			{
				modifier.OnValueChange -= OnModifierValueChange;
				OnModifierValueChange(modifier, modifier.CurrentValue, default);
			}
		}
		modifiers.Clear();
		orderedValues.Clear();
		orderedValuesDict.Clear();
	}

	protected virtual void OnModifierValueChange(StatModifier<T> modifier, T prevModifierValue, T newModifierValue)
	{
		if (orderedValuesDict.TryGetValue(modifier.Order, out var orderedValue))
		{
			T _prevValue = Value;
			Substract(ref orderedValue.Value, prevModifierValue);
			Add(ref orderedValue.Value, newModifierValue);
			OnValueChanged?.Invoke(_prevValue, Value);
		}
		else
		{
			throw new Exception($"Modifier not found in ordered values with order {modifier.Order}");
		}
	}

	// Internal container for values grouped by order/type
	protected class OrderedValue
	{
		public T Value;
		public int Order;
		public ModifierType Type;
		public int RegisteredModifiersCount;
		public AStat<T> StatRef;

		public OrderedValue(int order, ModifierType type, AStat<T> statRef)
		{
			Order = order;
			Type = type;
			StatRef = statRef;

			switch (Type)
			{
				case ModifierType.Flat:
					Value = StatRef.FromNumber(0);
					break;
				case ModifierType.Percent:
					Value = StatRef.FromNumber(1);
					break;
				default:
					Value = StatRef.FromNumber(0);
					break;
			}

			RegisteredModifiersCount = 0;
		}
	}
}