using System;
using System.Reflection;
using UnityEditor;

namespace Isar.UI
{
    public static class SerializedPropertyExtensions
    {
		/// <summary>
		/// Reflectively pulls the actual CLR object that backs this SerializedProperty.
		/// Works for nested fields, lists/arrays, etc.
		/// </summary>
		public static object GetTargetObject(this SerializedProperty prop)
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var parts = path.Split('.');
			foreach (var part in parts)
			{
				if (part.Contains("["))
				{
					// handle array / list indexing
					var name = part.Substring(0, part.IndexOf('['));
					var index = Convert.ToInt32(part.Substring(part.IndexOf('[')).Replace("[", "").Replace("]", ""));
					obj = GetValue_AtIndex(obj, name, index);
				}
				else
				{
					obj = GetValue_FieldOrProperty(obj, part);
				}
			}
			return obj;
		}

		static object GetValue_FieldOrProperty(object src, string name)
		{
			if (src == null) return null;
			var type = src.GetType();
			// try field first
			var fi = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (fi != null) return fi.GetValue(src);
			// then property
			var pi = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (pi != null) return pi.GetValue(src, null);
			return null;
		}

		static object GetValue_AtIndex(object src, string name, int index)
		{
			var enumerable = GetValue_FieldOrProperty(src, name) as System.Collections.IEnumerable;
			if (enumerable == null) return null;
			var enm = enumerable.GetEnumerator();
			for (int i = 0; i <= index; i++) enm.MoveNext();
			return enm.Current;
		}
	}
}
