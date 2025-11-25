/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.CoreModules
{
    /// <summary>
    /// Represents base class for all scriptable mapping.
    /// </summary>
    public abstract class ScriptableMapping : ScriptableObject, IScriptableMapping
    {
        /// <summary>
        /// Get mapping length.
        /// </summary>
        public abstract int GetMappingLength();
    }
}