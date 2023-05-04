using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// A simple component you can use to control a weapon and have it start and stop on demand, without having a character to handle it
    /// You can see it in action in the KoalaHealth demo scene, it's powering that demo's cannons
    /// </summary>
    public class WeaponHandler : TopDownMonoBehaviour
    {
        [Header("Weapon")]
        /// the weapon you want this component to pilot
        [Tooltip("the weapon you want this component to pilot")]
        public Weapon TargetWeapon;

        [Header("Debug")] 
        [MMInspectorButton("StartShooting")]
        public bool StartShootingButton;
        [MMInspectorButton("StopShooting")]
        public bool StopShootingButton;

        /// <summary>
        /// Makes the associated weapon start shooting
        /// </summary>
        public virtual void StartShooting()
        {
            if (TargetWeapon == null)
            {
                return;
            }
            TargetWeapon.WeaponInputStart();
        }

        /// <summary>
        /// Makes the associated weapon stop shooting
        /// </summary>
        public virtual void StopShooting()
        {
            if (TargetWeapon == null)
            {
                return;
            }
            TargetWeapon.WeaponInputStop();
        }
    }
}

