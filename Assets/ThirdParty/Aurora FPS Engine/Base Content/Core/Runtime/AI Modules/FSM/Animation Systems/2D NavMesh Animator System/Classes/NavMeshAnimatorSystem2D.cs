/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using UnityEngine;
using UnityEngine.AI;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/FSM/Animation/2D NavMesh Animator System")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAIController))]
    public class NavMeshAnimatorSystem2D : AnimatorSystem1D
    {
        [SerializeField]
        [Prefix("Float", Style = "Parameter")]
        private AnimatorParameter directionParameter = "Direction";

        // Stored required components.
        private NavMeshAgent navMeshAgent;

        // Stored required properties.
        private Vector3 velocity = Vector3.zero;
        private Vector2 smoothDeltaPosition = Vector2.zero;

        /// <summary>
        /// Called on the frame when a script is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            NavMeshAIController navMeshAIController = controller as NavMeshAIController;
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updatePosition = false;
        }

        /// <summary>
        /// Called after all Update functions have been called. 
        /// </summary>
        protected override void LateUpdate()
        {
            Vector3 worldDeltaPosition = navMeshAgent.nextPosition - transform.position;

            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dx, dy);

            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            if (Time.deltaTime > Mathf.Epsilon)
            {
                velocity = smoothDeltaPosition / Time.deltaTime;
            }

            if (worldDeltaPosition.magnitude > navMeshAgent.radius)
            {
                transform.position = navMeshAgent.nextPosition - (0.9f * worldDeltaPosition);
            }

            SetSpeedParameter(velocity.y);
            animator.SetFloat(directionParameter, velocity.x);
        }

        /// <summary>
        /// Callback for processing animation movements for modifying root motion.
        /// </summary>
        protected virtual void OnAnimatorMove()
        {
            Vector3 position = animator.rootPosition;
            position.y = navMeshAgent.nextPosition.y;
            transform.position = position;
        }
    }
}