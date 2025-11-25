/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree;

namespace AuroraFPSRuntime.AIModules
{
    public interface IBehaviourTree
    {
        /// <summary>
        /// Returns the behavior tree used.
        /// </summary>
        BehaviourTreeAsset GetBehaviourTree();
    }
}