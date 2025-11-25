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
using AuroraFPSRuntime.SystemModules.HealthModules;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [AIBehaviourMenu("RayCast Shooting", "Shooting/RayCast Shooting")]
    public class AIRayShooingBehaviour : AIShootingBehaviour
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
        private BulletItem bullet;

        [SerializeField]
        [Foldout("Shoot Settings")]
        [MinValue(0.0f)]
        [Order(52)]
        private float impulseAmplifier = 1.0f;

        [SerializeField]
        [Foldout("Shoot Settings")]
        [Order(53)]
        private LayerMask cullingLayer = 1 << 0;

        [SerializeField]
        [Foldout("Accuracy Settings")]
        [MinValue(0.0f)]
        [Order(70)]
        private float spreadMultiplier = 0.5f;

        [SerializeField]
        [AssetSelecter(AssetType = typeof(RayTrail))]
        [Foldout("Effect Settings")]
        [Order(400)]
        private RayTrail lineRendererEffect;

        protected override void MakeShoot()
        {
            Vector3 end = CalculateDirectionWithAccuracy(firePoint.position, GetTargetCollider().bounds.center);
            RayTrail effectClone = PoolManager.GetRuntimeInstance().CreateOrPop<RayTrail>(lineRendererEffect, firePoint.position, Quaternion.LookRotation(firePoint.forward));
            if (effectClone != null)
            {
                effectClone.Visualize(firePoint.position, end);
            }

            if (Physics.Linecast(firePoint.position, end, out RaycastHit hitInfo, cullingLayer))
            {
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(bullet.GetDamage(), new DamageInfo(owner.transform, hitInfo.point, hitInfo.normal));
                }

                Rigidbody rigidbody = hitInfo.transform.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddForce(firePoint.forward * (bullet.GetImpactImpulse() + impulseAmplifier), ForceMode.Impulse);
                }

                Decal.Spawn(bullet.GetDecalMapping(), hitInfo);

                if (effectClone != null)
                {
                    effectClone.Visualize(firePoint.position, hitInfo.point);
                }
            }
        }

        /// <summary>
        /// Calculate direction from start to end point with specific accuracy.
        /// </summary>
        /// <param name="accuracy">Accuracy value [0...1]</param>
        /// <param name="origin">Start point.</param>
        /// <param name="direction">End point.</param>
        /// <returns>Direction from start to end point with specific accuracy.</returns>
        private Vector3 CalculateDirectionWithAccuracy(Vector3 origin, Vector3 end)
        {
            end.x = Random.Range(end.x - spreadMultiplier, end.x + spreadMultiplier);
            end.y = Random.Range(end.y - spreadMultiplier, end.y + spreadMultiplier);
            return end;
        }

    }
}