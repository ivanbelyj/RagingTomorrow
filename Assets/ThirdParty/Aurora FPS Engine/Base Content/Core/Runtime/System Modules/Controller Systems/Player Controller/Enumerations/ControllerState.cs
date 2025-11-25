/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [Flags]
    public enum ControllerState
    {
        /// <summary>
        /// Controller state disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Controller is idle.
        /// </summary>
        Idle = 1 << 0,

        /// <summary>
        /// Controller is walking.
        /// </summary>
        Walking = 1 << 1,

        /// <summary>
        /// Controller is running.
        /// </summary>
        Running = 1 << 2,

        /// <summary>
        /// Controller is sprinting.
        /// </summary>
        Sprinting = 1 << 3,

        /// <summary>
        /// Controller is jumping.
        /// </summary>
        Jumped = 1 << 4,

        /// <summary>
        /// Controller in air.
        /// </summary>
        InAir = 1 << 5,

        /// <summary>
        /// Controller is idle crouching.
        /// </summary>
        Crouched = 1 << 6,

        /// <summary>
        /// Controller is climbing.
        /// </summary>
        Climbing = 1 << 7,

        /// <summary>
        /// Controller is zomming.
        /// </summary>
        Zooming = 1 << 8,

        /// <summary>
        /// All controller states.
        /// </summary>
        All = ~0
    }
}
