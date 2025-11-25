/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/FSM/Health/AI Health")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AIController))]
    public sealed class AICharacterHealth : CharacterHealth
    {
        // Stored required components.
        private AIController controller;

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<AIController>();
        }

        protected override void OnRevive()
        {
            base.OnRevive();
            controller.Sleep(false);
        }

        protected override void OnDead()
        {
            base.OnDead();
            controller.Sleep(true);
        }
    }
}