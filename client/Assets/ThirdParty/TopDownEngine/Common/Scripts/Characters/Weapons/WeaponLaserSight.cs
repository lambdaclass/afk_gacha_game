using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to a weapon and it'll project a laser ray towards the direction the weapon is facing
	/// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Laser Sight")]
	public class WeaponLaserSight : TopDownMonoBehaviour 
	{
		/// the possible modes this weapon laser sight can run on, 3D by default
		public enum Modes { TwoD, ThreeD }

		[Header("General Settings")]

		/// whether this laser should work in 2D or 3D
		[Tooltip("whether this laser should work in 2D or 3D")]
		public Modes Mode = Modes.ThreeD;
		/// if this is false, raycasts won't be computed for this laser sight
		[Tooltip("if this is false, raycasts won't be computed for this laser sight")]
		public bool PerformRaycast = true;
		/// if this is false, the laser won't be drawn
		[MMCondition("PerformRaycast")]
		[Tooltip("if this is false, the laser won't be drawn")]
		public bool DrawLaser = true;

		[Header("Raycast Settings")]

		/// the origin of the raycast used to detect obstacles
		[Tooltip("the origin of the raycast used to detect obstacles")]
		public Vector3 RaycastOriginOffset;
		/// the origin of the visible laser
		[Tooltip("the origin of the visible laser")]
		public Vector3 LaserOriginOffset;
		/// the maximum distance to which we should draw the laser
		[Tooltip("the maximum distance to which we should draw the laser")]
		public float LaserMaxDistance = 50;
		/// the collision mask containing all layers that should stop the laser
		[Tooltip("the collision mask containing all layers that should stop the laser")]
		public LayerMask LaserCollisionMask;

		[Header("Laser")]

		/// the width of the laser
		[Tooltip("the width of the laser")]
		public Vector2 LaserWidth = new Vector2(0.05f, 0.05f);
		/// the material used to render the laser
		[Tooltip("the material used to render the laser")]
		public Material LaserMaterial;

		public LineRenderer _line { get; protected set; }
		public RaycastHit _hit { get; protected set; }
		public RaycastHit2D _hit2D;
		public Vector3 _origin { get; protected set; }
		public Vector3 _raycastOrigin { get; protected set; }

		protected Vector3 _destination;
		protected Vector3 _laserOffset;
		protected Weapon _weapon;
		protected Vector3 _direction;

		protected Vector3 _weaponPosition, _thisPosition, _thisForward;
		protected Quaternion _weaponRotation, _thisRotation;
		protected int _initFrame;

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// On init we create our line if needed
		/// </summary>
		protected virtual void Initialization()
		{
			if (DrawLaser)
			{
				_line = gameObject.AddComponent<LineRenderer>();
				_line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				_line.receiveShadows = true;
				_line.startWidth = LaserWidth.x;
				_line.endWidth = LaserWidth.y;
				_line.material = LaserMaterial;
				_line.SetPosition(0, this.transform.position);
				_line.SetPosition(1, this.transform.position);
			}
			_weapon = GetComponent<Weapon>();
			if (_weapon == null)
			{
				Debug.LogWarning("This WeaponLaserSight is not associated to a weapon. Please add it to a gameobject with a Weapon component.");
			}

			_initFrame = Time.frameCount;
		}

		/// <summary>
		/// Every frame we draw our laser
		/// </summary>
		protected virtual void LateUpdate()
		{
			ShootLaser();
		}

		/// <summary>
		/// Draws the actual laser
		/// </summary>
		public virtual void ShootLaser()
		{
			if (!PerformRaycast)
			{
				return;
			}

			_laserOffset = LaserOriginOffset;
			_weaponPosition = _weapon.transform.position;
			_weaponRotation = _weapon.transform.rotation;
			_thisPosition = this.transform.position;
			_thisRotation = this.transform.rotation;
			_thisForward = this.transform.forward;
            
			if (Mode == Modes.ThreeD)
			{
				// our laser will be shot from the weapon's laser origin
				_origin = MMMaths.RotatePointAroundPivot(_thisPosition + _laserOffset, _thisPosition, _thisRotation);
				_raycastOrigin = MMMaths.RotatePointAroundPivot(_thisPosition + RaycastOriginOffset, _thisPosition, _thisRotation);

				// we cast a ray in front of the weapon to detect an obstacle
				_hit = MMDebug.Raycast3D(_raycastOrigin, _thisForward, LaserMaxDistance, LaserCollisionMask, Color.red, true);

				// if we've hit something, our destination is the raycast hit
				if (_hit.transform != null)
				{
					_destination = _hit.point;
				}
				// otherwise we just draw our laser in front of our weapon 
				else
				{
					_destination = _origin + _thisForward * LaserMaxDistance;
				}
			}
			else
			{
				_direction = _weapon.Flipped ? Vector3.left : Vector3.right;
				if (_direction == Vector3.left)
				{
					_laserOffset.x = -LaserOriginOffset.x;
				}

				_raycastOrigin = MMMaths.RotatePointAroundPivot(_weaponPosition + _laserOffset, _weaponPosition, _weaponRotation);
				_origin = _raycastOrigin;

				// we cast a ray in front of the weapon to detect an obstacle
				_hit2D = MMDebug.RayCast(_raycastOrigin, _weaponRotation * _direction, LaserMaxDistance, LaserCollisionMask, Color.red, true);
				if (_hit2D)
				{
					_destination = _hit2D.point;
				}
				// otherwise we just draw our laser in front of our weapon 
				else
				{
					_destination = _origin;
					_destination.x = _destination.x + LaserMaxDistance * _direction.x;
					_destination = MMMaths.RotatePointAroundPivot(_destination, _weaponPosition, _weaponRotation);
				}
			}

			if (Time.frameCount <= _initFrame + 1)
			{
				return;
			}

			// we set our laser's line's start and end coordinates
			if (DrawLaser)
			{
				_line.SetPosition(0, _origin);
				_line.SetPosition(1, _destination);
			}			
		}

		/// <summary>
		/// Turns the laser on or off depending on the status passed in parameters
		/// </summary>
		/// <param name="status">If set to <c>true</c> status.</param>
		public virtual void LaserActive(bool status)
		{
			_line.enabled = status;
		}

	}
}