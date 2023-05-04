using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	[AddComponentMenu("TopDown Engine/Managers/Multiplayer GUIManager")]
	public class MultiplayerGUIManager : GUIManager
	{
		[Header("Multiplayer GUI")]
		/// the HUD to display when in split screen mode
		[Tooltip("the HUD to display when in split screen mode")]
		public GameObject SplitHUD;
		/// the HUD to display when in group camera mode
		[Tooltip("the HUD to display when in group camera mode")]
		public GameObject GroupHUD;
		/// a UI object used to display the splitters UI images
		[Tooltip("a UI object used to display the splitters UI images")]
		public GameObject SplittersGUI;
	}
}