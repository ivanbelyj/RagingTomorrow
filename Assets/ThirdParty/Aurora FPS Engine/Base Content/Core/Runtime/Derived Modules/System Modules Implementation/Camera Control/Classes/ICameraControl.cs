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
    [System.Obsolete]
    public interface ICameraControl
    {
        /// <summary>
        /// Initialize camera control instance.
        /// </summary>
        /// <param name="controller">Target character controller reference.</param>
        void Initialize(Controller controller);
    }
}