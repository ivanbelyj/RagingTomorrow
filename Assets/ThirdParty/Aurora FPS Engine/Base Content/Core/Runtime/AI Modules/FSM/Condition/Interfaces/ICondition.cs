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
    public interface ICondition
    {
        /// <summary>
        /// Condition for translate to the next AI behaviour.
        /// </summary>
        bool IsExecuted();
    }
}