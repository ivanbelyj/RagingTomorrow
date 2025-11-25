/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.AIModules
{
    public interface IControllerMovement
    {
        /// <summary>
        /// Resume or stop controller movement.
        /// </summary>
        /// <param name="value">Set true to resume moving or false to stop.</param>
        void IsMoving(bool value);
    }
}