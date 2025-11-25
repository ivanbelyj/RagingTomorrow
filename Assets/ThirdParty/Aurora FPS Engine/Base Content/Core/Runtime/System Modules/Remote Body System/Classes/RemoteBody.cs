/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Remote Body/Remote Body")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [System.Obsolete("Remote Body component is deprecated and will be removed in next updates.\nUse Remote Body Animator component instead.")]
    public sealed class RemoteBody : MonoBehaviour
    {
        internal const float VELOCITY_RATIO = 0.025f;

        [SerializeReference]
        [NotNull]
        private PlayerController controller;

        [SerializeField]
        [Foldout("Movement Settings", Style = "Header")]
        private AnimatorParameter speedParameter = "Speed";

        [SerializeField]
        [Foldout("Movement Settings", Style = "Header")]
        private AnimatorParameter directionParameter = "Direction";

        [SerializeField]
        [Foldout("Movement Settings", Style = "Header")]
        private AnimatorParameter isCrouchedParameter = "IsCrouched";

        [SerializeField]
        [Foldout("Movement Settings", Style = "Header")]
        private AnimatorParameter isGroundedParameter = "IsGrounded";

        [SerializeField]
        [Foldout("Movement Settings", Style = "Header")]
        private AnimatorParameter isJumpedParameter = "IsJumped";

        [SerializeField]
        [Foldout("Movement Settings", Style = "Header")]
        private float velocitySmooth = 12.5f;

        [SerializeField]
        [Foldout("Look Settings", Style = "Header")]
        private Transform lookTarget;

        [SerializeField]
        [Foldout("Rotation Settings", Style = "Header")]
        [Slider(0, 360)]
        private float angleTolerance;

        [SerializeField]
        [VisibleIf("angleTolerance", ">", "0")]
        [Foldout("Rotation Settings", Style = "Header")]
        [Indent(1)]
        private AnimatorParameter turnAxisParameter = "TurnAxis";

        [SerializeField]
        [Label("Duration")]
        [VisibleIf("angleTolerance", ">", "0")]
        [Foldout("Rotation Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Indent(1)]
        private float rotateDuration;

        [SerializeField]
        [Label("Curve")]
        [VisibleIf("angleTolerance", ">", "0")]
        [Foldout("Rotation Settings", Style = "Header")]
        [Indent(1)]
        private AnimationCurve rotateCurve;

        // Stored required components.
        private Animator animator;

        // Stored required properties.
        private float yRotation;
        private Vector3 deltaVelocity;
        private Vector3 eulerAngles;
        private CoroutineObject<int> syncRotationCoroutine;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Debug.Assert(controller != null, $"<b><color=#FF0000>Attach reference of the player controller to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Controller<i>(field)</i>.</color></b>");

            animator = GetComponent<Animator>();
            syncRotationCoroutine = new CoroutineObject<int>(this);
            eulerAngles = controller.transform.forward;
        }

        /// <summary>
        /// Called every frame, while the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            MovementHandler();
            RotateHandler();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if(lookTarget != null)
            {
                animator.SetLookAtPosition(lookTarget.position);
                animator.SetLookAtWeight(1, 1, 1, 1, 0);
            }
        }

        private void MovementHandler()
        {
            Vector3 velocity = controller.GetVelocity();
            if(velocity.magnitude > VELOCITY_RATIO)
            {
                float dx = Vector3.Dot(controller.transform.right, velocity);
                float dy = Vector3.Dot(controller.transform.forward, velocity);
                Vector3 newDeltaVelocity = new Vector2(dx, dy);
                deltaVelocity = Vector3.Lerp(deltaVelocity, newDeltaVelocity, velocitySmooth * Time.deltaTime);
            }
            else
            {
                deltaVelocity = Vector3.zero;
            }
            

            animator.SetFloat(speedParameter, deltaVelocity.y);
            animator.SetFloat(directionParameter, deltaVelocity.x);
            animator.SetBool(isGroundedParameter, controller.IsGrounded());
            animator.SetBool(isCrouchedParameter, controller.IsCrouched());

            if (controller.IsJumped())
            {
                animator.SetTrigger(isJumpedParameter);
            }
        }

        private void RotateHandler()
        {
            if (angleTolerance > 0)
            {
                if (!controller.IsMoving())
                {
                    float angle = Vector3.Angle(controller.transform.forward, eulerAngles);
                    if (Mathf.Abs(angle) > angleTolerance)
                    {
                        int axis = yRotation > transform.eulerAngles.y ? 1 : -1;
                        yRotation = transform.eulerAngles.y;
                        syncRotationCoroutine.Start(SyncRotation, axis, true);
                    }
                    else if (!syncRotationCoroutine.IsProcessing())
                    {
                        transform.rotation = Quaternion.LookRotation(eulerAngles);
                    }
                }
                else if (!syncRotationCoroutine.IsProcessing())
                {
                    transform.localRotation = Quaternion.identity;
                }
            }
        }

        private IEnumerator SyncRotation(int axis)
        {
            eulerAngles = controller.transform.forward;
            animator.SetInteger(turnAxisParameter, axis);


            float time = 0.0f;
            float speed = 1 / rotateDuration;
            while (time < 1.0f)
            {
                time += speed * Time.deltaTime;
                float smooth = rotateCurve.Evaluate(time);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(eulerAngles), smooth);
                yield return null;
                animator.SetInteger(turnAxisParameter, 0);
            }
        }
    }
}