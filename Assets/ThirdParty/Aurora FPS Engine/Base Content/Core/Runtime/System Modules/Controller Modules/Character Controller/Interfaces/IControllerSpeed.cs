/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime
{
    public interface IControllerSpeed
    {
        float GetSpeed();

        float GetWalkSpeed();

        float GetRunSpeed();

        float GetSprintSpeed();

        float GetBackwardSpeedPerсent();

        float GetSideSpeedPerсent();
    }
}