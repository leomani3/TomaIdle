using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using BreakInfinity;

[Serializable]
public class BigBalanceFunction : ABalanceFunction<BigDouble>
{
    /// <summary>
    /// Evaluate the function at the given index and return a BigDouble.
    /// </summary>
    public override BigDouble Evaluate(int index)
    {
        return functionType switch
        {
            FunctionType.Constant => baseValue,
            FunctionType.Linear => baseValue + index * multiplier,
            FunctionType.Exponential => baseValue + BigDouble.Pow(index, exponent) * multiplier,
            FunctionType.Logarithmic => baseValue + BigDouble.Log(index + 1, 10) * multiplier,
            _ => baseValue,
        };
    }

	protected override string ValueToString(BigDouble value)
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
