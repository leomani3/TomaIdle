using BreakInfinity;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AStat<T> where T: IComparable<T>, IEquatable<T>
{
	//Static properties
	public static int FlatDefaultOrder = 100;
	public static int PercentDefaultOrder = 200;

	//Public properties
	[field: SerializeField]
	public bool UseCache { get; protected set; } = true;
	[field: SerializeField]
	public float CacheUpdateInterval { get; protected set; } = 0.1f;
	public Action<T, T> OnValueChanged;

	//Protected properties
	protected Dictionary<StatModifier<T>, int> modifiers = new Dictionary<StatModifier<T>, int>();
	protected List<OrderedValue> orderedValues = new List<OrderedValue>();
	protected Dictionary<int, OrderedValue> orderedValuesDict = new Dictionary<int, OrderedValue>();//For fast access to ordered values by order
	protected Dictionary<int, StatModifier<T>> localModifiers = new Dictionary<int, StatModifier<T>>();

	protected T cachedValue = default;
	protected bool dirtyCache = true;
	protected float lastCacheUpdateTime = -1f;

	//Value Getter
	public T Value
	{
		get
		{
			UpdateCache();
			return cachedValue;
		}
	}

	public AStat(bool useCache = true)
	{
		UseCache = useCache;
	}

	public AStat(float cacheUpdateInterval)
	{
		UseCache = true;
		CacheUpdateInterval = cacheUpdateInterval;
	}

	public abstract T GetBaseValue();

	public virtual void UpdateCache(bool force = false)
	{
		if (dirtyCache && (!UseCache || force || Time.unscaledTime - lastCacheUpdateTime > CacheUpdateInterval))
		{
			T prevValue = cachedValue;
			cachedValue = GetBaseValue();

			OrderedValue orderedValue;
			for (int i = 0; i < orderedValues.Count; i++)
			{
				orderedValue = orderedValues[i];
				if (orderedValue.Type == ModifierType.Flat)
				{
					Add(ref cachedValue, orderedValue.Value);
				}
				else if (orderedValue.Type == ModifierType.Percent)
				{
					Mult(ref cachedValue, orderedValue.Value);
				}
			}
			lastCacheUpdateTime = Time.unscaledTime;
			dirtyCache = false;
			if (!Equal(cachedValue, prevValue))
			{
				OnValueChanged?.Invoke(prevValue, cachedValue);
			}
		}
	}

	protected abstract void Add(ref T a, T b);
	protected abstract T Add(T a, T b);
	protected abstract void Mult(ref T a, T b);
	protected abstract void Divide(ref T a, T b);
	protected abstract void Substract(ref T a, T b);
	protected abstract bool Equal(T a, T b);
	protected abstract T FromNumber(float value);
	protected abstract T FromNumber(double value);
	protected abstract T FromNumber(int value);

	public virtual void ClearCache()
	{
		lastCacheUpdateTime = -1f - CacheUpdateInterval;
	}

	public virtual void AddFlat(T value)
	{
		AddFlat(value, FlatDefaultOrder);
	}

	public virtual void AddFlat(T value, int order)
	{
		if (localModifiers.TryGetValue(order, out StatModifier<T> existingModifier))
		{
			if (existingModifier.Type != ModifierType.Flat)
			{
				throw new Exception($"Cannot add flat modifier with order {order} because it already has a {existingModifier.Type} modifier.");
			}
			existingModifier.UpdateValue(Add(existingModifier.CurrentValue, value));
		}
		else
		{
			StatModifier<T> newLocalModifier = new StatModifier<T>(value, ModifierType.Flat, order);
			localModifiers.Add(order, newLocalModifier);
			AddModifier(localModifiers[order]);
		}
	}

	public virtual void AddPercent(T value)
	{
		AddPercent(value, PercentDefaultOrder);
	}

	public virtual void AddPercent(T value, int order)
	{
		if (localModifiers.TryGetValue(order, out StatModifier<T> existingModifier))
		{
			if (existingModifier.Type != ModifierType.Percent)
			{
				throw new Exception($"Cannot add percent modifier with order {order} because it already has a {existingModifier.Type} modifier.");
			}
			existingModifier.UpdateValue(Add(existingModifier.CurrentValue, value));
		}
		else
		{
			StatModifier<T> newLocalModifier = new StatModifier<T>(value, ModifierType.Percent, order);
			localModifiers.Add(order, newLocalModifier);
			AddModifier(localModifiers[order]);
		}
	}

	public virtual void AddModifier(StatModifier<T> modifier)
	{
 		if (!modifiers.TryAdd(modifier, 1))
			modifiers[modifier]++;//Same stat can be added multiple times
		modifier.OnValueChange += OnModifierValueChange;

		OrderedValue orderedValue;
		if (!orderedValuesDict.ContainsKey(modifier.Order))
		{
			orderedValue = new OrderedValue(modifier.Order, modifier.Type, this);
			orderedValuesDict.Add(modifier.Order, orderedValue);

			orderedValues.Add(orderedValue);
			orderedValue.RegisteredModifiersCount = 1;
		}
		else
		{
			orderedValue = orderedValuesDict[modifier.Order];
			orderedValue.RegisteredModifiersCount++;
		}

		OnModifierValueChange(modifier, default, modifier.CurrentValue);
	}

	public virtual void RemoveModifier(StatModifier<T> modifier)
	{
		if (modifiers.TryGetValue(modifier, out int count))
		{
			modifier.OnValueChange -= OnModifierValueChange;

			OnModifierValueChange(modifier, modifier.CurrentValue, default);
			if (count == 1)
			{
				modifiers.Remove(modifier);
			}
			else
			{
				modifiers[modifier]--;
			}
			if (orderedValuesDict.TryGetValue(modifier.Order, out OrderedValue orderedValue))
			{
				orderedValue.RegisteredModifiersCount--;
				if (orderedValue.RegisteredModifiersCount == 0)
				{
					orderedValues.Remove(orderedValue);
					orderedValuesDict.Remove(modifier.Order);
				}
			}
			else
			{
				throw new Exception($"Modifier not found in ordered values with order {modifier.Order}");
			}
		}
	}

	public virtual void ClearModifiers()
	{
		foreach (var modifier in modifiers)
		{
			if (modifier.Key == null)
				continue;
			for (int i = 0; i < modifier.Value; i++)
			{
				modifier.Key.OnValueChange -= OnModifierValueChange;
				OnModifierValueChange(modifier.Key, modifier.Key.CurrentValue, default);
			}
		}
		modifiers.Clear();
		orderedValues.Clear();
		orderedValuesDict.Clear();
	}

	protected virtual void OnModifierValueChange(StatModifier<T> modifier, T prevValue, T newValue)
	{
		if (orderedValuesDict.TryGetValue(modifier.Order, out OrderedValue orderedValue))
		{
			Substract(ref orderedValue.Value, prevValue);
			Add(ref orderedValue.Value, newValue);
			
			OnValueChanged?.Invoke(prevValue, newValue);
			dirtyCache = true;
		}
		else
		{
			throw new Exception($"Modifier not found in ordered values with order {modifier.Order}");
		}
	}

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
