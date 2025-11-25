/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree
{
    public class TreeGroup : ScriptableObject
    {
        #region [Editor Section]
#if UNITY_EDITOR
        [HideInInspector]
        public string title;

        [SerializeReference]
        [HideInInspector]
        public List<TreeNode> nodes = new List<TreeNode>();

        [HideInInspector]
        public BehaviourTreeAsset tree;
#endif
        #endregion

    }
}