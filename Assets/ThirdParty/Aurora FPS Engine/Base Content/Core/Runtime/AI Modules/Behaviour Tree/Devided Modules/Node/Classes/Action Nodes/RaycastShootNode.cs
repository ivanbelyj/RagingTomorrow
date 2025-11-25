/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Raycast Shoot", "Actions/Combat/Raycast Shoot")]
    [HideScriptField]
    public class RaycastShootNode : ShootNode
    {
        [SerializeField]
        [Slider(0f, 1f)]
        private float deviation;

        protected override void MakeShoot()
        {
            Vector3 direction = (GetTargetCollider().bounds.center - firePoint.position).normalized;
            direction = (direction + Random.insideUnitSphere * deviation).normalized;
            Debug.DrawRay(firePoint.position, direction * 10, Color.red, 3f);
            if (Physics.Raycast(firePoint.position, direction, out RaycastHit hitInfo))
            {
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage, new DamageInfo(owner.transform, hitInfo.point, hitInfo.normal));
                }
            }
        }
    }
}