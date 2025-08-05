using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using BreakInfinity;

[Serializable]
public class FloatBalanceFunction : ABalanceFunction<float>
{
    /// <summary>
    /// Evaluate the function at the given index and return a BigDouble.
    /// </summary>
    public override float Evaluate(int index)
    {
        return functionType switch
        {
            FunctionType.Constant => baseValue,
            FunctionType.Linear => baseValue + index * multiplier,
            FunctionType.Exponential => baseValue + Mathf.Pow(index, exponent) * multiplier,
            FunctionType.Logarithmic => baseValue + Mathf.Log(index + 1, 10) * multiplier,
            _ => baseValue,
        };
    }

	protected override string ValueToString(float value)
	{
        return value.ToString("G5");
	}


#if UNITY_EDITOR
	[OnInspectorInit]
    private void EnsurePreviewIsUpToDate()
    {
        RegeneratePreview();
    }
#endif

    public IReadOnlyList<string> Preview => previewValues;
}
