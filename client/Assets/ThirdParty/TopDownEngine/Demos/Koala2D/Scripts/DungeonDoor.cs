using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A class used in the Koala demo to handle the dungeon doors
	/// </summary>
	[ExecuteInEditMode]
	public class DungeonDoor : TopDownMonoBehaviour
	{
		/// the possible states of the door
		public enum DoorStates { Open, Closed }

		[Header("Bindings)")]
		/// the top part of the door
		[Tooltip("the top part of the door")]
		public GameObject OpenDoorTop;
		/// the bottom part of the door
		[Tooltip("the bottom part of the door")]
		public GameObject OpenDoorBottom;
		/// the object to show when the door is closed
		[Tooltip("the object to show when the door is closed")]
		public GameObject ClosedDoor;

		[Header("State")]
		/// the current state of the door
		[Tooltip("the current state of the door")]
		public DoorStates DoorState = DoorStates.Open;

		/// a test button to toggle the door open or closed
		[MMInspectorButton("ToggleDoor")]
		public bool ToogleDoorButton;
		/// a test button to open the door 
		[MMInspectorButton("OpenDoor")]
		public bool OpenDoorButton;
		/// a test button to close the door
		[MMInspectorButton("CloseDoor")]
		public bool CloseDoorButton;

		/// <summary>
		/// On Start we initialize our door based on its initial status
		/// </summary>
		protected virtual void Start()
		{
			if (DoorState == DoorStates.Open)
			{
				SetDoorOpen();                
			}
			else
			{
				SetDoorClosed();
			}
		}

		/// <summary>
		/// Opens the door
		/// </summary>
		public virtual void OpenDoor()
		{
			DoorState = DoorStates.Open;
		}

		/// <summary>
		/// Closes the door
		/// </summary>
		public virtual void CloseDoor()
		{
			DoorState = DoorStates.Closed;
		}

		/// <summary>
		/// Opens or closes the door based on its current status
		/// </summary>
		public virtual void ToggleDoor()
		{
			if (DoorState == DoorStates.Open)
			{
				DoorState = DoorStates.Closed;
			}
			else
			{
				DoorState = DoorStates.Open;
			}
		}

		/// <summary>
		/// On Update, we open or close the door if needed
		/// </summary>
		protected virtual void Update()
		{
			if ((OpenDoorBottom == null) || (OpenDoorTop == null) || (ClosedDoor == null))
			{
				return;
			}

			if (DoorState == DoorStates.Open)
			{
				if (!OpenDoorBottom.activeInHierarchy)
				{
					SetDoorOpen();
				}                
			}
			else
			{
				if (!ClosedDoor.activeInHierarchy)
				{
					SetDoorClosed();
				}
			}
		}

		/// <summary>
		/// Closes the door
		/// </summary>
		protected virtual void SetDoorClosed()
		{
			ClosedDoor.SetActive(true);
			OpenDoorBottom.SetActive(false);
			OpenDoorTop.SetActive(false);
		}

		/// <summary>
		/// Opens the door
		/// </summary>
		protected virtual void SetDoorOpen()
		{
			OpenDoorBottom.SetActive(true);
			OpenDoorTop.SetActive(true);
			ClosedDoor.SetActive(false);
		}
	}
}