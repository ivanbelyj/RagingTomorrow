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
using System.Linq;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Look IK", "Actions/Movement/Look IK")]
    [HideScriptField]
    public class LookIKNode : ActionNode
    {
        [SerializeField]
        private string lookHingeName = "Look Hinge";

        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private string targetVariable;

        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private Transform target;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool targetToggle;
#endif
        #endregion

        // Stored required components.
        private Transform lookHinge;

        protected override void OnInitialize()
        {
            lookHinge = owner.GetComponentsInChildren<Transform>().Where(t => t.name == lookHingeName).FirstOrDefault();
        }

        protected override void OnEntry()
        {
            if (tree.TryGetVariable<TransformVariable>(targetVariable, out TransformVariable transformVariable))
            {
                target = transformVariable;
            }
        }

        protected override State OnUpdate()
        {
            if (target != null)
            {
                if (target.TryGetComponent<Collider>(out Collider targetCollider))
                {
                    lookHinge.LookAt(targetCollider.bounds.center);
                }
                else
                {
                    lookHinge.LookAt(target);
                }

                return State.Success;
            }
            else
            {
                lookHinge.localRotation = Quaternion.identity;
                return State.Failure;
            }
        }
    }
}