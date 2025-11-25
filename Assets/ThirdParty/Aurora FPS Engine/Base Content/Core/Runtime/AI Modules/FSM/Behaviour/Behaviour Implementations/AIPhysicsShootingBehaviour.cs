/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.WeaponModules;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [AIBehaviourMenu("Physics Shooting", "Shooting/Physics Shooting")]
    public sealed class AIPhysicsShootingBehaviour : AIShootingBehaviour
    {
        [SerializeField]
        [Foldout("Shoot Settings")]
        [NotNull]
        [Order(50)]
        private Transform firePoint;

        [SerializeField]
        [Foldout("Shoot Settings")]
        [NotNull]
        [Order(51)]
        private PhysicsBullet bullet;

        [SerializeField]
        [Foldout("Shoot Settings")]
        [MinValue(0.0f)]
        [Order(52)]
        private float impulseAmplifier;

        // Stored required components.
        private PoolManager poolManager;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            poolManager = PoolManager.GetRuntimeInstance();
        }

        protected override void MakeShoot()
        {
            Vector3 targetPosition = GetTargetCollider().bounds.center;
            Vector3 direction = (targetPosition - firePoint.position).normalized;
            PhysicsBullet bulletInstance = poolManager.CreateOrPop<PhysicsBullet>(bullet, firePoint.position, Quaternion.LookRotation(direction));
            bulletInstance.ApplySpeed(direction, impulseAmplifier);
        }
    }
}