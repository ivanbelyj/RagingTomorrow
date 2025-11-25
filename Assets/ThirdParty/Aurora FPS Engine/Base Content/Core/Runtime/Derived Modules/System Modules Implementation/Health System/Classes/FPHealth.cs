/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.SystemModules.ControllerModules;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;
using AuroraFPSRuntime.SystemModules.ControllerSystems;

namespace AuroraFPSRuntime
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Health/First Person Health")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerController))]
    public partial class FPHealth : CharacterHealth
    {
        // Base character health properties.
        [SerializeField]
        [ReorderableList(
            DisplayHeader = false,
            ElementLabel = "Settings {niceIndex}",
            NoneElementLabel = "Add new settings...")]
        [Foldout("Velocity Damage Settings", Style = "Header")]
        private VelocityDamage[] velocityDamages;

        [SerializeField]
        [ReorderableList(ElementLabel = "Preset {niceIndex}", DisplayHeader = false)]
        [Foldout("Damage Shake Settings", Style = "Header")]
        private DamageShake[] damageShakes;

        // Stored required components.
        private PlayerController controller;
        private CameraShaker shaker;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<PlayerController>();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            shaker = controller.GetPlayerCamera().GetShaker();
        }

        public override void TakeDamage(float amount, DamageInfo damageInfo)
        {
            base.TakeDamage(amount, damageInfo);
            for (int i = 0; i < damageShakes.Length; i++)
            {
                DamageShake damageShake = damageShakes[i];
                if (Math.InRange(amount, damageShake.GetDamageRange()))
                {
                    shaker.RegisterShake(new KickShake(damageShake.GetShakeSettings(), damageShake.GetShakeDirection()));
                }
            }
        }

        #region [Velocity Damage Implementation]
        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun
        /// touching another rigidbody/collider.
        /// </summary>
        /// <param name="other">The Collision data associated with this collision.</param>
        protected virtual void OnCollisionEnter(Collision other)
        {
            ContactPoint contact = other.GetContact(0);
            CalculateVelocityDamage(other.relativeVelocity.magnitude, new DamageInfo(transform, contact.point, contact.normal));
        }

        /// <summary>
        /// Calculate and apply damage by velocity.
        /// </summary>
        protected virtual void CalculateVelocityDamage(float velocity, DamageInfo info)
        {
            if (enabled && velocity > 0)
            {
                for (int i = 0; i < velocityDamages.Length; i++)
                {
                    VelocityDamage property = velocityDamages[i];
                    if (Math.InRange(velocity, property.GetVelocity()))
                    {
                        TakeDamage(property.GetDamage(), info);
                    }
                }
            }
        }
        #endregion

        #region [Getter / Setter]
        public VelocityDamage[] GetVelocityDamageProperties()
        {
            return velocityDamages;
        }

        public void SetVelocityDamageProperties(VelocityDamage[] value)
        {
            velocityDamages = value;
        }
        #endregion
    }
}