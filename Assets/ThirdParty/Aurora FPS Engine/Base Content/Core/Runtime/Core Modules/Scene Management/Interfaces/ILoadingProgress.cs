/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.CoreModules.SceneManagement
{
    public interface ILoadingProgress
    {
        /// <summary>
        /// Get current scene loading progress.
        /// </summary>
        float GetLoadingProgress();
    }
}