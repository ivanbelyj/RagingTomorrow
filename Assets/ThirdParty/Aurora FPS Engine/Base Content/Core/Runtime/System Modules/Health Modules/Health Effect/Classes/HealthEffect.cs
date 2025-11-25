/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [System.Serializable]
    public abstract class HealthEffect
    {
        /// <summary>
        /// Implement this method to make some initialization 
        /// and get access to CharacterHealth references.
        /// </summary>
        /// <param name="healthComponent">Player health component reference.</param>
        public abstract void Initialization(CharacterHealth characterHealth);
    }
}