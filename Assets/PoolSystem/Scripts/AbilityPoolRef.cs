using Lean.Pool;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityPoolRef", menuName = "ScriptableObjects/AbilityPoolRef")]
public class AbilityPoolRef : ScriptableObject
{
	public LeanAbilityPool pool;
}