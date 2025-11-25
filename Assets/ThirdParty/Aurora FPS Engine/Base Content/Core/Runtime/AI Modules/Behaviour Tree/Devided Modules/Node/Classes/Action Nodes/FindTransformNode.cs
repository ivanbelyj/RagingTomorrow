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
    [TreeNodeContent("Find Transform", "Actions/Unity/Find Transform")]
    [HideScriptField]
    public class FindTransformNode : ActionNode
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
        private string objectName;

        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private string storageVariable;

        protected override State OnUpdate()
        {
            Transform transform = null;

            switch (fetchMethod)
            {
                case FetchMethod.Tag:
                    GameObject go1 = GameObject.FindGameObjectWithTag(tagName);
                    if (go1 != null)
                    {
                        transform = go1.transform;
                    }
                    break;
                case FetchMethod.Manually:
                    GameObject go2 = GameObject.Find(objectName);
                    if (go2 != null)
                    {
                        transform = go2.transform;
                    }
                    break;
            }

            if (transform != null)
            {
                if (tree.TryGetVariable<TransformVariable>(storageVariable, out TransformVariable variable))
                {
                    variable.SetValue(transform);
                    return State.Success;
                }
            }

            return State.Failure;
        }
    }
}
