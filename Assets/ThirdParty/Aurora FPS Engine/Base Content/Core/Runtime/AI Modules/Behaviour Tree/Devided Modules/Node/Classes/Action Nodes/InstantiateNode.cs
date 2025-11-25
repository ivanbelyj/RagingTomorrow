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
    [TreeNodeContent("Instantiate", "Actions/Unity/Instantiate")]
    [HideScriptField]
    public class InstantiateNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(GameObject))]
        private string prefabVariable;

        [SerializeField]
        [TreeVariable(typeof(GameObject))]
        private GameObject prefab;


        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        private string positionVariable;

        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        private Vector3 position;


        [SerializeField]
        [TreeVariable(typeof(Quaternion))]
        private string rotationVariable;

        [SerializeField]
        [TreeVariable(typeof(Quaternion))]
        private Quaternion rotation;


        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private string parentVariable;


        [SerializeField]
        [TreeVariable(typeof(GameObject))]
        private string createdObjectVariable;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool prefabToggle;

        [SerializeField]
        [HideInInspector]
        private bool positionToggle;

        [SerializeField]
        [HideInInspector]
        private bool rotationToggle;
#endif
        #endregion

        protected override State OnUpdate()
        {
            if (!string.IsNullOrEmpty(prefabVariable) && tree.TryGetVariable<GameObjectVariable>(prefabVariable, out GameObjectVariable gameObjectVariable1))
            {
                prefab = gameObjectVariable1;
            }

            if (prefab == null)
            {
                return State.Failure;
            }

            if (!string.IsNullOrEmpty(positionVariable) && tree.TryGetVariable<Vector3Variable>(positionVariable, out Vector3Variable vector3Variable))
            {
                position = vector3Variable;
            }

            if (!string.IsNullOrEmpty(rotationVariable) && tree.TryGetVariable<QuaternionVariable>(rotationVariable, out QuaternionVariable quaternionVariable))
            {
                rotation = quaternionVariable;
            }

            GameObject created = Instantiate(prefab, position, rotation);
            if (tree.TryGetVariable<GameObjectVariable>(createdObjectVariable, out GameObjectVariable gameObjectVariable2))
            {
                gameObjectVariable2.SetValue(created);
            }

            return State.Success;
        }
    }
}