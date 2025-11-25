/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.CoreModules.Serialization.Collections
{
    public interface ISerializationStorage<T>
    {
        T GetStorageData();
    }
}