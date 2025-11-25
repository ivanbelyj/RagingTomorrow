/* ================================================================
   ----------------------------------------------------------------
   Project   :   Apex Inspector
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.CoreModules.InputSystem
{
    public interface IInputListener
    {
        /// <summary>
        /// Register required input actions.
        /// </summary>
        void RegisterInputActions();

        /// <summary>
        /// Remove registered input actions.
        /// </summary>
        void RemoveInputActions();
    }
}