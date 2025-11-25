/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.Transitions;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    public interface IBehaviourTransitions
    {
        void AddTransition(Transition transition);

        bool RemoveTransition(Transition transition);
    }
}