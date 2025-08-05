using BreakInfinity;
using System.Globalization;
using UnityEngine;

public static class NumberManagement
{
	public static string[] baseIndexToMagnitude = new string[]
	{
		"K",
		"M",
		"B",
		"T",
		"Qa",
		"Qi",
		"Sx",
		"Sp",
		"Oc",
		"No",
		"Dc",
	};
	
	public static string[] baseMagnitudes = new string[]
	{
		"Thousand",
		"Million",
		"Billion",
		"Trittion",
		"Quadrillion",
		"Quintillion",
		"Sextillion",
		"Septillion",
		"Octillion",
		"Nonillion",
		"Decillion",
	};

	private static int maxMagnitude = 36;

	// Extension to format BigDouble for incremental game UI
	public static string Format(this BigDouble value, string mantissaFormat = "G3")
	{
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		if (value.Exponent < 3)
		{
			return (value.Mantissa * Mathf.Pow(10, value.Exponent)).ToString(mantissaFormat);
		}
		else
		{
			return (value.Mantissa * Mathf.Pow(10, value.Exponent % 3)).ToString(mantissaFormat) + GetUnit(value.Exponent);
		}
	}

	public static string GetUnit(long magnitude)
	{
		if (magnitude < 3)
			return "";
		if (magnitude < maxMagnitude)
		{
			return baseIndexToMagnitude[(int)(magnitude / 3) - 1].ToString();
		}
		//Use alphabet for larger numbers. Repeat the alphabet if needed
		const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		magnitude = (magnitude - maxMagnitude) / 3 + alphabet.Length;
		string result = alphabet[(int)(magnitude % alphabet.Length)].ToString();
		while (magnitude >= alphabet.Length)
		{
			magnitude = magnitude / alphabet.Length - 1;
			result = alphabet[(int)(magnitude % alphabet.Length)] + result;
		}
		return result;
	}
}
