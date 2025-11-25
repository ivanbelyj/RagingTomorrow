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
    [TreeNodeContent("Find Transforms", "Actions/Unity/Find Transforms")]
    [HideScriptField]
    public class FindTransformsNode : ActionNode
    {
        public enum FetchMethod
        {
            Tag,
            Manually
        }

        [SerializeField]
        private FetchMethod fetchMethod;

        [SerializeField]
        [TagPopup]
        [VisibleIf("fetchMethod", "Tag")]
        private string tagName;

        [SerializeField]
        [VisibleIf("fetchMethod", "Manually")]
        private string[] names;

        [SerializeField]
        [TreeVariable(typeof(List<Transform>))]
        private string storageVariable;

        protected override State OnUpdate()
        {
            List<Transform> transforms = new List<Transform>();

            switch (fetchMethod)
            {
                case FetchMethod.Tag:
                    GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tagName);
                    for (int i = 0; i < gameObjects.Length; i++)
                    {
                        transforms.Add(gameObjects[i].transform);
                    }
                    break;
                case FetchMethod.Manually:
                    for (int i = 0; i < names.Length; i++)
                    {
                        GameObject go = GameObject.Find(names[i]);
                        if (go != null)
                        {
                            transforms.Add(go.transform);
                        }
                    }
                    break;
            }

            if (tree.TryGetVariable<TransformListVariable>(storageVariable, out TransformListVariable variable))
            {
                variable.SetValue(transforms);
                return State.Success;
            }

            return State.Failure;
        }
    }
}