using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{

	[CustomEditor (typeof(CharacterAbility),true)]
	[CanEditMultipleObjects]

	/// <summary>
	/// Adds custom labels to the Character inspector
	/// </summary>

	public class CharacterAbilityInspector : Editor 
	{		
		SerializedProperty AbilityStartSfx, AbilityInProgressSfx, AbilityStopSfx, AbilityStartFeedbacks, AbilityStopFeedbacks;

		protected List<String> _propertiesToHide;
		protected bool _hasHiddenProperties = false;
		protected bool _foldout;

		protected virtual void OnEnable()
		{
			_propertiesToHide = new List<string>();

			AbilityStartSfx = this.serializedObject.FindProperty("AbilityStartSfx");
			AbilityInProgressSfx = this.serializedObject.FindProperty("AbilityInProgressSfx");
			AbilityStopSfx = this.serializedObject.FindProperty("AbilityStopSfx");
			AbilityStartFeedbacks = this.serializedObject.FindProperty("AbilityStartFeedbacks");
			AbilityStopFeedbacks = this.serializedObject.FindProperty("AbilityStopFeedbacks");

			MMHiddenPropertiesAttribute[] attributes = (MMHiddenPropertiesAttribute[])target.GetType().GetCustomAttributes(typeof(MMHiddenPropertiesAttribute), false);
			if (attributes != null)
			{
				if (attributes.Length != 0)
				{
					if (attributes[0].PropertiesNames != null)
					{
						_propertiesToHide = new List<String>(attributes[0].PropertiesNames);
						_hasHiddenProperties = true;
					}
				}
			}
		}

		/// <summary>
		/// When inspecting a Character, adds to the regular inspector some labels, useful for debugging
		/// </summary>
		public override void OnInspectorGUI()
		{
			CharacterAbility t = (target as CharacterAbility);

			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			if (t.HelpBoxText() != "")
			{
				EditorGUILayout.HelpBox(t.HelpBoxText(),MessageType.Info);
			}

			Editor.DrawPropertiesExcluding(serializedObject, new string[] { "AbilityStartSfx","AbilityInProgressSfx", "AbilityStopSfx", "AbilityStartFeedbacks", "AbilityStopFeedbacks" });

			EditorGUILayout.Space();

			EditorGUILayout.GetControlRect (true, 16f, EditorStyles.foldout);
			Rect foldRect = GUILayoutUtility.GetLastRect ();
			if (Event.current.type == EventType.MouseUp && foldRect.Contains (Event.current.mousePosition)) 
			{
				_foldout = !_foldout;
				GUI.changed = true;
				Event.current.Use ();
			}
			_foldout = EditorGUI.Foldout (foldRect, _foldout, "Ability Sounds");	      

			if (_foldout) 
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(AbilityStartSfx);
				EditorGUILayout.PropertyField(AbilityInProgressSfx);
				EditorGUILayout.PropertyField(AbilityStopSfx);
				EditorGUI.indentLevel--;
			}

			if (_propertiesToHide.Count > 0)
			{
				if (_propertiesToHide.Count < 2)
				{
					EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);
				}
				if (!_propertiesToHide.Contains("AbilityStartFeedbacks"))
				{
					EditorGUILayout.PropertyField(AbilityStartFeedbacks);
				}
				if (!_propertiesToHide.Contains("AbilityStopFeedbacks"))
				{
					EditorGUILayout.PropertyField(AbilityStopFeedbacks);
				}
			}
			else
			{
				EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(AbilityStartFeedbacks);
				EditorGUILayout.PropertyField(AbilityStopFeedbacks);
			}

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}	
	}
}