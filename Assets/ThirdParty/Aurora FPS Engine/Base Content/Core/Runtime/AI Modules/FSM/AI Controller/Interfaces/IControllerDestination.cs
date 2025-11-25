/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    public interface IControllerDestination
    {
        /// <summary>
        /// Set controller destination.
        /// </summary>
        /// <param name="position">Position in wolrd space.</param>
        void SetDestination(Vector3 position);

        /// <summary>
        /// Return true if controller reach current destination. Otherwise false.
        /// </summary>
        bool IsReachDestination();
    }
}