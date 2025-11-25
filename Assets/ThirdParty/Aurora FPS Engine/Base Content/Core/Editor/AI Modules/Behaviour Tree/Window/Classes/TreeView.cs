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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public class TreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<TreeView, GraphView.UxmlTraits> { }

        public Action<TreeNodeView> OnNodeSelected;

        private BehaviourTreeAsset tree;
        private bool autoSorting;

        public TreeView()
        {
            name = "Behaviour Tree";

            Insert(0, new GridBackground());

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new DoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            ApexSettings settings = ApexSettings.Current;
            string treeUxmlPath = Path.Combine(settings.GetRootPath(), BehaviourTreeEditor.UssRelativePath);
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(treeUxmlPath);
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        public virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        /// <summary>
        /// Undo/Redo Callback
        /// </summary>
        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Finds a TreeNodeView with TreeNode.
        /// </summary>
        public TreeNodeView FindNodeView(TreeNode node)
        {
            TreeNodeView nodeView = null;
            try
            {
                nodeView = GetNodeByGuid(node.GetInstanceID().ToString()) as TreeNodeView;
            }
            catch
            {

            }
            return nodeView;
        }

        /// <summary>
        /// Fills in the graph view.
        /// </summary>
        public void PopulateView(BehaviourTreeAsset tree)
        {
            this.tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (tree.GetRootNode() == null)
            {
                tree.SetRootNode(OnCreateNode(typeof(RootNode)) as RootNode);
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            tree.InitializeNodes();

            // Create node view
            tree.nodes.ForEach(n => AddElement(CreateNodeView(n)));

            // Create edges
            tree.nodes.ForEach(n =>
            {
                List<TreeNode> children = BehaviourTreeAsset.GetChildren(n);
                children.ForEach(c =>
                {
                    TreeNodeView parentView = FindNodeView(n);
                    TreeNodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });

            // Delete empty groups
            //List<TreeGroup> deleteGroups = tree.groups.Where(g => g.nodes.Count == 0).ToList();
            //for (int i = 0; i < deleteGroups.Count; i++)
            //{
            //    OnDeleteGroup(deleteGroups[i]);
            //}

            // Create groups view
            //tree.groups.ForEach(g =>
            //{
            //    List<TreeNodeView> nodeViews = g.nodes.Select(n => FindNodeView(n)).ToList();
            //    TreeGroupView groupView = CreateGroupView(g);
            //    groupView.AddElements(nodeViews);
            //    AddElement(groupView);
            //});
        }

        public void ClearTree()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
        }

        /// <summary>
        /// Get all ports compatible with given port.
        /// </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }

        /// <summary>
        /// Called when the graph is changed in the graph editor.
        /// </summary>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    TreeNodeView nodeView = element as TreeNodeView;
                    if (nodeView != null)
                    {
                        OnDeleteNode(nodeView.GetNode());
                    }

                    Edge edge = element as Edge;
                    if (edge != null)
                    {
                        TreeNodeView parentView = edge.output.node as TreeNodeView;
                        TreeNodeView childView = edge.input.node as TreeNodeView;
                        OnRemoveChild(parentView.GetNode(), childView.GetNode());
                    }

                    //TreeGroupView groupView = element as TreeGroupView;
                    //if (groupView != null)
                    //{
                    //    OnDeleteGroup(groupView.GetGroup());
                    //}
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    TreeNodeView parentView = edge.output.node as TreeNodeView;
                    TreeNodeView childView = edge.input.node as TreeNodeView;
                    OnAddChild(parentView.GetNode(), childView.GetNode());
                });
            }

            nodes.ForEach((n) =>
            {
                TreeNodeView view = n as TreeNodeView;
                view.SortChildren();
            });

            if (autoSorting)
            {
                SortTree();
            }

            return graphViewChange;
        }

        /// <summary>
        /// Add menu items to the contextual menu.
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (tree == null) return;

            Vector2 position = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);

            var types = TypeCache.GetTypesDerivedFrom<TreeNode>();
            foreach (Type type in types)
            {
                if (type.IsAbstract || type.IsGenericType)
                {
                    continue;
                }

                TreeNodeContentAttribute attribute = ApexReflection.GetAttribute<TreeNodeContentAttribute>(type);
                if (attribute != null && !attribute.Hide)
                {
                    evt.menu.AppendAction(string.Format(attribute?.Path ?? type.Name), (a) => CreateNode(type, position));
                }
            }

            evt.menu.AppendSeparator();

            //evt.menu.AppendAction("Add Group", (a) => CreateGroup("New Group", position));
        }

        /// <summary>
        /// Create a node view to display in the graph.
        /// </summary>
        private TreeNodeView CreateNodeView(TreeNode node, bool autoFocus = false)
        {
            TreeNodeView nodeView = new TreeNodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            if (autoFocus)
            {
                nodeView.OnSelected();
            }
            return nodeView;
        }

        /// <summary>
        /// Create a group view to display in the graph.
        /// </summary>
        //private TreeGroupView CreateGroupView(TreeGroup group)
        //{
        //    TreeGroupView groupView = new TreeGroupView(group);
        //    groupView.GetGroup().tree = tree;
        //    return groupView;
        //}

        #region [Runtime Visualization]
        /// <summary>
        /// Updating node states in runtime.
        /// </summary>
        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                TreeNodeView view = n as TreeNodeView;
                view.UpdateState();
            });
        }
        #endregion

        #region [Behaviour Tree Management]
        /// <summary>
        /// Create a group and its view.
        /// </summary>
        //private void CreateGroup(string title, Vector2 position)
        //{
        //    TreeGroup group = OnCreateGroup(title);
        //    TreeGroupView groupView = CreateGroupView(group);
        //    groupView.SetPosition(new Rect(position, Vector2.zero));
        //    AddElement(groupView);
        //}

        /// <summary>
        /// Create a group.
        /// </summary>
        //private TreeGroup OnCreateGroup(string title)
        //{
        //    TreeGroup group = ScriptableObject.CreateInstance(typeof(TreeGroup)) as TreeGroup;
        //    group.title = title;
        //    group.name = "Group";

        //    Undo.RecordObject(tree, "Behaviour Tree (CreateGroup)");
        //    tree.groups.Add(group);

        //    if (!Application.isPlaying)
        //    {
        //        AssetDatabase.AddObjectToAsset(group, tree);
        //    }

        //    Undo.RegisterCreatedObjectUndo(group, "Behaviour Tree (CreateGroup)");

        //    AssetDatabase.SaveAssets();
        //    return group;
        //}

        /// <summary>
        /// Delete a group.
        /// </summary>
        //private void OnDeleteGroup(TreeGroup group)
        //{
        //    tree.groups.Remove(group);

        //    //AssetDatabase.RemoveObjectFromAsset(group);
        //    Undo.DestroyObjectImmediate(group);

        //    AssetDatabase.SaveAssets();
        //}

        /// <summary>
        /// Create a node and its view.
        /// </summary>
        public void CreateNode(Type type, Vector2 position)
        {
            TreeNode node = OnCreateNode(type);
            node.nodePosition = position;
            AddElement(CreateNodeView(node, true));
        }

        /// <summary>
        /// Create a node.
        /// </summary>
        private TreeNode OnCreateNode(Type type)
        {
            TreeNode node = ScriptableObject.CreateInstance(type) as TreeNode;
            node.name = type.Name;

            Undo.RecordObject(tree, "Behaviour Tree (CreateNode)");
            tree.nodes.Add(node);
            tree.InitializeNodes();


            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, tree);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;
        }

        /// <summary>
        /// Delete a node.
        /// </summary>
        private void OnDeleteNode(TreeNode node)
        {
            Undo.RecordObject(tree, "Behaviour Tree (DeleteNode)");
            tree.nodes.Remove(node);

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Сonnect the 2 nodes to each other.
        /// </summary>
        private void OnAddChild(TreeNode parent, TreeNode child)
        {
            CompositeNode compositeNode = parent as CompositeNode;
            if (compositeNode != null)
            {
                Undo.RecordObject(compositeNode, "Behaviour Tree (AddChild)");
                compositeNode.AddChild(child);
                EditorUtility.SetDirty(compositeNode);
            }

            DecoratorNode decoratorNode = parent as DecoratorNode;
            if (decoratorNode != null)
            {
                Undo.RecordObject(decoratorNode, "Behaviour Tree (AddChild)");
                decoratorNode.SetChild(child);
                EditorUtility.SetDirty(decoratorNode);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode != null)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
                rootNode.SetChild(child);
                EditorUtility.SetDirty(rootNode);
            }
        }

        /// <summary>
        /// Disconnect the 2 nodes from each other.
        /// </summary>
        private void OnRemoveChild(TreeNode parent, TreeNode child)
        {
            CompositeNode compositeNode = parent as CompositeNode;
            if (compositeNode != null)
            {
                Undo.RecordObject(compositeNode, "Behaviour Tree (RemoveChild)");
                compositeNode.RemoveChild(child);
                EditorUtility.SetDirty(compositeNode);
            }

            DecoratorNode decoratorNode = parent as DecoratorNode;
            if (decoratorNode != null)
            {
                Undo.RecordObject(decoratorNode, "Behaviour Tree (RemoveChild)");
                decoratorNode.SetChild(null);
                EditorUtility.SetDirty(decoratorNode);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode != null)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
                rootNode.SetChild(null);
                EditorUtility.SetDirty(rootNode);
            }
        }

        #endregion

        #region [Tree Sorting]
        /// <summary>
        /// 
        /// </summary>
        public void SortTree()
        {
            if (tree != null && tree.GetRootNode() != null)
            {
                OnSortTree(tree.GetRootNode(), 0, 0, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Vector2 OnSortTree(TreeNode node, float minX, float maxX, float y)
        {
            TreeNodeView nodeView = FindNodeView(node);
            Vector2 result = new Vector2(minX, maxX);
            Rect posRect = nodeView.GetPosition();

            if (node is CompositeNode compositeNode)
            {
                List<TreeNode> children = compositeNode.GetChildren();
                for (int i = 0; i < children.Count; i++)
                {
                    Vector2 res;
                    if (i == 0)
                    {
                        res = OnSortTree(children[i], maxX, maxX, y + 1);
                    }
                    else
                    {
                        res = OnSortTree(children[i], maxX + 1, maxX + 1, y + 1);
                    }

                    if (maxX < res.y) maxX = res.y;
                }
                result = new Vector2(minX, maxX);
            }
            else if (node is DecoratorNode decoratorNode)
            {
                TreeNode child = decoratorNode.GetChild();
                if (child != null)
                {
                    Vector2 res = OnSortTree(child, minX, maxX, y + 1);
                    if (maxX < res.y) maxX = res.y;
                    result = res;
                }
            }
            else if (node is ActionNode actionNode)
            {
                result = new Vector2(minX, maxX);
            }
            else if (node is RootNode rootNode)
            {
                TreeNode child = rootNode.GetChild();
                if (child != null)
                {
                    Vector2 res = OnSortTree(child, minX, maxX, y + 1);
                    if (maxX < res.y) maxX = res.y;
                }
            }

            posRect.x = (minX + maxX) / 2 * 220 - nodeView.contentRect.width / 2;
            posRect.y = y * 190;
            nodeView.SetPosition(posRect);

            return result;
        }
        #endregion

        #region [Getter / Setter]
        public BehaviourTreeAsset GetTree()
        {
            return tree;
        }

        public bool AutoSorting()
        {
            return autoSorting;
        }

        public void AutoSorting(bool value)
        {
            autoSorting = value;
        }
        #endregion
    }
}