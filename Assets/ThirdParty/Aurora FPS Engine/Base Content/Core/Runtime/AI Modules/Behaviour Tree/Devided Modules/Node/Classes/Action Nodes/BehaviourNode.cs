/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("@GetBehaviourName", "Custom/Behaviour")]
    [HideScriptField]
    public class BehaviourNode : ActionNode
    {
        [SerializeField]
        private BehaviourTreeAsset behaviourTree;

        protected override void OnInitialize()
        {
            behaviourTree.Initialize(owner);
        }

        protected override State OnUpdate()
        {
            return behaviourTree.Update();
        }

        private string GetBehaviourName()
        {
            return behaviourTree?.name ?? "Behaviour";
        }

        /// <summary>
        /// Implements cloning for BehaviourNode.
        /// </summary>
        public override TreeNode Clone()
        {
            BehaviourNode node = Instantiate(this);
            node.behaviourTree = behaviourTree.Clone();
            return node;
        }

        #region [Getter / Setter]
        public BehaviourTreeAsset GetBehaviour()
        {
            return behaviourTree;
        }
        #endregion
    }
}