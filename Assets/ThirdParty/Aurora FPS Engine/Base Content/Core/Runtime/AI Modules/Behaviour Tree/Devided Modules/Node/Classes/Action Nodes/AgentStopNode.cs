/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Stop", "Actions/Movement/Nav Mesh/Stop")]
    [HideScriptField]
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentStopNode : ActionNode
    {
        // Stored required components.
        private NavMeshAgent agent = null;

        protected override void OnInitialize()
        {
            agent = owner.GetComponent<NavMeshAgent>();
        }

        protected override State OnUpdate()
        {
            if (agent != null)
            {
                agent.isStopped = true;
                return State.Success;
            }
            return State.Success;
        }
    }
}