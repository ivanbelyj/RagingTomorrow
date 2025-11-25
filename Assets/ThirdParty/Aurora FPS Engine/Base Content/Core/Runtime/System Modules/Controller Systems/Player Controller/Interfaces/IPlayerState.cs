/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    interface IPlayerState
    {
        ControllerState GetState();
        bool CompareState(ControllerState state);
        bool HasState(ControllerState state);
    }
}
