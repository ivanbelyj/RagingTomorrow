/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Set Destination", "Actions/Movement/Nav Mesh/Set Destination")]
    [HideScriptField]
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentSetDestinationNode : ActionNode
    {
        private enum TargetType
        {
            Transform,
            Vector3
        }

        [SerializeField]
        [TreeVariable(typeof(float))]
        private string speedVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private float speed = 5;

        
        [SerializeField]
        [TreeVariable(typeof(float))]
        private string stoppingDistanceVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private float stoppingDistance = 0.1f;


        [SerializeField]
        [TreeVariable(typeof(bool))]
        private string updateRotationVariable;

        [SerializeField]
        [TreeVariable(typeof(bool))]
        private bool updateRotation = true;


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string accelerationVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private float acceleration = 40;


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string toleranceVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private float tolerance = 1f;


        [SerializeField]
        private TargetType targetType;

        [SerializeField]
        [VisibleIf("targetType", "Transform")]
        [TreeVariable(typeof(Transform))]
        private string transformVariable;

        [SerializeField]
        [VisibleIf("targetType", "Vector3")]
        [TreeVariable(typeof(Vector3))]
        private string positionVariable;

        [SerializeField]
        [VisibleIf("targetType", "Vector3")]
        [TreeVariable(typeof(Vector3))]
        private Vector3 position;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool speedToggle;

        [SerializeField]
        [HideInInspector]
        private bool stoppingDistanceToggle;

        [SerializeField]
        [HideInInspector]
        private bool updateRotationToggle;

        [SerializeField]
        [HideInInspector]
        private bool accelerationToggle;

        [SerializeField]
        [HideInInspector]
        private bool toleranceToggle;

        [SerializeField]
        [HideInInspector]
        private bool positionToggle;
#endif
        #endregion

        // Stored required components.
        private NavMeshAgent agent = null;

        // Stored required properties.
        private Transform targetTransform;

        protected override void OnInitialize()
        {
            agent = owner.GetComponent<NavMeshAgent>();
        }

        protected override void OnEntry()
        {
            if (targetType == TargetType.Transform)
            {
                if (tree.TryGetVariable<TransformVariable>(transformVariable, out TransformVariable variable1))
                {
                    targetTransform = variable1;
                }
            }

            if (!string.IsNullOrEmpty(speedVariable))
            {
                if (tree.TryGetVariable<FloatVariable>(speedVariable, out FloatVariable variable))
                {
                    speed = variable;
                }
            }
            agent.speed = speed;

            if (!string.IsNullOrEmpty(stoppingDistanceVariable))
            {
                if (tree.TryGetVariable<FloatVariable>(stoppingDistanceVariable, out FloatVariable variable))
                {
                    stoppingDistance = variable;
                }
            }
            agent.stoppingDistance = stoppingDistance;

            if (!string.IsNullOrEmpty(updateRotationVariable))
            {
                if (tree.TryGetVariable<BoolVariable>(updateRotationVariable, out BoolVariable variable))
                {
                    updateRotation = variable;
                }
            }
            agent.updateRotation = updateRotation;

            if (!string.IsNullOrEmpty(accelerationVariable))
            {
                if (tree.TryGetVariable<FloatVariable>(accelerationVariable, out FloatVariable variable))
                {
                    acceleration = variable;
                }
            }
            agent.acceleration = acceleration;

            agent.isStopped = false;
            agent.stoppingDistance = tolerance;
        }

        protected override State OnUpdate()
        {
            Debug.Assert(agent != null, $"The controller [{owner.name}] should be based on NavMeshControll");
            if (agent == null)
            {
                return State.Failure;
            }


            if (targetType == TargetType.Transform)
            {
                if (targetTransform != null)
                {
                    agent.destination = targetTransform.position;
                }
                else
                {
                    return State.Failure;
                }
            }
            else
            {
                if (positionVariable != string.Empty && tree.TryGetVariable<Vector3Variable>(positionVariable, out Vector3Variable variable2))
                {
                    position = variable2;
                    agent.destination = position;
                }
                else
                {
                    return State.Failure;
                }
            }

            if (agent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                return State.Failure;
            }

            agent.isStopped = false;

            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= tolerance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return State.Success;
                    }
                }
            }

            return State.Running;
        }

        protected override void OnExit()
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
        }
    }
}