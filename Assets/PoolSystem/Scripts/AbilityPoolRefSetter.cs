using Lean.Pool;
using UnityEngine;

public class AbilityPoolRefSetter : MonoBehaviour
{
	[SerializeField] private AbilityPoolRef poolRef;

	private void Awake()
	{
		poolRef.pool = GetComponent<LeanAbilityPool>();
	}
}