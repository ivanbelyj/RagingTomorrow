/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree;
using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Nodes;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public class TreeNodeView : Node
    {
        public Action<TreeNodeView> OnNodeSelected;

        private TreeNode node;
        public Port input;
        public Port output;

        private object target;
        private MethodInfo methodInfo;
        private PropertyInfo propertyInfo;

        private VisualElement muteBlock;

        public TreeNodeView(TreeNode node) : base(Path.Combine(ApexSettings.Current.GetRootPath(), BehaviourTreeEditor.NodeUxmlRelativePath))
        {
            this.target = node;
            this.node = node;
            this.node.name = node.GetType().Name;
             
            TreeNodeContentAttribute attribute = ApexReflection.GetAttribute<TreeNodeContentAttribute>(node.GetType());
            if (attribute != null)
            {
                string[] splited = attribute.Name.Split('/', '\\');
                this.title = splited[splited.Length - 1];
            }
            else
            {
                this.title = node.name;
            }

            if (attribute != null && attribute.Name[0] == '@')
            {
                string methodName = attribute.Name.Replace("@", "");
                MethodInfo[] methodInfos = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                for (int i = 0; i < methodInfos.Length; i++)
                {
                    MethodInfo methodInfo = methodInfos[i];
                    if (methodInfo.Name == methodName)
                    {
                        this.methodInfo = methodInfo;
                        break;
                    }
                }

                PropertyInfo[] propertyInfos = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    PropertyInfo propertyInfo = propertyInfos[i];
                    if (propertyInfo.Name == methodName)
                    {
                        this.propertyInfo = propertyInfo;
                        break;
                    }
                }
            }

            muteBlock = this.Q<VisualElement>("mute-block");

            OnInspectorUpdate();

            this.viewDataKey = node.GetInstanceID().ToString();

            style.left = node.nodePosition.x;
            style.top = node.nodePosition.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            SetupDataBinding();
        }

        private void SetupDataBinding()
        {
            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "nodeDescription";
            descriptionLabel.Bind(new SerializedObject(node));
        }

        /// <summary>
        /// Setting up Primary Classes.
        /// </summary>
        private void SetupClasses()
        {
            if (node is BehaviourNode)
            {
                AddToClassList("behaviour");
            }
            else if (node is CompositeNode)
            {
                AddToClassList("composite");
            }
            else if (node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
            else if (node is RootNode)
            {
                AddToClassList("root");
            }
            else if (node is ActionNode)
            {
                AddToClassList("action");
            }
        }

        /// <summary>
        /// Creates node view input ports.
        /// </summary>
        private void CreateInputPorts()
        {
            if (node is CompositeNode)
            {
                input = new NodePort(Direction.Input, Port.Capacity.Single);
            }
            else if (node is DecoratorNode)
            {
                input = new NodePort(Direction.Input, Port.Capacity.Single);
            }
            else if (node is ActionNode)
            {
                input = new NodePort(Direction.Input, Port.Capacity.Single);
            }

            if (input != null)
            {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }

        /// <summary>
        /// Creates node view output ports.
        /// </summary>
        private void CreateOutputPorts()
        {
            if (node is CompositeNode)
            {
                output = new NodePort(Direction.Output, Port.Capacity.Multi);
            }
            else if (node is DecoratorNode)
            {
                output = new NodePort(Direction.Output, Port.Capacity.Single);
            }
            else if (node is RootNode)
            {
                output = new NodePort(Direction.Output, Port.Capacity.Single);
            }

            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }

        /// <summary>
        /// Save the change in the position of the node view.
        /// </summary>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree (SetPosition)");
            node.nodePosition.x = newPos.xMin;
            node.nodePosition.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        /// <summary>
        /// Selecting a node to display it in the inspector.
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            OnNodeSelected?.Invoke(null);
        }

        public void OnInspectorUpdate()
        {
            if (methodInfo != null)
            {
                title = methodInfo.Invoke(target, null) as string;
            }
            else if (propertyInfo != null)
            {
                title = propertyInfo.GetValue(target, null) as string;
            }

            muteBlock.style.display = node.mute ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Sort child nodes horizontally. 
        /// </summary>
        public void SortChildren()
        {
            if (node is CompositeNode compositeNode)
            {
                compositeNode.GetChildren().Sort(SortByHorizontalPosition);
            }
        }

        /// <summary>
        /// Compare nodes horizontally.
        /// </summary>
        private int SortByHorizontalPosition(TreeNode left, TreeNode right)
        {
            return left.nodePosition.x < right.nodePosition.x ? -1 : 1;
        }

        #region [Runtime Visualization]
        /// <summary>
        /// Updating state in runtime.
        /// </summary>
        public void UpdateState()
        {
            UpdateNodeState();
            UpdateEdgeState();
        }

        private void UpdateNodeState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (node.mute) return;

            switch (node.GetState())
            {
                case State.Running:
                    if (node.IsStarted())
                    {
                        AddToClassList("running");
                    }
                    break;
                case State.Failure:
                    AddToClassList("failure");
                    break;
                case State.Success:
                    AddToClassList("success");
                    break;
            }
        }

        private void UpdateEdgeState()
        {
            foreach (var portItem in inputContainer.Children())
            {
                Port port = portItem as Port;
                if (port != null)
                {
                    foreach (var edgeItem in port.connections)
                    {
                        Edge edge = edgeItem as Edge;
                        if (edge != null)
                        {
                            if (node.GetState() == State.Running && node.IsStarted())
                            {
                                edge.edgeControl.inputColor = Color.yellow;
                                edge.edgeControl.outputColor = Color.yellow;
                            }
                            else
                            {
                                edge.edgeControl.inputColor = Color.white;
                                edge.edgeControl.outputColor = Color.white;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region [Getter / Setter]
        public TreeNode GetNode()
        {
            return node;
        }

        public void SetNode(TreeNode node)
        {
            this.node = node;
        }
        #endregion
    }
}