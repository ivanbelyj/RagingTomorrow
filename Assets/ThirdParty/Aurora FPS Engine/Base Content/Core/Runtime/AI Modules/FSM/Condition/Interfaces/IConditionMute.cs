/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.AIModules.Conditions
{
    public interface IConditionMute
    {
        /// <summary>
        /// Condition mute state value.
        /// If true this condition will be ignored.
        /// </summary>
        bool IsMuted();
    }
}