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
using AuroraFPSRuntime.CoreModules.Mathematics;
using UnityEngine;

#region [Unity Editor Section]
#if UNITY_EDITOR
#endif
#endregion

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [AIBehaviourMenu("Run Away", "Locomotion/Run Away")]
    public sealed class AIRunAwayVisionBehaviour : AIBehaviour
    {
        [SerializeField]
        [NotNull]
        private FieldOfView fieldOfView;

        [SerializeField]
        [MinValue(0.01f)]
        private float minDistance;

        [SerializeField]
        [CustomView(ViewInitialization = "OnTargetBehaviourInitialization", ViewGUI = "OnTargetBehaviourGUI")]
        [Foldout("Default Behaviour")]
        [Message("Execute target behaviour transition when target become invisible.")]
        private string targetBehaviour = AIIdleBehaviour.IDLE_BEHAVIOUR;

        private Transform transform;

        /// <summary>
        /// Called when AIController owner instance is being loaded.
        /// </summary>
        protected override void OnInitialize()
        {
            transform = owner.transform;
        }

        /// <summary>
        /// Called when this behaviour becomes enabled.
        /// </summary>
        protected override void OnEnable()
        {
            fieldOfView.OnTargetsBecomeInvisible += ExecuteDefaultTransition;
        }

        /// <summary>
        /// Called when this behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            fieldOfView.OnTargetsBecomeInvisible -= ExecuteDefaultTransition;
        }

        /// <summary>
        /// Switch owner behaviour to target behaviour.
        /// </summary>
        private void ExecuteDefaultTransition()
        {
            owner.SwitchBehaviour(targetBehaviour);
        }

        /// <summary>
        /// Called every frame, while this behaviour is running.
        /// </summary>
        protected override void Update()
        {
            Transform target = fieldOfView.GetNearestTarget();
            if (target != null)
            {
                float distance = Math.Distance2D(transform.position, target.position);
                if(distance < minDistance)
                {
                    Vector3 direction = transform.position - target.position;
                    owner.SetDestination(transform.position + direction);
                }
            }
        }
    }
}