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
    [ReferenceContent("Unity Event", "Unity Event")]
    public sealed class SwitcherEvent : CharacterDeathEvent
    {
        [SerializeField]
        private UnityEvent eventCallback;

        public override void OnDead()
        {
            eventCallback?.Invoke();
        }
    }
}

