/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Bullet Shell System/Bullet Shell")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public class BulletShell : PoolObject
    {
        // Stored required component.
        private new Rigidbody rigidbody;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody>();

            Collider shellCollider = GetComponent<Collider>();

            GameObject playerController = GameObject.FindGameObjectWithTag(TNC.Player);
            Collider playerCollider = playerController.GetComponent<Collider>();

            Physics.IgnoreCollision(shellCollider, playerCollider, true);
        }

        /// <summary>
        /// Throw this bullet shell.
        /// </summary>
        /// <param name="direction">Throw direction.</param>
        /// <param name="force">Throw force.</param>
        public virtual void Throw(Vector3 direction, float force)
        {
            rigidbody.AddForce(direction * force, ForceMode.Impulse);
            rigidbody.AddTorque(direction * force, ForceMode.Impulse);
        }

        #region [Getter / Setter]
        public Rigidbody GetRigidbody()
        {
            return rigidbody;
        }
        #endregion
    }
}