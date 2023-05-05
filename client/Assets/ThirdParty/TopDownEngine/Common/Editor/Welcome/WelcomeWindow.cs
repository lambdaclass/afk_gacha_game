using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace MoreMountains.TopDownEngine
{
	public class WelcomeWindow : EditorWindow
	{
		public Texture2D WelcomeBanner;

		public const string PostProcessingPackageID = "com.unity.postprocessing";
		public static string PostProcessingPackageVersionID = "com.unity.postprocessing@2.1.3";
		public static string CinemachinePackageID = "com.unity.cinemachine";
		public static string CinemachinePackageVersionID = "com.unity.cinemachine@2.3.3";
		public static string PixelPerfectPackageID = "com.unity.2d.pixel-perfect";
		public static string PixelPerfectPackageVersionID = "com.unity.2d.pixel-perfect@1.0.1-preview";
		public const string DOCUMENTATION_URL = "https://topdown-engine-docs.moremountains.com/";
        
		private static readonly int WelcomeWindowWidth = 500;
		private static readonly int WelcomeWindowHeight = 700;
        
		private static GUIStyle _largeTextStyle;
		public static string RelativePath = "";

		public static GUIStyle LargeTextStyle
		{
			get
			{
				if (_largeTextStyle == null)
				{
					_largeTextStyle = new GUIStyle(UnityEngine.GUI.skin.label)
					{
						richText = true,
						wordWrap = true,
						fontStyle = FontStyle.Bold,                        
						fontSize = 14,
						alignment = TextAnchor.MiddleLeft,
						padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 }
					};
				}
				return _largeTextStyle;
			}
		}

		private static GUIStyle _regularTextStyle;
		public static GUIStyle RegularTextStyle
		{
			get
			{
				if (_regularTextStyle == null)
				{
					_regularTextStyle = new GUIStyle(UnityEngine.GUI.skin.label)
					{
						richText = true,
						wordWrap = true,
						fontStyle = FontStyle.Normal,
						fontSize = 12,
						alignment = TextAnchor.MiddleLeft,
						padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 }
					};
				}
				return _regularTextStyle;
			}
		}

		private static GUIStyle _footerTextStyle;
		public static GUIStyle FooterTextStyle
		{
			get
			{
				if (_footerTextStyle == null)
				{
					_footerTextStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
					{
						alignment = TextAnchor.LowerCenter,
						wordWrap = true,
						fontSize = 12
					};
				}

				return _footerTextStyle;
			}
		}

		[MenuItem("Tools/More Mountains/Welcome to the TopDown Engine", false, 0)]
		public static void ShowWindow()
		{
			EditorWindow editorWindow = GetWindow(typeof(WelcomeWindow), false, " Welcome", true);
			editorWindow.autoRepaintOnSceneChange = true;
			editorWindow.titleContent.image = EditorGUIUtility.IconContent("_Help").image;
			editorWindow.maxSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
			editorWindow.minSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
			editorWindow.position = new Rect(Screen.width / 2 + WelcomeWindowWidth / 2, Screen.height / 2, WelcomeWindowWidth, WelcomeWindowHeight);         
			editorWindow.Show();
		}
        
		#if  UNITY_EDITOR
		private void OnGUI()
		{
			if (EditorApplication.isCompiling)
			{
				this.ShowNotification(new GUIContent("Compiling Scripts", EditorGUIUtility.IconContent("BuildSettings.Editor").image)); 
			}
			else
			{
				this.RemoveNotification();
			}

			if (RelativePath == "")
			{
				string editorPath = "Common/Editor/Welcome/";
				MonoScript welcomeWindowScript = MonoScript.FromScriptableObject(this);
				string welcomeWindowScriptFullPath = AssetDatabase.GetAssetPath(welcomeWindowScript);
				int welcomeWindowScriptFullPathIndex = welcomeWindowScriptFullPath.IndexOf(editorPath);
				RelativePath = welcomeWindowScriptFullPath.Substring(0, welcomeWindowScriptFullPathIndex);
			}            

			Rect welcomeImageRect = new Rect(0, 0, 500, 200);
			UnityEngine.GUI.DrawTexture(welcomeImageRect, WelcomeBanner);
			GUILayout.Space(220);
            
			GUILayout.BeginArea(new Rect(EditorGUILayout.GetControlRect().x + 10, 220, WelcomeWindowWidth - 20, WelcomeWindowHeight));

			EditorGUILayout.LabelField("Welcome to the TopDown Engine!\n"
				, RegularTextStyle);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("GETTING STARTED", LargeTextStyle);
			EditorGUILayout.LabelField("You can start by having a look at the documentation, or open the Demos folder and start looking at all the engine's features. Have fun with your project!", RegularTextStyle);

			EditorGUILayout.Space();
			if (GUILayout.Button(new GUIContent(" Open Documentation", EditorGUIUtility.IconContent("_Help").image), GUILayout.MaxWidth(185f)))
			{
				Application.OpenURL(DOCUMENTATION_URL);
			}

			GUILayout.EndArea();
            
			Rect areaRect = new Rect(0, WelcomeWindowHeight - 20, WelcomeWindowWidth, WelcomeWindowHeight - 20);
			GUILayout.BeginArea(areaRect);
			EditorGUILayout.LabelField("Copyright © 2018 More Mountains", FooterTextStyle);
			GUILayout.EndArea();

		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
		#endif
	}
}