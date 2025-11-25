///* ================================================================
//   ----------------------------------------------------------------
//   Project   :   Aurora FPS Engine
//   Publisher :   Renowned Games
//   Developer :   Tamerlan Shakirov
//   ----------------------------------------------------------------
//   Copyright 2022 Renowned Games All rights reserved.
//   ================================================================ */

//using UnityEngine;

//namespace AuroraFPSRuntime.AIModules.Conditions
//{
//    [RequireComponent(typeof(AIFieldOfView))]
//    [ConditionMenu("On Lost Target(Transform)", "Field Of View/On Lost Target(Transform)", Description = "Called when the AI loses a target from view.")]
//    public class OnLostTargetTransfromCondition : Condition
//    {
//        [SerializeField] private Transform target;

//        private bool hasTarget;
//        private AIFieldOfView fieldOfView;


//        /// <summary>
//        /// Initializing required properties.
//        /// </summary>
//        /// <param name="core">Target AIController instance.</param>
//        protected override void OnInitialize(AIController core)
//        {
//            fieldOfView = core.GetComponent<AIFieldOfView>();
//        }

//        /// <summary>
//        /// Condition for translate to the next AI behaviour.
//        /// </summary>
//        public override bool IsExecuted()
//        {
//            bool stillHasTarget = false;
//            for (int i = 0; i < fieldOfView.GetVisibleTargetCount(); i++)
//            {
//                if (fieldOfView.GetVisibleTarget(i) == target)
//                {
//                    hasTarget = true;
//                    stillHasTarget = true;
//                }
//            }
//            return hasTarget && !stillHasTarget;
//        }

//        #region [Getter / Setter]
//        public Transform GetTarget()
//        {
//            return target;
//        }

//        public void SetTarget(Transform value)
//        {
//            target = value;
//        }
//        #endregion
//    }
//}