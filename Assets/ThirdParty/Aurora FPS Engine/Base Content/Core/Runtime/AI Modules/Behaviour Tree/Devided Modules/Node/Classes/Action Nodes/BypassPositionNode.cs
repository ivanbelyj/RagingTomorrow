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

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Bypass Position", "Actions/Position/Bypass Position")]
    [HideScriptField]
    public class BypassPositionNode : ActionNode
    {
        private enum Direction
        {
            Front = 0,
            Behind = 1,
            Leftward = 2,
            Rightward = 3
        }

        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private string targetVariable;

        [SerializeField]
        private Direction direction = Direction.Front;


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string radiusVariable;

        [SerializeField]
        [Min(0.01f)]
        private float radius;


        [SerializeField]
        private LayerMask wallLayer = 1;

        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        private string storageVariable;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool radiusToggle;
#endif
        #endregion

        // Stored required properties.
        private Transform target;
        private Vector3 position;

        protected override void OnEntry()
        {
            if (tree.TryGetVariable<TransformVariable>(targetVariable, out TransformVariable variable))
            {
                target = variable;

                if (target == null)
                {
                    return;
                }

                Ray ray = new Ray();

                switch (direction)
                {
                    case Direction.Front:
                        ray = new Ray(target.position, target.forward);
                        break;
                    case Direction.Behind:
                        ray = new Ray(target.position, -target.forward);
                        break;
                    case Direction.Leftward:
                        ray = new Ray(target.position, -target.right);
                        break;
                    case Direction.Rightward:
                        ray = new Ray(target.position, target.right);
                        break;
                }

                if (Physics.Raycast(ray, out RaycastHit hitInfo, radius, wallLayer))
                {
                    position = hitInfo.point;
                }
                else
                {
                    position = ray.origin + ray.direction * radius;
                }
            }
        }

        protected override State OnUpdate()
        {
            if (target == null || string.IsNullOrEmpty(storageVariable))
            {
                return State.Failure;
            }
            
            if (tree.TryGetVariable<Vector3Variable>(storageVariable, out Vector3Variable variable))
            {
                variable.SetValue(position);
                return State.Success;
            }

            return State.Failure;
        }
    }
}