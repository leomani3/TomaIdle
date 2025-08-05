using BreakInfinity;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ABalanceFunction<T> where T : IComparable<T>, IEquatable<T>
{
	public enum FunctionType
	{
		Constant,
		Linear,
		Exponential,
		Logarithmic
	}

	[OnValueChanged(nameof(RegeneratePreview))]
	[SerializeField] protected FunctionType functionType = FunctionType.Linear;

	[OnValueChanged(nameof(RegeneratePreview))]
	[SerializeField] protected float baseValue = 1f;

	[OnValueChanged(nameof(RegeneratePreview))]
	[SerializeField] protected float multiplier = 1f;

	[OnValueChanged(nameof(RegeneratePreview))]
	[ShowIf("@ShowExponent()")]
	[SerializeField] protected float exponent = 2f;

	[OnValueChanged(nameof(RegeneratePreview))]
	[MinValue(1)]
	[SerializeField] protected int previewLength = 10;

	[ReadOnly]
	[ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, IsReadOnly = true)]
	[SerializeField] protected List<string> previewValues = new();


	/// <summary>
	/// Evaluate the function at the given index and return a BigDouble.
	/// </summary>
	public abstract T Evaluate(int index);
	protected abstract string ValueToString(T value);

	protected void RegeneratePreview()
	{
		previewValues.Clear();
		for (int i = 0; i < previewLength; i++)
		{
			T value = Evaluate(i);
			previewValues.Add(ValueToString(value)); // Format as short readable string
		}
	}

#if UNITY_EDITOR
	protected bool ShowExponent()
	{
		return functionType == FunctionType.Exponential;
	}
#endif
}
