using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Rendering;

namespace MoreMountains.TopDownEngine
{

	[CustomEditor (typeof(Character))]
	[CanEditMultipleObjects]

	/// <summary>
	/// Adds custom labels to the Character inspector
	/// </summary>

	public class CharacterInspector : Editor 
	{		
		public enum Modes { TwoD, ThreeD }

		void onEnable()
		{
			// nothing
		}
		
		/// <summary>
		/// When inspecting a Character, adds to the regular inspector some labels, useful for debugging
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			Character character = (Character)target;


			// adds movement and condition states
			if (character.CharacterState!=null)
			{
				EditorGUILayout.LabelField("Movement State",character.MovementState.CurrentState.ToString());
				EditorGUILayout.LabelField("Condition State",character.ConditionState.CurrentState.ToString());
			}

			// auto completes the animator
			if (character.CharacterAnimator == null)
			{
				if (character.GetComponent<Animator>() != null)
				{
					character.CharacterAnimator = character.GetComponent<Animator>();
				}
			}

			// draws the default inspector if in Player mode
			if (character.CharacterType == Character.CharacterTypes.Player)
			{
				DrawDefaultInspector();
			}

			// in AI mode draws everything but the PlayerID field
			if (character.CharacterType == Character.CharacterTypes.AI)
			{
				character.PlayerID = "";
				Editor.DrawPropertiesExcluding(serializedObject, new string[] { "PlayerID" });
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Autobuild", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("The Character Autobuild button will automatically add all the components needed for a functioning Character, and set their settings, layer, tags. Be careful, if you've already customized your character, this will reset its settings!", MessageType.Warning, true);
			if (GUILayout.Button("AutoBuild Player Character 2D"))
			{
				GenerateCharacter(Character.CharacterTypes.Player, Modes.TwoD);
			}
			if (GUILayout.Button("AutoBuild Player Character 3D"))
			{
				GenerateCharacter(Character.CharacterTypes.Player, Modes.ThreeD);
			}
			if (GUILayout.Button("AutoBuild AI Character 2D"))
			{
				GenerateCharacter(Character.CharacterTypes.AI, Modes.TwoD);
			}
			if (GUILayout.Button("AutoBuild AI Character 3D"))
			{
				GenerateCharacter(Character.CharacterTypes.AI, Modes.ThreeD);
			}
				
			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Adds all the possible components to a character
		/// </summary>
		protected virtual void GenerateCharacter(Character.CharacterTypes type, Modes mode)
		{
			Character character = (Character)target;

			Debug.LogFormat(character.name + " : Character Autobuild Start");

			if (type == Character.CharacterTypes.Player)
			{
				character.CharacterType = Character.CharacterTypes.Player;
				// sets the layer
				character.gameObject.layer = LayerMask.NameToLayer("Player");
				// sets the tag
				character.gameObject.tag = "Player";
				// sets the player ID
				character.PlayerID = "Player1";
			}

			if (type == Character.CharacterTypes.AI)
			{
				character.CharacterType = Character.CharacterTypes.AI;
				// sets the layer
				character.gameObject.layer = LayerMask.NameToLayer("Enemies");
			}

			if (mode == Modes.TwoD)
			{
				// Adds the rigidbody2D
				Rigidbody2D rigidbody2D = (character.GetComponent<Rigidbody2D>() == null) ? character.gameObject.AddComponent<Rigidbody2D>() : character.GetComponent<Rigidbody2D>();
				rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
				rigidbody2D.simulated = true;
				rigidbody2D.useAutoMass = false;
				rigidbody2D.mass = 1;
				rigidbody2D.drag = 1;
				rigidbody2D.angularDrag = 0.05f;
				rigidbody2D.gravityScale = 0;
				rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
				rigidbody2D.sleepMode = RigidbodySleepMode2D.StartAwake;
				rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
				rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

				SortingGroup sortingGroup = (character.GetComponent<SortingGroup>() == null) ? character.gameObject.AddComponent<SortingGroup>() : character.GetComponent<SortingGroup>();
				sortingGroup.sortingLayerName = "Characters";

				// Adds the boxcollider2D if needed
				BoxCollider2D boxcollider2D = (character.GetComponent<BoxCollider2D>() == null) ? character.gameObject.AddComponent<BoxCollider2D>() : character.GetComponent<BoxCollider2D>();
				boxcollider2D.isTrigger = false;

				// adds the top down controller 2D
				TopDownController2D topDownController2D = (character.GetComponent<TopDownController2D>() == null) ? character.gameObject.AddComponent<TopDownController2D>() : character.GetComponent<TopDownController2D>();
				topDownController2D.Gravity = -30;                
				topDownController2D.GroundLayerMask = LayerMask.GetMask("Ground");
				topDownController2D.HoleLayerMask = LayerMask.GetMask("Hole");

				// adds 2D specific components
				if (character.GetComponent<CharacterOrientation2D>() == null) { character.gameObject.AddComponent<CharacterOrientation2D>(); }
				if (character.GetComponent<CharacterDash2D>() == null) { character.gameObject.AddComponent<CharacterDash2D>(); }
				if (character.GetComponent<CharacterJump2D>() == null) { character.gameObject.AddComponent<CharacterJump2D>(); }
			}

			if (mode == Modes.ThreeD)
			{
				// adds a character controller
				CharacterController characterController = (character.GetComponent<CharacterController>() == null) ? character.gameObject.AddComponent<CharacterController>() : character.GetComponent<CharacterController>();
				characterController.slopeLimit = 45f;
				characterController.stepOffset = 0.3f;
				characterController.skinWidth = 0.08f;
				characterController.minMoveDistance = 0.001f;
				characterController.radius = 0.5f;

				// adds a rigidbody
				Rigidbody rigidbody = (character.GetComponent<Rigidbody>() == null) ? character.gameObject.AddComponent<Rigidbody>() : character.GetComponent<Rigidbody>();
				rigidbody.mass = 1;
				rigidbody.drag = 0;
				rigidbody.angularDrag = 0.05f;
				rigidbody.interpolation = RigidbodyInterpolation.None;
				rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
				rigidbody.useGravity = true;
				rigidbody.isKinematic = true;

				// adds the top down controller 3D
				TopDownController3D topDownController3D = (character.GetComponent<TopDownController3D>() == null) ? character.gameObject.AddComponent<TopDownController3D>() : character.GetComponent<TopDownController3D>();
				topDownController3D.Gravity = 40;
				topDownController3D.ObstaclesLayerMask = LayerMask.GetMask("Obstacles", "Ground", "ObstaclesDoors", "MovingPlatform", "FallingPlatform");

				// adds 3D specific components
				if (character.GetComponent<CharacterOrientation3D>() == null) { character.gameObject.AddComponent<CharacterOrientation3D>(); }
				if (character.GetComponent<CharacterCrouch>() == null) { character.gameObject.AddComponent<CharacterCrouch>(); }
				if (character.GetComponent<CharacterJump3D>() == null) { character.gameObject.AddComponent<CharacterJump3D>(); }
				if (character.GetComponent<CharacterDash3D>() == null) { character.gameObject.AddComponent<CharacterDash3D>(); }
			}

			// adds components common to 2D and 3D
			if (character.GetComponent<CharacterMovement>() == null) { character.gameObject.AddComponent<CharacterMovement>(); }
			if (character.GetComponent<CharacterRun>() == null) { character.gameObject.AddComponent<CharacterRun>(); }

			// adds (usually) player specific components
			if (type == Character.CharacterTypes.Player)
			{
				if (character.GetComponent<CharacterButtonActivation>() == null) { character.gameObject.AddComponent<CharacterButtonActivation>(); }
				if (character.GetComponent<CharacterPause>() == null) { character.gameObject.AddComponent<CharacterPause>(); }
				if (character.GetComponent<CharacterTimeControl>() == null) { character.gameObject.AddComponent<CharacterTimeControl>(); }
			}

			// adds health
			Health health = (character.GetComponent<Health>() == null) ? character.gameObject.AddComponent<Health>() : character.GetComponent<Health>();
			health.MaximumHealth = 100;
			health.CurrentHealth = 100;
            
			Debug.LogFormat(character.name + " : Character Autobuild Complete");
		}
	}
}