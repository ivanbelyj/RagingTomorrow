/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.CoreModules.Coroutines
{
    public interface ICoroutineObjectBase
    {
        /// <summary>
        /// Coroutine is processing.
        /// </summary>
        bool IsProcessing();
    }
}