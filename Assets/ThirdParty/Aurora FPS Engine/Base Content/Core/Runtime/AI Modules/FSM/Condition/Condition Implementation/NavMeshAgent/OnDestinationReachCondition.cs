/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.Mathematics;
using UnityEngine;
using UnityEngine.AI;


namespace AuroraFPSRuntime.AIModules.Conditions
{
    [ConditionMenu("On Destination Reach", "NavMeshAgent/On Destination Reach", Description = "Return true when target destination is reached.")]
    public class OnDestinationReachCondition : Condition
    {
        // Base on destination reach properties.
        [SerializeField] private float tolerance = 0.1f;

        // Stored required components.
        private NavMeshAgent navMeshAgent;

        /// <summary>
        /// Initializing required properties.
        /// </summary>
        /// <param name="core">Target AIController instance.</param>
        protected override void OnInitialize(AIController core)
        {
            base.OnInitialize(core);
            navMeshAgent = core.GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Condition for translate to the next AI behaviour.
        /// </summary>
        public override bool IsExecuted()
        {
            return !navMeshAgent.pathPending && Math.Approximately(navMeshAgent.remainingDistance, navMeshAgent.stoppingDistance, tolerance) && navMeshAgent.hasPath;
        }
    }
}