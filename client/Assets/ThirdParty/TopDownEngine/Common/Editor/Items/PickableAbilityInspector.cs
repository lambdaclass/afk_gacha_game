using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEditor;

namespace MoreMountains.TopDownEngine
{
	public struct CharacterAbilityTypePair
	{
		public System.Type AbilityType;
		public string AbilityName;
	}

	[CustomEditor (typeof(PickableAbility))]
	[CanEditMultipleObjects]
	public class PickableAbilityInspector : Editor
	{
		protected static string[] _typeDisplays;
		protected List<string> _typeNames = new List<string>();
		public static List<CharacterAbilityTypePair> _typesAndNames = new List<CharacterAbilityTypePair>();

		protected SerializedProperty _abilityType;
		protected int _abilityIndex = 0;

		protected int _lastIndex = 0;
		protected string _currentTypeAsString;

		protected virtual void OnEnable()
		{
			_abilityType = serializedObject.FindProperty("AbilityType");
			PrepareAbilityList();
		}

		protected virtual void PrepareAbilityList()
		{
			_currentTypeAsString = (target as PickableAbility).AbilityTypeAsString;

			if ((_typeDisplays != null) && (_typeDisplays.Length > 0))
			{
				for (int i = 0; i < _typesAndNames.Count; i++)
				{
					if (_currentTypeAsString == _typesAndNames[i].AbilityType.Name)
					{
						_abilityIndex = i;
						_lastIndex = i;
					}
				}
				return;
			}

			// Retrieve available abilities
			List<System.Type> types = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(CharacterAbility))
				select assemblyType).ToList(); 

			// Create display list from types
			_typeNames.Clear();
			for (int i = 0; i < types.Count; i++)
			{
				CharacterAbilityTypePair _newType = new CharacterAbilityTypePair();
				_newType.AbilityType = types[i];
				_newType.AbilityName = types[i].Name;
				if ((_newType.AbilityName == "CharacterAbility") || (_newType.AbilityName == null))
				{
					continue;
				}
				_typesAndNames.Add(_newType);
			}

			_typesAndNames = _typesAndNames.OrderBy(t => t.AbilityName).ToList(); 

			for (int i = 0; i < _typesAndNames.Count; i++)
			{
				_typeNames.Add(_typesAndNames[i].AbilityName);
			}

			_typeDisplays = _typeNames.ToArray(); 
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawDefaultInspector();

			int newIndex = EditorGUILayout.Popup(_abilityIndex, _typeDisplays) ;
			if ((newIndex > 0) && (newIndex != _lastIndex))
			{
				serializedObject.Update();
				Undo.RecordObject(target, "Change selected ability");
				_lastIndex = newIndex;
				_abilityIndex = newIndex ;
				(target as PickableAbility).AbilityTypeAsString = _typesAndNames[_abilityIndex].AbilityType.Name;
				serializedObject.ApplyModifiedProperties();
				PrefabUtility.RecordPrefabInstancePropertyModifications(serializedObject.targetObject);

				PrepareAbilityList();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}    
}