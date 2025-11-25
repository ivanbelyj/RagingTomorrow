/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using System;
using UnityEngine;
using Math = AuroraFPSRuntime.CoreModules.Mathematics.Math;

namespace AuroraFPSRuntime.AIModules.Vision
{
    [HideScriptField]
    [AddComponentMenu(null)]
    public abstract class FieldOfView : MonoBehaviour, IVisionTarget, IVisionCallback
    {
        #region [IVisionTarget Implementation]
        public abstract IReadOnlyList<Transform> GetVisibleTargets();

        /// <summary>
        /// Get first target of visible targets.
        /// </summary>
        public virtual Transform GetFirstTarget()
        {
            if(GetVisibleTargets() != null && GetVisibleTargets().Count > 0)
            {
                return GetVisibleTargets()[0];
            }
            return null;
        }

        /// <summary>
        /// Get most nearest target of visible targets.
        /// </summary>
        public virtual Transform GetNearestTarget()
        {
            int bestDistanceIndex = -1;
            float bestDistance = Mathf.Infinity;
            for (int i = 0; i < GetVisibleTargets().Count; i++)
            {
                Transform target = GetVisibleTargets()[i];
                float distance = Math.Distance2D(transform.position, target.position);
                if(distance < bestDistance)
                {
                    bestDistanceIndex = i;
                    bestDistance = distance;
                }
            }

            if(bestDistanceIndex >= 0)
            {
                return GetVisibleTargets()[bestDistanceIndex];
            }
            return null;
        }

        /// <summary>
        /// Get most distant target of visible targets.
        /// </summary>
        public virtual Transform GetDistantTarget()
        {
            int bestDistanceIndex = -1;
            float bestDistance = Mathf.NegativeInfinity;
            for (int i = 0; i < GetVisibleTargets().Count; i++)
            {
                Transform target = GetVisibleTargets()[i];
                float distance = Math.Distance2D(transform.position, target.position);
                if (distance > bestDistance)
                {
                    bestDistanceIndex = i;
                    bestDistance = distance;
                }
            }

            if (bestDistanceIndex >= 0)
            {
                return GetVisibleTargets()[bestDistanceIndex];
            }
            return null;
        }
        #endregion

        #region[IVisionCallback Implementation]
        /// <summary>
        /// Called when target become visible in field of view.
        /// </summary>
        public abstract event Action<Transform> OnTargetBecomeVisible;

        /// <summary>
        /// Called when target become invisible in field of view.
        /// </summary>
        public abstract event Action OnTargetsBecomeInvisible;
        #endregion
    }
}