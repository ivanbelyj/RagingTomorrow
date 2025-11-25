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
using UnityEngine.AI;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/FSM/Controller/NavMesh AI Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshAIController : AIController
    {
        // Stored required properties.
        private NavMeshAgent navMeshAgent;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override void Sleep(bool value)
        {
            base.Sleep(value);
            navMeshAgent.isStopped = value;
        }

        #region [IControllerVelocity Implementation]
        /// <summary>
        /// Controller velocity in Vector3 representation.
        /// </summary>
        public override Vector3 GetVelocity()
        {
            return navMeshAgent.velocity;
        }
        #endregion

        #region [IControllerOrientation Implementation]
        public override void UpdateOrientation(bool value)
        {
            navMeshAgent.updateRotation = value;
        }
        #endregion

        #region [IControllerDestination Implementation]
        /// <summary>
        /// Set controller destination.
        /// </summary>
        /// <param name="position">Position in wolrd space.</param>
        public override void SetDestination(Vector3 position)
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(position);
            }
        }

        /// <summary>
        /// Return true if controller reach current destination. Otherwise false.
        /// </summary>
        public override bool IsReachDestination()
        {
            return navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
        }
        #endregion

        #region [IControllerMovement Implementation]
        /// <summary>
        /// Set controller destination.
        /// </summary>
        /// <param name="position">Position in wolrd space.</param>
        public override void IsMoving(bool value)
        {
            navMeshAgent.isStopped = !value;
        }
        #endregion

        #region [Getter / Setter]
        public NavMeshAgent GetNavMeshAgent()
        {
            return navMeshAgent;
        }
        #endregion
    }
}