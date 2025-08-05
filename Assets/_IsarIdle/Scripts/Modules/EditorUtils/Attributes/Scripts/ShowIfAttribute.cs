using UnityEngine;

namespace Isar.Utils
{
	public enum ConditionOperator
	{
		AND, //True if ALL conditions are true
		OR, //True if ONE of the conditions is true
	}

	public abstract class AConditionAttribute : PropertyAttribute
	{
		public ConditionOperator conditionOperator { get; protected set; }
		public string[] conditions { get; protected set; }
	}

	public class ShowIfAttribute : AConditionAttribute
	{
		public ShowIfAttribute(ConditionOperator conditionOperator = ConditionOperator.AND, params string[] conditions)
		{
			this.conditionOperator = conditionOperator;
			this.conditions = conditions;
		}

		public ShowIfAttribute(string condition)
		{
			this.conditionOperator = ConditionOperator.AND;
			this.conditions = new string[] { condition };
		}
	}

	public class HideIfAttribute : AConditionAttribute
	{
		public HideIfAttribute(ConditionOperator conditionOperator = ConditionOperator.AND, params string[] conditions)
		{
			this.conditionOperator = conditionOperator;
			this.conditions = conditions;
		}

		public HideIfAttribute(string condition)
		{
			this.conditionOperator = ConditionOperator.AND;
			this.conditions = new string[] { condition };
		}
	}
}
