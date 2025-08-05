using BreakInfinity;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BigStat : AStat<BigDouble>
{
	//Public properties
	[field: SerializeField]
	public BigDouble BaseValue { get; protected set; }

	public override BigDouble GetBaseValue()
	{
		return BaseValue;
	}

	public BigStat(BigDouble baseValue) : base()
	{
		BaseValue = baseValue;
	}
	
	public void SetBaseValue(BigDouble baseValue)
	{
		if (baseValue != BaseValue)
		{
			BaseValue = baseValue;
		}
	}

	protected override void Add(ref BigDouble a, BigDouble b)
	{
		a += b;
	}

	protected override BigDouble Add(BigDouble a, BigDouble b)
	{
		return a + b;
	}

	protected override void Mult(ref BigDouble a, BigDouble b)
	{
		a *= b;
	}

	protected override void Divide(ref BigDouble a, BigDouble b)
	{
		a /= b;
	}

	protected override void Substract(ref BigDouble a, BigDouble b)
	{
		a -= b;
	}

	protected override bool Equal(BigDouble a, BigDouble b)
	{
		return a == b;
	}

	protected override BigDouble FromNumber(float value)
	{
		return value;
	}

	protected override BigDouble FromNumber(double value)
	{
		return value;
	}

	protected override BigDouble FromNumber(int value)
	{
		return value;
	}
}
