/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree;
using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/Behaviour Tree/Behaviour Tree Runner")]
    [DisallowMultipleComponent]
    public class BehaviourTreeRunner : MonoBehaviour, IBehaviourTree
    {
        [SerializeField]
        [Label("Behaviour Tree")]
        [NotNull]
        private BehaviourTreeAsset sharedBehaviourTree = null;

        // Stored required properties.
        private BehaviourTreeAsset behaviourTree;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            behaviourTree = sharedBehaviourTree.Clone();
            behaviourTree.Initialize(this);

            if (behaviourTree.GetCallType() == BehaviourTreeAsset.CallType.Awake)
            {
                behaviourTree.Update();
            }
        }

        protected virtual void OnEnable()
        {
            if (behaviourTree.GetCallType() == BehaviourTreeAsset.CallType.OnEnable)
            {
                behaviourTree.Update();
            }
        }

        ///<summary>
        /// Called on the frame when a script is enabled,
        /// just before any of the Update methods are called the first time.
        ///</summary>
        protected virtual void Start()
        {
            if (behaviourTree.GetCallType() == BehaviourTreeAsset.CallType.Start)
            {
                behaviourTree.Update();
            }
        }

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (behaviourTree.GetCallType() == BehaviourTreeAsset.CallType.Update)
            {
                behaviourTree.Update();
            }
        }

        protected virtual void OnDisable()
        {
            behaviourTree.GetRootNode().Abort();

            if (behaviourTree.GetCallType() == BehaviourTreeAsset.CallType.OnDisable)
            {
                behaviourTree.Update();
            }
        }

        /// <summary>
        /// Return shared behaviour tree;
        /// </summary>
        public BehaviourTreeAsset GetSharedBehaviourTree()
        {
            return sharedBehaviourTree;
        }

        /// <summary>
        /// Gizmos are drawn only when the object is selected.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (behaviourTree != null)
            {
                BehaviourTreeAsset.Traverse(behaviourTree.GetRootNode(), (n) =>
                {
                    n.OnDrawGizmos();
                });
            }
        }

        #region [IBehaviourTree Implementation]
        public BehaviourTreeAsset GetBehaviourTree()
        {
            return behaviourTree;
        }
        #endregion
    }
}