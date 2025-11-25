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
    public interface IControllerVelocity
    {
        /// <summary>
        /// Controller velocity in Vector3 representation.
        /// </summary>
        Vector3 GetVelocity();
    }
}