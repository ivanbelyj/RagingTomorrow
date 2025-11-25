/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.AnimatorStateBehaviours
{
    [HideScriptField]
    public sealed class UpdateParameter : StateMachineBehaviour
    {
        public enum Callback
        {
            OnStateMachineEnter,
            OnStateMachineExit,
            OnStateEnter,
            OnStateExit,
            OnStateIK,
            OnStateMove,
            OnStateUpdate
        }

        [SerializeField]
        private Callback callback = Callback.OnStateEnter;

        [SerializeField]
        private AnimatorControllerParameterType type;

        [SerializeField]
        [Label("Name")]
        private AnimatorParameter parameter;

        [SerializeField]
        [Label("Value")]
        [VisibleIf("type", "Float")]
        private float floatValue;

        [SerializeField]
        [Label("Value")]
        [VisibleIf("type", "Int")]
        private int intValue;

        [SerializeField]
        [Label("Value")]
        [VisibleIf("type", "Bool")]
        private bool boolValue;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if(callback == Callback.OnStateEnter)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter, floatValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter, intValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter, boolValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (boolValue)
                        {
                            animator.SetTrigger(parameter);
                        }
                        else
                        {
                            animator.ResetTrigger(parameter);
                        }
                        break;
                }
            }
            
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            if (callback == Callback.OnStateExit)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter, floatValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter, intValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter, boolValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (boolValue)
                        {
                            animator.SetTrigger(parameter);
                        }
                        else
                        {
                            animator.ResetTrigger(parameter);
                        }
                        break;
                }
            }
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateIK(animator, stateInfo, layerIndex);
            if (callback == Callback.OnStateIK)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter, floatValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter, intValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter, boolValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (boolValue)
                        {
                            animator.SetTrigger(parameter);
                        }
                        else
                        {
                            animator.ResetTrigger(parameter);
                        }
                        break;
                }
            }
        }

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);
            if (callback == Callback.OnStateMachineEnter)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter, floatValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter, intValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter, boolValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (boolValue)
                        {
                            animator.SetTrigger(parameter);
                        }
                        else
                        {
                            animator.ResetTrigger(parameter);
                        }
                        break;
                }
            }
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            Debug.Log("Exit");
            base.OnStateMachineExit(animator, stateMachinePathHash);
            if (callback == Callback.OnStateMachineExit)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter, floatValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter, intValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter, boolValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (boolValue)
                        {
                            animator.SetTrigger(parameter);
                        }
                        else
                        {
                            animator.ResetTrigger(parameter);
                        }
                        break;
                }
            }
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);
            if (callback == Callback.OnStateMove)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter, floatValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter, intValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter, boolValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (boolValue)
                        {
                            animator.SetTrigger(parameter);
                        }
                        else
                        {
                            animator.ResetTrigger(parameter);
                        }
                        break;
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (callback == Callback.OnStateUpdate)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter, floatValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter, intValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter, boolValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (boolValue)
                        {
                            animator.SetTrigger(parameter);
                        }
                        else
                        {
                            animator.ResetTrigger(parameter);
                        }
                        break;
                }
            }
        }
    }
}