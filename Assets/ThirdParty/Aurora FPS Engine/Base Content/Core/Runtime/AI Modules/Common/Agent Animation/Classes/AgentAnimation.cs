/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/Common/Nav Mesh/Agent Animation")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class AgentAnimation : MonoBehaviour
    {
        // Stored required components.
        private Animator animator;
        private NavMeshAgent agent;

        // Stored required properties.
        private Vector3 velocity = Vector3.zero;
        private Vector2 smoothDeltaPosition = Vector2.zero;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            agent.updatePosition = false;
        }

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (!agent.enabled) return;

            Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dx, dy);

            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if time advances
            if (Time.deltaTime > 1e-5f)
                velocity = smoothDeltaPosition / Time.deltaTime;

            bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

            // Update animation parameters
            //animator.SetBool("move", shouldMove);
            animator.SetFloat("Direction", velocity.x);
            animator.SetFloat("Speed", velocity.y);

            //GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;
        }

        /// <summary>
        /// Called at each frame after the state machines 
        /// and the animations have been evaluated, but before OnAnimatorIK.
        /// </summary>
        private void OnAnimatorMove()
        {
            // Update position to agent position
            transform.position = agent.nextPosition;
        }

        /// <summary>
        /// Called after all Update functions have been called. 
        /// </summary>
        //private void LateUpdate()
        //{
        //    float dotF = Vector3.Dot(transform.forward, agent.velocity.normalized);
        //    float dotR = Vector3.Dot(transform.right, agent.velocity.normalized);

        //    animator.SetFloat("Speed", dotF * agent.velocity.magnitude);
        //    animator.SetFloat("Direction", dotR);
        //}

        //void Update()
        //{
        //    //agent.SetDestination(formationPosition.position);
        //    //animator.SetFloat("Speed", agent.velocity.magnitude);
        //}

        //void OnAnimatorMove()
        //{
        //    Vector3 position = animator.rootPosition;
        //    position.y = agent.nextPosition.y;
        //    transform.position = position;
        //    agent.nextPosition = transform.position;
        //}
    }
}