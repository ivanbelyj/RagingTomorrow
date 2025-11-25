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
    [TreeNodeContent("Rotate Towards", "Actions/Movement/Rotate Towards")]
    [HideScriptField]
    public class RotateTowardsNode : ActionNode
    {
        public enum TargetType
        {
            Transform,
            Vector3
        }


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string rotationSpeedVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private float rotationSpeed = 5f;

        [SerializeField]
        private TargetType targetType;


        [SerializeField]
        [TreeVariable(typeof(Transform))]
        [VisibleIf("targetType", "Transform")]
        private string transformVariable;


        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        [VisibleIf("targetType", "Vector3")]
        private string pointVariable;

        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        [VisibleIf("targetType", "Vector3")]
        private Vector3 point;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool rotationSpeedToggle;

        [SerializeField]
        [HideInInspector]
        private bool pointToggle;
#endif
        #endregion

        protected override void OnEntry()
        {
            if (!string.IsNullOrEmpty(rotationSpeedVariable) && tree.TryGetVariable<FloatVariable>(rotationSpeedVariable, out FloatVariable variable))
            {
                rotationSpeed = variable;
            }
        }

        protected override State OnUpdate()
        {
            Quaternion? rotation = CalculateRotation();

            if (rotation == null)
            {
                return State.Failure;
            }

            float dot = Quaternion.Dot(owner.transform.rotation, rotation.Value);
            if (Mathf.Abs(dot) >= 0.999)
            {
                return State.Success;
            }

            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation.Value, Time.deltaTime * rotationSpeed);
            return State.Running;
        }

        private Quaternion? CalculateRotation()
        {
            Quaternion rotation = Quaternion.identity;
            switch (targetType)
            {
                case TargetType.Transform:
                    if (tree.TryGetVariable<TransformVariable>(transformVariable, out TransformVariable variable))
                    {
                        if (variable.GetValue() == null)
                        {
                            return null;
                        }

                        Vector3 direction1 = variable.GetValue().position - owner.transform.position;
                        direction1.y = 0f;
                        rotation = Quaternion.LookRotation(direction1);
                    }
                    break;
                case TargetType.Vector3:
                    if (!string.IsNullOrEmpty(pointVariable) && tree.TryGetVariable<Vector3Variable>(pointVariable, out Vector3Variable vector3Variable))
                    {
                        point = vector3Variable;
                    }

                    Vector3 direction2 = point - owner.transform.position;
                    direction2.y = 0f;
                    rotation = Quaternion.LookRotation(direction2);
                    break;
            }
            return rotation;
        }
    }
}