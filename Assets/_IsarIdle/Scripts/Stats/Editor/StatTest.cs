using BreakInfinity;
using UnityEditor;
using UnityEngine;

public static class StatTest
{
	[MenuItem("Isar/Tests/Stat Test - Float")]
	public static void FloatTest()
	{
		Stat testStat = new Stat(10f);

		AssertValue(testStat.Value, 10f, "Initial value");
		StatModifier<float> addModifier = new StatModifier<float>(10f, ModifierType.Flat);
		testStat.AddModifier(addModifier);
		AssertValue(testStat.Value, 20f, "Adding flat modifier");
		StatModifier<float> percentModifier = new StatModifier<float>(0.2f, ModifierType.Percent);
		testStat.AddModifier(percentModifier);
		AssertValue(testStat.Value, 24f, "Adding percent modifier");
		testStat.RemoveModifier(addModifier);
		AssertValue(testStat.Value, 12f, "Removing flat modifier");
		percentModifier.UpdateValue(0.5f);
		AssertValue(testStat.Value, 15f, "Updating percent modifier");
		testStat.ClearModifiers();
		AssertValue(testStat.Value, 10f, "Clearing modifiers");

		// Test with multiple modifiers
		StatModifier<float> flatModifier1 = new StatModifier<float>(5f, ModifierType.Flat);
		StatModifier<float> flatModifier2 = new StatModifier<float>(3f, ModifierType.Flat);
		StatModifier<float> flatModifier3 = new StatModifier<float>(2f, ModifierType.Flat);
		StatModifier<float> percentModifier1 = new StatModifier<float>(0.1f, ModifierType.Percent);
		testStat.AddModifier(flatModifier1);//15
		testStat.AddModifier(flatModifier2);//18
		testStat.AddModifier(flatModifier3);//20
		testStat.AddModifier(percentModifier1);//22
		AssertValue(testStat.Value, 22f, "After adding multiple modifiers");
		testStat.RemoveModifier(flatModifier1);//15 * 1.1 = 16.5
		AssertValue(testStat.Value, 16.5f, "After removing one flat modifier");

		testStat.ClearModifiers();
		AssertValue(testStat.Value, 10f, "Clearing modifiers");

		// Test with percent modifier only
		StatModifier<float> percentModifier2 = new StatModifier<float>(0.3f, ModifierType.Percent);
		testStat.AddModifier(percentModifier2);
		AssertValue(testStat.Value, 13f, "Adding percent modifier only");
		testStat.RemoveModifier(percentModifier2);
		AssertValue(testStat.Value, 10f, "Removing percent modifier");

		// Done
		Debug.Log("DONE TEST");
	}

	[MenuItem("Isar/Tests/Stat Test - BigDouble")]
	public static void BigDoubleTest()
	{
		BigStat testStat = new BigStat(10d);

		AssertValue(testStat.Value, 10d, "Initial value");
		StatModifier<BigDouble> addModifier = new StatModifier<BigDouble>(10d, ModifierType.Flat);
		testStat.AddModifier(addModifier);
		AssertValue(testStat.Value, 20d, "Adding flat modifier");
		StatModifier<BigDouble> percentModifier = new StatModifier<BigDouble>(0.2d, ModifierType.Percent);
		testStat.AddModifier(percentModifier);
		AssertValue(testStat.Value, 24d, "Adding percent modifier");
		testStat.RemoveModifier(addModifier);
		AssertValue(testStat.Value, 12d, "Removing flat modifier");
		percentModifier.UpdateValue(0.5d);
		AssertValue(testStat.Value, 15d, "Updating percent modifier");
		testStat.ClearModifiers();
		AssertValue(testStat.Value, 10d, "Clearing modifiers");

		// Test with multiple modifiers
		StatModifier<BigDouble> flatModifier1 = new StatModifier<BigDouble>(5, ModifierType.Flat);
		StatModifier<BigDouble> flatModifier2 = new StatModifier<BigDouble>(3, ModifierType.Flat);
		StatModifier<BigDouble> flatModifier3 = new StatModifier<BigDouble>(2, ModifierType.Flat);
		StatModifier<BigDouble> percentModifier1 = new StatModifier<BigDouble>(0.1d, ModifierType.Percent);
		testStat.AddModifier(flatModifier1);//15
		testStat.AddModifier(flatModifier2);//18
		testStat.AddModifier(flatModifier3);//20
		testStat.AddModifier(percentModifier1);//22
		AssertValue(testStat.Value, 22d, "After adding multiple modifiers");
		testStat.RemoveModifier(flatModifier1);//15 * 1.1 = 16.5
		AssertValue(testStat.Value, 16.5d, "After removing one flat modifier");

		testStat.ClearModifiers();
		AssertValue(testStat.Value, 10d, "Clearing modifiers");

		// Test with percent modifier only
		StatModifier<BigDouble> percentModifier2 = new StatModifier<BigDouble>(0.3d, ModifierType.Percent);
		testStat.AddModifier(percentModifier2);
		AssertValue(testStat.Value, 13d, "Adding percent modifier only");
		testStat.RemoveModifier(percentModifier2);
		AssertValue(testStat.Value, 10d, "Removing percent modifier");

		StatModifier<BigDouble> hugePercentModifier = new StatModifier<BigDouble>(new BigDouble(1, 100000000), ModifierType.Percent);
		testStat.AddModifier(hugePercentModifier);
		AssertValue(testStat.Value, new BigDouble(10d) * new BigDouble(1, 100000000), "Adding huge percent modifier");
		testStat.RemoveModifier(hugePercentModifier);
		AssertValue(testStat.Value, 10d, "Removing huge percent modifier");

		StatModifier<BigDouble> hugeFlatModifier = new StatModifier<BigDouble>(new BigDouble(1, 123456789), ModifierType.Flat);
		testStat.AddModifier(hugeFlatModifier);
		AssertValue(testStat.Value, new BigDouble(10d) + new BigDouble(1, 123456789), "Adding huge flat modifier");
		testStat.RemoveModifier(hugeFlatModifier);
		AssertValue(testStat.Value, 10d, "Removing huge flat modifier");
		// Done
		Debug.Log("DONE TEST");
	}

	private static bool AssertValue(float value, float targetValue, string message)
	{
		if (Mathf.Abs(value - targetValue) > 0.000001f)
		{
			Debug.LogError($"Assertion failed: {message}. Expected {targetValue}, but got {value}");
			return false;
		}
		return true;
	}

	private static bool AssertValue(BigDouble value, BigDouble targetValue, string message)
	{
		if (BigDouble.Abs(value - targetValue) > 0.000001d)
		{
			Debug.LogError($"Assertion failed: {message}. Expected {targetValue}, but got {value}");
			return false;
		}
		return true;
	}
}
