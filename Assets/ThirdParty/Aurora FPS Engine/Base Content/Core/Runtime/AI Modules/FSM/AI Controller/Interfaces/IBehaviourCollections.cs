/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.Behaviour;
using System.Collections.Generic;

namespace AuroraFPSRuntime.AIModules
{
    public interface IBehaviourCollections
    {
        /// <summary>
        /// Iterate all initialized behaviour names.
        /// </summary>
        IEnumerable<string> GetBehaviourNames();

        /// <summary>
        /// Iterate all initialized behaviour instances.
        /// </summary>
        IEnumerable<AIBehaviour> GetBehaviours();
    }
}