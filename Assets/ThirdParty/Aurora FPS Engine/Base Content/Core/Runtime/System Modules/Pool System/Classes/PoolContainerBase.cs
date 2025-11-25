/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules
{
    [System.Serializable]
    public abstract class PoolContainerBase : IPoolContainer
    {
        public enum Allocator
        {
            Free,
            Fixed,
            Dynamic
        }

        /// <summary>
        /// Length of available object in pool.
        /// </summary>
        public abstract int GetLength();
    }
}