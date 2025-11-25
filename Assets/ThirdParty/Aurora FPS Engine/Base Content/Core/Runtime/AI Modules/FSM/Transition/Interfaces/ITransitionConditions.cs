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
    public interface ITransitionConditions
    {
        void AddCondition(Condition condition);

        bool RemoveCondition(Condition condition);
    }
}