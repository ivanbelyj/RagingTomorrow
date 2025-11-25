/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.AIModules.Vision;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [AIBehaviourMenu("Follow", "Locomotion/Follow")]
    public class AIFollowBehaviour : AIBehaviour
    {
        [SerializeField]
        private TargetSelection targetSelection = TargetSelection.Nearest;

        [SerializeField]
        [NotNull]
        private FieldOfView fieldOfView;

        [SerializeField]
        [CustomView(ViewInitialization = "OnTargetBehaviourInitialization", ViewGUI = "OnTargetBehaviourGUI")]
        [Foldout("Default Transition")]
        [Message("Execute transition to target behaviour, when lost target.\nIf target behaviour empty, wait for custom transition.")]
        private string targetBehaviour = AIIdleBehaviour.IDLE_BEHAVIOUR;

        private Transform target;

        /// <summary>
        /// Called every frame, while this behaviour is running.
        /// </summary>
        protected override void Update()
        {
            switch (targetSelection)
            {
                case TargetSelection.First:
                    target = fieldOfView.GetFirstTarget();
                    break;
                case TargetSelection.Nearest:
                    target = fieldOfView.GetNearestTarget();
                    break;
                case TargetSelection.Distant:
                    target = fieldOfView.GetDistantTarget();
                    break;
            }

            if(target != null)
            {
                owner.SetDestination(target.position);
            }
            else if(target == null)
            {
                owner.SwitchBehaviour(targetBehaviour);
            }
        }
    }
}