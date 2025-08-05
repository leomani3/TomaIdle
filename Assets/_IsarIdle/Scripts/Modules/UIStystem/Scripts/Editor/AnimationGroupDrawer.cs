using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Isar.UI
{
    [CustomPropertyDrawer(typeof(AnimationGroup))]
    public class AnimationGroupDrawer : PropertyDrawer
	{
		static List<Type> AAnimationTypes = new List<Type>();
		static List<string> AAnimationNames = new List<string>();
		int selectedAnimationType = 0;
		SerializedProperty targetProperty;
		AnimationGroup targetAnimationGroup;
		UnityEngine.Object targetObject;

		List<bool> showFoldouts = new List<bool>();
		List<bool> hideFoldouts = new List<bool>();

		bool showFoldout = false;
		bool hideFoldout = false;
		float maxShowDuration = -1f;
		float maxHideDuration = -1f;

		private static string CustomComponentCopyBuffer
		{
			get => EditorPrefs.GetString("CustomComponentCopyBuffer", string.Empty);
			set => EditorPrefs.SetString("CustomComponentCopyBuffer", value);
		}

		private void Init()
		{
			if (AAnimationTypes.Count > 0 && AAnimationNames.Count == AAnimationTypes.Count)
				return;
			//Using reflection, get all classes that inherit from AUIAnimation
			AAnimationTypes.Clear();
			AAnimationNames.Clear();
			Type[] types = typeof(AAnimation).Assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsSubclassOf(typeof(AAnimation)) && !type.IsAbstract)
				{
					AnimationAttribute attribute = (AnimationAttribute)Attribute.GetCustomAttribute(type, typeof(AnimationAttribute));
					if (attribute != null)
					{
						AAnimationNames.Add(attribute.DisplayName);
					}
					else
					{
						AAnimationNames.Add("Unknown/" + type.Name);
					}
					AAnimationTypes.Add(type);
				}
			}
			selectedAnimationType = AAnimationTypes.Count > 0 ? Mathf.Clamp(selectedAnimationType, 0, AAnimationTypes.Count - 1) : 0;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Init();
			bool guiEnabled = GUI.enabled;
			EditorGUI.BeginProperty(position, label, property);
			targetProperty = property;
			targetAnimationGroup = (AnimationGroup)property.GetTargetObject();
			targetObject = property.serializedObject.targetObject;

			SerializedProperty showAnimations = property.FindPropertyRelative("showAnimations");
			SerializedProperty hideAnimations = property.FindPropertyRelative("hideAnimations");
			SerializedProperty showDuration = property.FindPropertyRelative("showDuration");
			SerializedProperty hideDuration = property.FindPropertyRelative("hideDuration");
			SerializedProperty isInAnimation = property.FindPropertyRelative("isInAnimation");
			SerializedProperty isInShowAnimation = property.FindPropertyRelative("isInShowAnimation");
			SerializedProperty isInHideAnimation = property.FindPropertyRelative("isInHideAnimation");

			GUIStyle foldoutHeaderStyle = new GUIStyle(EditorStyles.foldoutHeader);
			maxShowDuration = -1;
			maxHideDuration = -1;

			//SHOW
			string title = showAnimations.displayName + " (" + (showAnimations.arraySize == 0 ? "None" : showAnimations.arraySize) + ")";
			if (isInShowAnimation.boolValue)
				title += " - PLAYING";
			showFoldout = EditorGUILayout.Foldout(showFoldout, title, true, foldoutHeaderStyle);

			if (showFoldout)
			{
				EditorGUILayout.BeginHorizontal();
				GUI.enabled = false;
				EditorGUILayout.LabelField("Duration: " + showDuration.floatValue.ToString("F2") + "s");
				GUI.enabled = guiEnabled;
				EditorGUILayout.EndHorizontal();
			}

			GUI.enabled = guiEnabled && !isInAnimation.boolValue;
			if (showAnimations.arraySize > 0)
				maxShowDuration = DrawAnimations(showAnimations, targetAnimationGroup.ShowAnimations, showFoldouts, showFoldout);
			else
			{
				maxShowDuration = 0;
			}
			if (showFoldout)
				DrawSelector(showAnimations, showFoldouts);

			//HIDE
			title = hideAnimations.displayName + " (" + (hideAnimations.arraySize == 0 ? "None" : hideAnimations.arraySize) + ")";
			if (isInHideAnimation.boolValue)
				title += " - PLAYING";
			hideFoldout = EditorGUILayout.Foldout(hideFoldout, title, true, foldoutHeaderStyle);

			if (hideFoldout)
			{
				EditorGUILayout.BeginHorizontal();
				GUI.enabled = false;
				EditorGUILayout.LabelField("Duration: " + hideDuration.floatValue.ToString("F2") + "s");
				GUI.enabled = guiEnabled;
				EditorGUILayout.EndHorizontal();
			}

			GUI.enabled = guiEnabled && !isInAnimation.boolValue;
			if (hideAnimations.arraySize > 0)
				maxHideDuration = DrawAnimations(hideAnimations, targetAnimationGroup.HideAnimations, hideFoldouts, hideFoldout);
			else
			{
				maxHideDuration = 0;
			}
			if (hideFoldout)
				DrawSelector(hideAnimations, hideFoldouts);


			UpdateDurations(showDuration, hideDuration);
			GUI.enabled = guiEnabled;

			EditorGUI.EndProperty();
			GUI.enabled = guiEnabled;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return 0;
		}

		private float DrawAnimations(SerializedProperty animations, List<AAnimation> referenceAnimations, List<bool> animFoldouts, bool show)
		{
			float duration = 0f;
			List<int> elementsToRemove = new List<int>();
			//Draw
			if (show && animations.arraySize > 0)
				EditorGUILayout.BeginVertical("box");
			else
				EditorGUILayout.BeginVertical();

			while (animFoldouts.Count < animations.arraySize)
				animFoldouts.Add(false);
			while (animFoldouts.Count > animations.arraySize)
				animFoldouts.RemoveAt(animFoldouts.Count - 1);
			int drawn = 0;
			if (animations != null) //Still check when foldout is false, to calculate max durations
			{
				for (int i = 0; i < animations.arraySize; i++)
				{
					SerializedProperty property = animations.GetArrayElementAtIndex(i);
					if (property == null)
					{
						elementsToRemove.Add(i);
						continue;
					}
					if (i > referenceAnimations.Count - 1)
					{
						elementsToRemove.Add(i);
						continue;
					}
					/*
					if (i > animationTarget.animations.Count - 1)
					{
						elementsToRemove.Add(i);
						continue;
					}
					*/

					//AAnimation animation = animationTarget.animations[i];

					AAnimation animation = property.managedReferenceValue as AAnimation;

					if (animation == null)
					{
						elementsToRemove.Add(i);
						continue;
					}

					duration = Mathf.Max(duration, animation.PlayDuration + animation.PlayDelay);
					if (!show)
					{
						continue;
					}

					Rect rect = EditorGUILayout.BeginVertical("box");
					//if clicked on rect, toggle foldout
					if (drawn % 2 == 0)
					{
						EditorGUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.1f));
					}
					EditorGUILayout.Space(5);
					rect = EditorGUILayout.BeginVertical();
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space(15);
					var managed = property.managedReferenceValue;
					string typeName = managed != null
						? managed.GetType().Name
						: "Null Reference";

					animFoldouts[i] = EditorGUILayout.Foldout(animFoldouts[i], typeName, true);

					//GUILayout.Label(typeName, EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Remove"))
					{
						Undo.RecordObject(targetObject, "Remove Animation");
						elementsToRemove.Add(i);
					}
					else if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
					{
						if (Event.current.keyCode == KeyCode.Mouse0)
						{
							animFoldouts[i] = !animFoldouts[i];
							Event.current.Use();
						}
						else if (Event.current.keyCode == KeyCode.Mouse1)
						{

							//Context menu to copy animation and paste it if already in clipboard
							GenericMenu menu = new GenericMenu();
							menu.AddItem(new GUIContent("Copy"), false, () =>
							{
								//Serialize the animation
								CopySerializer copySerializer = new CopySerializer(animation);
								string json = JsonUtility.ToJson(copySerializer);

								//Copy the string to the clipboard
								CustomComponentCopyBuffer = json;
							});

							string json = CustomComponentCopyBuffer;
							bool isJsonValid = !string.IsNullOrEmpty(json) && json.StartsWith("{") && json.EndsWith("}");
							CopySerializer copySerializer = null;
							if (isJsonValid)
							{
								//Check if the json can be deserialized to a AUIAnimation
								copySerializer = JsonUtility.FromJson<CopySerializer>(json);
								isJsonValid = copySerializer != null;
							}

							if (isJsonValid)
							{
								int index = i;
								menu.AddItem(new GUIContent("Paste"), false, () =>
								{
									Undo.RecordObject(targetObject, "Paste Animation");
									animation = copySerializer.Deserialize();

									animations.GetArrayElementAtIndex(index).managedReferenceValue = animation;
									referenceAnimations[index] = animation;
								});
							}
							else
							{
								menu.AddDisabledItem(new GUIContent("Paste"));
							}
							menu.ShowAsContext();

							Event.current.Use();
						}
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();

					if (animFoldouts[i])
					{
						EditorGUI.indentLevel++;
						DrawSerializeReferenceElement(property);
						EditorGUI.indentLevel--;
					}
					EditorGUILayout.Space(5);
					EditorGUILayout.EndVertical();
					drawn++;
				}
			}

			if (elementsToRemove.Count > 0)
			{
				for (int i = elementsToRemove.Count - 1; i >= 0; i--)
				{
					animations.DeleteArrayElementAtIndex(elementsToRemove[i]);
					animFoldouts.RemoveAt(elementsToRemove[i]);
				}
			}
			EditorGUILayout.Space(5);
			EditorGUILayout.EndVertical();
			return duration;
		}

		private void DrawSelector(SerializedProperty animations, List<bool> animFoldouts)
		{
			EditorGUILayout.BeginHorizontal();
			//Popup to select animation type
			selectedAnimationType = EditorGUILayout.Popup(selectedAnimationType, AAnimationNames.ToArray());
			if (GUILayout.Button("Add Animation"))
			{
				if (selectedAnimationType >= 0 && selectedAnimationType < AAnimationTypes.Count)
				{
					Undo.RecordObject(targetObject, "Add Animation");
					Type type = AAnimationTypes[selectedAnimationType];
					AAnimation animation = (AAnimation)Activator.CreateInstance(type);
					animation.OnAdd(targetObject);

					animations.InsertArrayElementAtIndex(animations.arraySize);
					SerializedProperty property = animations.GetArrayElementAtIndex(animations.arraySize - 1);
					property.managedReferenceValue = animation;
					animFoldouts.Add(true);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawSerializeReferenceElement(SerializedProperty element)
		{
			// Iterate all of its visible children:
			var iter = element.Copy();
			// Move into the first child
			bool hasChild = iter.NextVisible(true);
			int startDepth = iter.depth;

			// Keep drawing until we go back up above startDepth
			while (hasChild && iter.depth > startDepth - 1)
			{
				EditorGUILayout.PropertyField(iter, true);
				hasChild = iter.NextVisible(false);
			}
		}

		private void UpdateDurations(SerializedProperty showDuration, SerializedProperty hideDuration)
		{
			if (maxShowDuration >= 0f)
				showDuration.floatValue = maxShowDuration;
			if (maxHideDuration >= 0f)
				hideDuration.floatValue = maxHideDuration;
		}

		public class CopySerializer
		{
			public string typeString;
			public string serializedString;

			public CopySerializer(AAnimation animation)
			{
				Type type = animation.GetType();
				typeString = type.AssemblyQualifiedName;
				serializedString = JsonUtility.ToJson(animation);
			}

			public AAnimation Deserialize()
			{
				Type type = Type.GetType(typeString);
				AAnimation animation = (AAnimation)Activator.CreateInstance(type);
				JsonUtility.FromJsonOverwrite(serializedString, animation);
				return animation;
			}
		}
	}
}
