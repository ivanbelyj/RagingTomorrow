/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Looting/Aid Kit")]
    [DisallowMultipleComponent]
    public class AidKit : LootObject
    {
        [SerializeField]
        [MinValue(0.0f)]
        private float healthPoint = 100;

        /// <summary>
        /// Called when character being loot object.
        /// </summary>
        /// <param name="other">Reference of loot object transform.</param>
        /// <returns>The success of looting the specified object.</returns>
        protected override bool OnLoot(Transform other)
        {
            ObjectHealth health = other.GetComponent<ObjectHealth>();
            if(health != null && health.IsAlive())
            {
                health.ApplyHealth(healthPoint);
                return true;
            }
            return false;
        }

        protected override void CalculateMessageCode(Transform other, out int messageCode)
        {
            ObjectHealth health = other.GetComponent<ObjectHealth>();
            if (health != null && health.IsAlive() && health.GetHealth() < health.GetMaxHealth())
            {
                messageCode = 1;
            }
            else
            {
                messageCode = 0;
            }
        }
    }
}
