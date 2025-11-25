/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [System.Serializable]
    [ReferenceContent("Unity Event", "Unity Event")]
    public sealed class RespawnUnityEvent : CharacterRespawnEvent
    {
        [SerializeField]
        private UnityEvent revive;

        public override void OnRevive()
        {
            revive.Invoke();
        }
    }
}