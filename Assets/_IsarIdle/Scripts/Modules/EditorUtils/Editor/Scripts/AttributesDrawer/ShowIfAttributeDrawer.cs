using UnityEditor;
using Isar.Utils;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Isar.Editor
{
	public abstract class ADisplayAttributeDrawer : PropertyDrawer
	{
		protected static float displayDuration = 0.16f;
		Dictionary<string, bool> wasDrawn = new Dictionary<string, bool>();
		Dictionary<string, float> displayT = new Dictionary<string, float>();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (CanDraw(property))
			{
				EditorGUI.PropertyField(position, property, label, true);
				return;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			bool canDraw = CanDraw(property);
			if (!displayT.ContainsKey(property.propertyPath))
			{
				wasDrawn[property.propertyPath] = false;
				displayT[property.propertyPath] = -100;
			}
			bool validityChanged = displayT[property.propertyPath] <= 0 || canDraw != wasDrawn[property.propertyPath];

			if (validityChanged)
			{
				float timeRemaining = displayT[property.propertyPath] - Time.realtimeSinceStartup;
				if (timeRemaining > 0f)
				{
					displayT[property.propertyPath] = Time.realtimeSinceStartup + (displayDuration - timeRemaining);
				}
				else if (displayT[property.propertyPath] <= 0)
				{
					displayT[property.propertyPath] = Time.realtimeSinceStartup;
				}
				else
				{
					displayT[property.propertyPath] = Time.realtimeSinceStartup + displayDuration;
				}
			}
			wasDrawn[property.propertyPath] = canDraw;

			float maxHeight = base.GetPropertyHeight(property, label);
			float targetHeight = canDraw ? maxHeight : 0f;

			if (displayT[property.propertyPath] > Time.realtimeSinceStartup)
			{
				float p = 1f - (displayT[property.propertyPath] - Time.realtimeSinceStartup) / displayDuration;
				if (canDraw)
				{
					targetHeight = Mathf.Lerp(0, maxHeight, p);
				}
				else
				{
					targetHeight = Mathf.Lerp(maxHeight, 0, p);
				}
			}

			return targetHeight;
		}

		protected bool IsValid(SerializedProperty property)
		{
			if (property == null || property.serializedObject == null)
			{
				return false;
			}
			AConditionAttribute attribute = (AConditionAttribute)this.attribute;
			if (attribute == null)
			{
				return false;
			}
			ConditionOperator conditionOperator = attribute.conditionOperator;
			bool targetPattern = conditionOperator == ConditionOperator.AND ? false : true;
			bool unknown = true;
			object parentObject = GetParentObject(property);

			foreach (var condition in attribute.conditions)
			{
				unknown = true;
				var conditionField = GetField(parentObject, property, condition);
				if (conditionField != null && conditionField.FieldType == typeof(bool))
				{
					unknown = false;
					if ((bool)conditionField.GetValue(parentObject) == targetPattern)
						return targetPattern;
				}

				var conditionMethod = GetMethod(parentObject, property, condition);
				if (conditionMethod != null && conditionMethod.ReturnType == typeof(bool) && conditionMethod.GetParameters().Length == 0)
				{
					unknown = false;
					if ((bool)conditionMethod.Invoke(parentObject, null) == targetPattern)
						return targetPattern;
				}

				if (unknown)
				{
					Debug.LogWarning($"ConditionAttribute: Unknown condition '{condition}' in {parentObject.GetType().FullName}. Please check the method or field name.");
				}
			}

			return !targetPattern;
		}

		protected static object GetParentObject(SerializedProperty property)
		{
			string propertyPath = property.propertyPath;
			propertyPath = propertyPath.Substring(0, propertyPath.LastIndexOf('.'));
			SerializedObject serializedObject = property.serializedObject;
			SerializedProperty parentProp = serializedObject.FindProperty(propertyPath);
			object p = parentProp.boxedValue;
			return p;
		}

		protected static MethodInfo GetMethod(object target, SerializedProperty property, string methodName)
		{
			var methodInfos = target.GetType()
				.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(m => m.Name.Equals(methodName, StringComparison.InvariantCulture));

			return methodInfos.FirstOrDefault();
		}

		protected static FieldInfo GetField(object target, SerializedProperty property, string fieldName)
		{
			var types = new List<Type>()
			{
				target.GetType(),
			};

			while (types.Last().BaseType != null)
			{
				types.Add(types.Last().BaseType);
			}

			for (var i = types.Count - 1; i >= 0; i--)
			{
				var fieldInfos = types[i]
					.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.Where(f => f.Name.Equals(fieldName, StringComparison.InvariantCulture));

				foreach (var fieldInfo in fieldInfos)
				{
					return fieldInfo;
				}
			}
			return default;
		}

		protected abstract bool CanDraw(SerializedProperty property);
	}

	[CustomPropertyDrawer(typeof(ShowIfAttribute), true)]
	public class ShowIfAttributeDrawer : ADisplayAttributeDrawer
	{
		protected override bool CanDraw(SerializedProperty property)
		{
			return IsValid(property);
		}
	}

	[CustomPropertyDrawer(typeof(HideIfAttribute), true)]
	public class HideIfAttributeDrawer : ADisplayAttributeDrawer
	{
		protected override bool CanDraw(SerializedProperty property)
		{
			return !IsValid(property);
		}
	}
}
