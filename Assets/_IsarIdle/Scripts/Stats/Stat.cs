using BreakInfinity;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat : AStat<float>
{
	//Public properties
	[field: SerializeField]
	public float BaseValue { get; protected set; }

	public override float GetBaseValue()
	{
		return BaseValue;
	}

	public Stat(float baseValue) : base()
	{
		BaseValue = baseValue;
	}
	
	public void SetBaseValue(float baseValue)
	{
		if (baseValue != BaseValue)
		{
			BaseValue = baseValue;
		}
	}

	protected override void Add(ref float a, float b)
	{
		a += b;
	}

	protected override float Add(float a, float b)
	{
		return a + b;
	}

	protected override void Mult(ref float a, float b)
	{
		a *= b;
	}

	protected override void Divide(ref float a, float b)
	{
		a /= b;
	}

	protected override void Substract(ref float a, float b)
	{
		a -= b;
	}

	protected override bool Equal(float a, float b)
	{
		return a == b;
	}

	protected override float FromNumber(float value)
	{
		return value;
	}

	protected override float FromNumber(double value)
	{
		return (float)value;
	}

	protected override float FromNumber(int value)
	{
		return value;
	}
}
