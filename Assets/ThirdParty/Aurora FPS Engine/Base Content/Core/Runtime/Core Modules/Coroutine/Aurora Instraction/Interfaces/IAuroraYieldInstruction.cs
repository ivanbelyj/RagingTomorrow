/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.CoreModules.Coroutines
{
    public interface IAuroraYieldInstruction
    {
        bool IsExecuting();
        bool IsPaused();

        void Pause();
        void Resume();
        void Terminate();

        event Action<AuroraYieldInstruction> OnStartedCallback;
        event Action<AuroraYieldInstruction> OnPausedCallback;
        event Action<AuroraYieldInstruction> OnTerminatedCallback;
        event Action<AuroraYieldInstruction> OnDoneCallback;
    }
}