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
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Random Instantiate", "Actions/Unity/Random Instantiate")]
    [HideScriptField]
    public class RandomInstantiateNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(List<GameObject>))]
        private string prefabsVariable;

        [SerializeField]
        [TreeVariable(typeof(GameObject))]
        private List<GameObject> prefabs;


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
        private bool prefabsToggle;

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
            if (!string.IsNullOrEmpty(prefabsVariable) && tree.TryGetVariable<GameObjectListVariable>(prefabsVariable, out GameObjectListVariable gameObjectListVariable))
            {
                prefabs = gameObjectListVariable;
            }

            if (prefabs == null)
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

            GameObject created = Instantiate(prefabs[Random.Range(0, prefabs.Count)], position, rotation);
            if (tree.TryGetVariable<GameObjectVariable>(createdObjectVariable, out GameObjectVariable gameObjectVariable))
            {
                gameObjectVariable.SetValue(created);
            }

            return State.Success;
        }
    }
}