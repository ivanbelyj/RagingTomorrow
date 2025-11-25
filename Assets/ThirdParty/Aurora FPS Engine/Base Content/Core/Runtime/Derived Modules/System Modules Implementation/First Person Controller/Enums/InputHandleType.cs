/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    /// <summary>
    /// Input handle type.
    /// </summary>
    [System.Obsolete("Use Controller from AuroraFPSRuntime.SystemModules.ControllerSystem instead.")]
    public enum InputHandleType
    {
        /// <summary>
        /// Once press button for update action and one more time for reset action.
        /// </summary>
        Trigger,

        /// <summary>
        /// Hold button for update action.
        /// </summary>
        Hold
    }
}