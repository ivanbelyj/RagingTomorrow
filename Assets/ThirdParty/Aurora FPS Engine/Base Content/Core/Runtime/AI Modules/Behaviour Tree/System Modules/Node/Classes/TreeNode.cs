/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [HideInInspector]
    public abstract class TreeNode : ScriptableObject
    {
        protected BehaviourTreeRunner owner;
        protected BehaviourTreeAsset tree;

        // Stored required properties.
        private State state = State.Running;
        private bool started = false;

        [Order(900)]
        public bool mute;

        #region [Editor Section]
#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 nodePosition;

        [UnityEngine.TextArea]
        [Label("Description")]
        [Order(901)]
        public string nodeDescription;

        /// <summary>
        /// Initializes the tree to work in the editor.
        /// </summary>
        internal void InitializeTree(BehaviourTreeAsset tree)
        {
            this.tree = tree;
        }
#endif
        #endregion

        /// <summary>
        /// Called when initializing the behavioour tree.
        /// </summary>
        public void Initialize(BehaviourTreeRunner owner, BehaviourTreeAsset tree)
        {
            this.owner = owner;
            this.tree = tree;

            AddRequiredComponents();
            OnInitialize();
        }

        /// <summary>
        /// Called when updating a node from the behaviour tree.
        /// </summary>
        public virtual State Update()
        {
            if (!started)
            {
                OnEntry();
                started = true;
            }

            state = OnUpdate();

            if (state == State.Success || state == State.Failure)
            {
                OnExit();
                started = false;
            }

            return state;
        }

        /// <summary>
        /// Abort the node operation.
        /// </summary>
        internal void Abort()
        {
            BehaviourTreeAsset.Traverse(this, node =>
            {
                BehaviourNode behaviourNode = node as BehaviourNode;
                if (behaviourNode != null)
                {
                    behaviourNode.GetBehaviour().GetRootNode().Abort();
                }

                node.state = State.Running;
                if (node.started)
                {
                    node.started = false;
                    node.OnExit();
                }
            });
        }

        /// <summary>
        /// Called when initializing a node.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Called when initializing a node.
        /// </summary>
        protected virtual void OnEntry() { }

        /// <summary>
        /// Called when updating the node.
        /// </summary>
        protected abstract State OnUpdate();

        /// <summary>
        /// Called when exiting the node.
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// Returns a clone of itself.
        /// </summary>
        public virtual TreeNode Clone()
        {
            return Instantiate(this);
        }

        /// <summary>
        /// Implement OnDrawGizmos to draw a gizmo.
        /// </summary>
        public virtual void OnDrawGizmos() { }

        /// <summary>
        /// Adds the required components to the object that runs the tree.
        /// </summary>
        private void AddRequiredComponents()
        {
            IEnumerable<RequireComponent> requireComponents = GetType().GetCustomAttributes<RequireComponent>(true);
            foreach (RequireComponent requireComponent in requireComponents)
            {
                if (requireComponent.m_Type0 != null && owner.GetComponent(requireComponent.m_Type0) == null)
                {
                    if (!requireComponent.m_Type0.IsInterface && !requireComponent.m_Type0.IsAbstract)
                    {
                        owner.gameObject.AddComponent(requireComponent.m_Type0);
                        Debug.LogWarning($"The <color=red><b>{requireComponent.m_Type0.Name}</b></color> component was added to the <b>{owner.name}</b> object from the <b>{tree.name}</b> tree.");
                    }
                }

                if (requireComponent.m_Type1 != null && owner.GetComponent(requireComponent.m_Type1) == null)
                {
                    if (!requireComponent.m_Type0.IsInterface && !requireComponent.m_Type0.IsAbstract)
                    {
                        owner.gameObject.AddComponent(requireComponent.m_Type1);
                        Debug.LogWarning($"The <color=red><b>{requireComponent.m_Type0.Name}</b></color> component was added to the <b>{owner.name}</b> object from the <b>{tree.name}</b> tree.");
                    }
                }

                if (requireComponent.m_Type2 != null && owner.GetComponent(requireComponent.m_Type2) == null)
                {
                    if (!requireComponent.m_Type0.IsInterface && !requireComponent.m_Type0.IsAbstract)
                    {
                        owner.gameObject.AddComponent(requireComponent.m_Type2);
                        Debug.LogWarning($"The <color=red><b>{requireComponent.m_Type0.Name}</b></color> component was added to the <b>{owner.name}</b> object from the <b>{tree.name}</b> tree.");
                    }
                }
            }
        }

        #region [Getter / Setter]
        public BehaviourTreeRunner GetOwner()
        {
            return owner;
        }

        public BehaviourTreeAsset GetBehaviourTree()
        {
            return tree;
        }

        public State GetState()
        {
            return state;
        }

        public bool IsStarted()
        {
            return started;
        }
        #endregion
    }
}