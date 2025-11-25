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
    [TreeNodeContent("Crouch", "Actions/Movement/Crouch")]
    [HideScriptField]
    [RequireComponent(typeof(Animator))]
    public class CrouchNode : ActionNode
    {
        [SerializeField]
        private bool procedural;

        [SerializeField]
        [TreeVariable(typeof(float))]
        [VisibleIf("procedural", true)]
        private string heightOffsetVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        [VisibleIf("procedural", true)]
        [Min(0f)]
        private float heightOffset = 1.635f;


        [SerializeField]
        [TreeVariable(typeof(float))]
        [VisibleIf("procedural", true)]
        private string radiusVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        [VisibleIf("procedural", true)]
        [Min(0f)]
        private float radius = 1f;


        [SerializeField]
        [VisibleIf("procedural", true)]
        private LayerMask wallLayer;


        [SerializeField]
        [TreeVariable(typeof(bool))]
        [VisibleIf("procedural", false)]
        private string crouchVariable;

        [SerializeField]
        [TreeVariable(typeof(bool))]
        [VisibleIf("procedural", false)]
        private bool crouch;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool proceduralToggle;

        [SerializeField]
        [HideInInspector]
        private bool heightOffsetToggle;

        [SerializeField]
        [HideInInspector]
        private bool radiusToggle;

        [SerializeField]
        [HideInInspector]
        private bool crouchToggle;
#endif
        #endregion

        // Stored required components.
        private Animator animator;

        protected override void OnInitialize()
        {
            animator = owner.GetComponent<Animator>();
        }

        protected override State OnUpdate()
        {
            if (!string.IsNullOrEmpty(heightOffsetVariable) && tree.TryGetVariable<FloatVariable>(heightOffsetVariable, out FloatVariable floatVariable1))
            {
                heightOffset = floatVariable1;
            }

            if (!string.IsNullOrEmpty(radiusVariable) && tree.TryGetVariable<FloatVariable>(radiusVariable, out FloatVariable floatVariable2))
            {
                radius = floatVariable2;
            }

            if (!string.IsNullOrEmpty(crouchVariable) && tree.TryGetVariable<BoolVariable>(crouchVariable, out BoolVariable boolVariable))
            {
                crouch = boolVariable;
            }

            if (procedural)
            {
                bool needCrouch = true;
                int iterations = 8;
                for (int i = 0; i < iterations; i++)
                {
                    float angle = ((float)i / iterations) * Mathf.PI * 2;
                    Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

                    if (Physics.Raycast(owner.transform.position + Vector3.up * heightOffset, direction, out RaycastHit hitInfo, radius, wallLayer))
                    {
                        needCrouch = false;
                    }
                }

                animator.SetBool("IsCrouched", needCrouch);
                return needCrouch ? State.Success : State.Failure;
            }
            else
            {
                animator.SetBool("IsCrouched", crouch);
                return State.Success;
            }
        }
    }
}