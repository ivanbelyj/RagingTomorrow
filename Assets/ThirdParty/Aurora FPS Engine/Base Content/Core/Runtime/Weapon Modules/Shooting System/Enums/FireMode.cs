/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.WeaponModules
{
    /// <summary>
    /// Fire mode of the weapon.
    ///     Single - Single fire.
    ///     Fixed queue - Fire with fixed queue (for example 3 fire).
    ///     Free - Free fire without restrictions.
    /// </summary>
    [System.Flags]
    public enum FireMode
    {
        Mute = 0,
        Single = 1 << 0,
        Queue = 1 << 1,
        Free = 1 << 2,
        All = ~0
    }
}