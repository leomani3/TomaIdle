using BreakInfinity;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestBigDouble : MonoBehaviour
{
	public BigDouble bigDouble = 1;

	[Button]
	public void Add(BigDouble value)
	{
		bigDouble += value;
	}

	[Button]
	public void Double()
	{
		bigDouble *= 2;
	}

	[Button]
	public void Log()
	{
		Debug.Log(bigDouble.Format());
	}
}
