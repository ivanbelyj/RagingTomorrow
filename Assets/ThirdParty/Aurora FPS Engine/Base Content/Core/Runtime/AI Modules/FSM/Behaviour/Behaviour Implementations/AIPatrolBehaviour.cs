/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Mathematics;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [AIBehaviourMenu("Patrol", "Locomotion/Patrol")]
    public class AIPatrolBehaviour : AIBehaviour
    {
        public enum LoadMap
        {
            Automatically,
            Manual
        }

        public enum PatrolType
        {
            /// <summary>
            /// Constantly selected at random from destination info array a new destination point.
            /// </summary>
            Random,

            /// <summary>
            /// Walking through all the destination points in turn circle after circle.
            /// </summary>
            Sequential,

            /// <summary>
            /// Walking through all the destination points and stop on the last.
            /// </summary>
            Finite
        }

        [SerializeField]
        private PatrolType patrolType = PatrolType.Sequential;

        [SerializeField]
        private LoadMap loadMap = LoadMap.Manual;

        [SerializeField]
        [VisibleIf("loadMap", "Automatically")]
        private string mapName = string.Empty;

        [SerializeField]
        [VisibleIf("loadMap", "Manual")]
        private WayPointMap map;

        [SerializeField]
        [MinValue(0.0f)]
        private float tolerance = 0.125f;

        [SerializeField]
        [CustomView(ViewInitialization = "OnTargetBehaviourInitialization", ViewGUI = "OnTargetBehaviourGUI")]
        [Foldout("Default Transition")]
        [Message("Execute transition to target behaviour, when Finite patrol type is finished.")]
        private string targetBehaviour = AIIdleBehaviour.IDLE_BEHAVIOUR;

        // Stored required properties.
        private int currentIndex;

        /// <summary>
        /// Called when AIController owner instance is being loaded.
        /// </summary>
        protected override void OnInitialize()
        {
            if(loadMap == LoadMap.Automatically)
            {
                GameObject mapGameObject = GameObject.Find(mapName);
                map = mapGameObject.GetComponent<WayPointMap>();
            }
        }

        /// <summary>
        /// Called when this behaviour becomes enabled.
        /// </summary>
        protected override void OnEnable()
        {
            currentIndex = 0;
        }

        /// <summary>
        /// Called every frame, while this behaviour is running.
        /// </summary>
        protected override void Update()
        {
            if (map != null)
            {
                Vector3 position = map.GetPointPosition(currentIndex);
                if (Math.Distance2D(owner.transform.position, position) <= tolerance)
                {
                    switch (patrolType)
                    {
                        case PatrolType.Random:
                            currentIndex = Random.Range(0, map.GetPointCount());
                            break;
                        case PatrolType.Sequential:
                            currentIndex = Math.Loop(currentIndex + 1, 0, map.GetPointCount() - 1);
                            break;
                        case PatrolType.Finite:
                            int lastIndex = map.GetPointCount() - 1;

                            if (currentIndex == lastIndex)
                            {
                                owner.SwitchBehaviour(targetBehaviour);
                            }

                            currentIndex = Mathf.Clamp(currentIndex + 1, 0, lastIndex);
                            break;
                    }
                }
                else
                {
                    owner.SetDestination(position);
                }
            }
            else
            {
                owner.SwitchBehaviour(targetBehaviour);
            }
        }
    }
}