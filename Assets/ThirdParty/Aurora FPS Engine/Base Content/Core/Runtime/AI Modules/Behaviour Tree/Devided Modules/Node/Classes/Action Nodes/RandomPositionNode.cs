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
    [TreeNodeContent("Random Position", "Actions/Position/Random Position")]
    [HideScriptField]
    public class RandomPositionNode : ActionNode
    {
        private enum Position
        {
            Relative,
            Absolute
        }

        [SerializeField]
        private Position positionType;


        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        [VisibleIf("positionType", "Relative")]
        private string relativePositionVariable;

        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        [VisibleIf("positionType", "Relative")]
        private Vector3 relativePosition;


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string radiusVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private float radius;


        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        private string storageVariable;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool relativePositionToggle;

        [SerializeField]
        [HideInInspector]
        private bool radiusToggle;
#endif
        #endregion

        protected override State OnUpdate()
        {
            if (!string.IsNullOrEmpty(radiusVariable) && tree.TryGetVariable<FloatVariable>(radiusVariable, out FloatVariable floatVariable))
            {
                radius = floatVariable;
            }

            Vector2 point2D = Random.insideUnitCircle * radius;
            Vector3 point3D = new Vector3(point2D.x, 0, point2D.y);

            if (positionType == Position.Absolute)
            {
                point3D += owner.transform.position;
            }
            else
            {
                if (!string.IsNullOrEmpty(relativePositionVariable) && tree.TryGetVariable<Vector3Variable>(relativePositionVariable, out Vector3Variable vector3Variable1))
                {
                    relativePosition = vector3Variable1;
                }

                point3D += relativePosition;
            }

            if (tree.TryGetVariable<Vector3Variable>(storageVariable, out Vector3Variable vector3Variable2))
            {
                vector3Variable2.SetValue(point3D);
                return State.Success;
            }

            return State.Failure;
        }
    }
}