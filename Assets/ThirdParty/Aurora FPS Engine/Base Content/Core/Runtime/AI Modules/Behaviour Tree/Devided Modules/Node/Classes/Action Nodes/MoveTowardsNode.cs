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
    [TreeNodeContent("Move Towards", "Actions/Movement/Move Towards")]
    [HideScriptField]
    public class MoveTowardsNode : ActionNode
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
        private float speed;


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string toleranceVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private float tolerance = 1f;


        [SerializeField]
        [TreeVariable(typeof(bool))]
        private string lookAtTargetVariable;

        [SerializeField]
        [TreeVariable(typeof(bool))]
        private bool lookAtTarget = true;


        [SerializeField]
        [TreeVariable(typeof(float))]
        [VisibleIf("lookAtTarget")]
        private string rotationSpeedVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        [VisibleIf("lookAtTarget")]
        private float rotationSpeed = 5f;


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
        private bool toleranceToggle;

        [SerializeField]
        [HideInInspector]
        private bool lookAtTargetToggle;

        [SerializeField]
        [HideInInspector]
        private bool rotationSpeedToggle;

        [SerializeField]
        [HideInInspector]
        private bool positionToggle;
#endif
        #endregion

        // Stored required properties.
        private Transform targetTransform;
        private Vector3? desiredPostion;
        private Quaternion desiredRotation;

        protected override void OnEntry()
        {
            switch (targetType)
            {
                case TargetType.Transform:
                    if (tree.TryGetVariable<TransformVariable>(transformVariable, out TransformVariable variable1))
                    {
                        targetTransform = variable1;
                    }
                    break;
                case TargetType.Vector3:
                    if (positionVariable != string.Empty)
                    {
                        if (tree.TryGetVariable<Vector3Variable>(positionVariable, out Vector3Variable variable2))
                        {
                            position = variable2;
                        }
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(speedVariable))
            {
                if (tree.TryGetVariable<FloatVariable>(speedVariable, out FloatVariable variable))
                {
                    speed = variable;
                }
            }

            if (!string.IsNullOrEmpty(toleranceVariable))
            {
                if (tree.TryGetVariable<FloatVariable>(toleranceVariable, out FloatVariable variable))
                {
                    tolerance = variable;
                }
            }

            if (!string.IsNullOrEmpty(lookAtTargetVariable))
            {
                if (tree.TryGetVariable<BoolVariable>(lookAtTargetVariable, out BoolVariable variable))
                {
                    lookAtTarget = variable;
                }
            }

            if (!string.IsNullOrEmpty(rotationSpeedVariable))
            {
                if (tree.TryGetVariable<FloatVariable>(rotationSpeedVariable, out FloatVariable variable))
                {
                    rotationSpeed = variable;
                }
            }
        }

        protected override State OnUpdate()
        {
            CalcualteTargetPosition();
            if (desiredPostion == null) return State.Failure;
            CalculateTargetRotation(desiredPostion.Value);
            float dot = Quaternion.Dot(owner.transform.rotation, desiredRotation);

            if (Vector3.Distance(owner.transform.position, desiredPostion.Value) < tolerance &&
                (!lookAtTarget || lookAtTarget && Mathf.Abs(dot) >= 0.999))
            {
                return State.Success;
            }

            if (lookAtTarget)
            {
                owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
            }

            Vector3 moveVector = (desiredPostion.Value - owner.transform.position).normalized * Time.deltaTime;
            owner.transform.Translate(moveVector * speed, Space.World);
            return State.Running;
        }

        private void CalcualteTargetPosition()
        {
            if (targetType == TargetType.Transform)
            {
                if (targetTransform != null)
                {
                    desiredPostion = targetTransform.position;
                }
                else
                {
                    desiredPostion = null;
                }
            }
            else
            {
                desiredPostion = position;
            }
        }

        private void CalculateTargetRotation(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - owner.transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                desiredRotation = Quaternion.LookRotation(direction);
            }
        }
    }
}