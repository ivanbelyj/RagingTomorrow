/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Nodes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree
{
    [HideScriptField]
    public sealed class BehaviourTreeAsset : ScriptableObject
    {
        public enum CallType
        {
            Awake,
            OnEnable,
            Start,
            Update,
            OnDisable
        }

        [SerializeField]
        private CallType callType = CallType.Update;

        [SerializeField]
        private new string name;

        [SerializeField]
        private bool singleCall;

        [SerializeField]
        [UnityEngine.TextArea]
        private string description;

        [SerializeReference]
        [HideInInspector]
        private TreeNode rootNode = null;

        [SerializeField]
        [HideInInspector]
        private VariablesContainer variables = new VariablesContainer();

        private State state = State.Running;

        #region [Editor Section]
#if UNITY_EDITOR
        [SerializeReference]
        [HideInInspector]
        public List<TreeNode> nodes = new List<TreeNode>();

        [SerializeReference]
        [HideInInspector]
        public List<TreeGroup> groups = new List<TreeGroup>();

        private void OnEnable()
        {
            InitializeNodes();
        }

        public void InitializeNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].InitializeTree(this);
            }
        }
#endif
        #endregion

        /// <summary>
        /// Initialization of the tree, called in ai controller Awake.
        /// </summary>
        public void Initialize(BehaviourTreeRunner owner)
        {
            Traverse(rootNode, (n) =>
            {
                n.Initialize(owner, this);
            });
        }

        /// <summary>
        /// Updating the behaviour tree.
        /// </summary>
        public State Update()
        {
            if (state == State.Running || !singleCall)
            {
                state = rootNode.Update();
            }
            return state;
        }

        /// <summary>
        /// Returns a clone of itself.
        /// </summary>
        public BehaviourTreeAsset Clone()
        {
            BehaviourTreeAsset tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
#if UNITY_EDITOR
            tree.nodes = new List<TreeNode>();
            Traverse(tree.rootNode, (n) =>
            {
                tree.nodes.Add(n);
            });
#endif
            return tree;
        }

        #region [Static Methods]
        /// <summary>
        /// Return the child nodes of the node.
        /// </summary>
        public static List<TreeNode> GetChildren(TreeNode parent)
        {
            CompositeNode compositeNode = parent as CompositeNode;
            if (compositeNode != null)
            {
                return compositeNode.GetChildren();
            }

            List<TreeNode> children = new List<TreeNode>();
            DecoratorNode decoratorNode = parent as DecoratorNode;
            if (decoratorNode != null && decoratorNode.GetChild() != null)
            {
                children.Add(decoratorNode.GetChild());
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode != null && rootNode.GetChild() != null)
            {
                children.Add(rootNode.GetChild());
            }

            return children;
        }

        /// <summary>
        /// Performs an action for all child classes.
        /// </summary>
        public static void Traverse(TreeNode node, System.Action<TreeNode> visiter)
        {
            if (node != null)
            {
                visiter?.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }
        #endregion

        #region [Variables Methods]
        public void AddVariable(string name, TreeVariable variable)
        {
            string[] variableName = name.Split('/', '\\');

            switch (variableName[0])
            {
                case "Local":
                    variables[variableName[1]] = variable;
                    break;
                case "Global":
                    GlobalVariablesStorage.Current[variableName[1]] = variable;
                    break;
            }

        }

        public void AddVariable<T>(string name, T variable) where T : TreeVariable
        {
            string[] variableName = name.Split('/', '\\');

            switch (variableName[0])
            {
                case "Local":
                    if (variable is T localValue)
                    {
                        variables[variableName[1]] = localValue;
                    }
                    break;
                case "Global":
                    if (variable is T globalValue)
                    {
                        GlobalVariablesStorage.Current[variableName[1]] = globalValue;
                    }
                    break;
            }
        }

        public bool TryGetVariable(string name, out TreeVariable variable)
        {
            string[] variableName = name.Split('/', '\\');
            switch (variableName[0])
            {
                case "Local":
                    return variables.TryGetValue(variableName[1], out variable);
                case "Global":
                    return GlobalVariablesStorage.Current.TryGetVariable(variableName[1], out variable);
            }

            variable = null;
            return false;
        }

        public bool TryGetVariable<T>(string name, out T variable) where T : TreeVariable
        {
            string[] variableName = name.Split('/', '\\');
            switch (variableName[0])
            {
                case "Local":
                    if (variables.TryGetValue(variableName[1], out TreeVariable localValue))
                    {
                        variable = localValue as T;
                        if (variable != null)
                        {
                            return true;
                        }
                    }
                    break;
                case "Global":
                    if (GlobalVariablesStorage.Current.TryGetVariable(variableName[1], out TreeVariable globalValue))
                    {
                        variable = globalValue as T;
                        if (variable != null)
                        {
                            return true;
                        }
                    }
                    break;
            }

            variable = null;
            return false;
        }

        public TreeVariable GetVariable(string name)
        {
            string[] variableName = name.Split('/', '\\');
            switch (variableName[0])
            {
                case "Local":
                    if (variables.TryGetValue(variableName[1], out TreeVariable localVariable))
                    {
                        return localVariable;
                    }
                    break;
                case "Global":
                    if (GlobalVariablesStorage.Current.TryGetVariable(variableName[1], out TreeVariable globalVariable))
                    {
                        return globalVariable;
                    }
                    break;
            }

            return null;
        }

        public T GetVariable<T>(string name) where T : TreeVariable
        {
            string[] variableName = name.Split('/', '\\');
            switch (variableName[0])
            {
                case "Local":
                    if (variables.TryGetValue(variableName[1], out TreeVariable localVariable))
                    {
                        return localVariable as T;
                    }
                    break;
                case "Global":
                    if (variables.TryGetValue(variableName[1], out TreeVariable globalVariable))
                    {
                        return globalVariable as T;
                    }
                    break;
            }

            return null;
        }

        public IEnumerable<KeyValuePair<string, TreeVariable>> LocalVariables
        {
            get
            {
                foreach (var item in variables)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, TreeVariable>> GlobalVariables
        {
            get
            {
                return GlobalVariablesStorage.Current.Variables;
            }
        }

        public void RemoveVariable(string name)
        {
            string[] variableName = name.Split('/', '\\');
            switch (variableName[0])
            {
                case "Local":
                    variables.Remove(variableName[1]);
                    break;
                case "Global":
                    GlobalVariablesStorage.Current.RemoveVariable(variableName[1]);
                    break;
            }
        }
        #endregion

        #region [Getter / Setter]
        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetDescription()
        {
            return description;
        }

        public void SetDescription(string description)
        {
            this.description = description;
        }

        public TreeNode GetRootNode()
        {
            return rootNode;
        }

        public void SetRootNode(TreeNode node)
        {
            rootNode = node;
        }

        public CallType GetCallType()
        {
            return callType;
        }
        #endregion
    }
}