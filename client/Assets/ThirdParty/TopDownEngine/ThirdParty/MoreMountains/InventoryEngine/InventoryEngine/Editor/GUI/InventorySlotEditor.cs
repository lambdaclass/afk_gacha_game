using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.InventoryEngine;

namespace MoreMountains.InventoryEngine
{
	[CustomEditor(typeof(InventorySlot))]
	public class InventorySlotEditor : UnityEditor.UI.ButtonEditor 
	{
		protected SerializedProperty _movedSprite;
		protected SerializedProperty _parentInventoryDisplay;
		protected SerializedProperty _index;
		protected SerializedProperty _slotEnabled;
		protected SerializedProperty _targetImage;
		protected SerializedProperty _targetCanvasGroup;
		protected SerializedProperty _targetRectTransform;
		protected SerializedProperty _iconRectTransform;
		protected SerializedProperty _iconImage;
		protected SerializedProperty _quantityText;
        
		protected override void OnEnable()
		{
			base.OnEnable();
            
			_movedSprite = serializedObject.FindProperty("MovedSprite");
			_parentInventoryDisplay = serializedObject.FindProperty("ParentInventoryDisplay");
			_index = serializedObject.FindProperty("Index");
			_slotEnabled = serializedObject.FindProperty("SlotEnabled");
			_targetImage = serializedObject.FindProperty("TargetImage");
			_targetCanvasGroup = serializedObject.FindProperty("TargetCanvasGroup");
			_targetRectTransform = serializedObject.FindProperty("TargetRectTransform");
			_iconRectTransform = serializedObject.FindProperty("IconRectTransform");
			_iconImage = serializedObject.FindProperty("IconImage");
			_quantityText = serializedObject.FindProperty("QuantityText");
		}
        
		public override void OnInspectorGUI() {

			base.OnInspectorGUI();
 
			EditorGUILayout.LabelField("Bindings", EditorStyles.boldLabel);
			EditorGUILayout.ObjectField(_movedSprite);
			EditorGUILayout.PropertyField(_parentInventoryDisplay);
			EditorGUILayout.PropertyField(_index);
			EditorGUILayout.PropertyField(_slotEnabled);
			EditorGUILayout.PropertyField(_targetImage);
			EditorGUILayout.PropertyField(_targetCanvasGroup);
			EditorGUILayout.PropertyField(_targetRectTransform);
			EditorGUILayout.PropertyField(_iconRectTransform);
			EditorGUILayout.PropertyField(_iconImage);
			EditorGUILayout.PropertyField(_quantityText);
 
			serializedObject.ApplyModifiedProperties();

		}
	}    
}