/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.AIModules;
using AuroraFPSRuntime.AIModules.BehaviourTree;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public sealed class BehaviourTreeEditor : EditorWindow
    {
        public static readonly string TreeUxmlRelativePath = "Base Content/Core/Editor/AI Modules/Behaviour Tree/Styles/BehaviourTreeEditor.uxml";
        public static readonly string NodeUxmlRelativePath = "Base Content/Core/Editor/AI Modules/Behaviour Tree/Styles/NodeView.uxml";
        public static readonly string UssRelativePath = "Base Content/Core/Editor/AI Modules/Behaviour Tree/Styles/BehaviourTreeStyle.uss";

        public static readonly List<BehaviourTreeAsset> behaviourTrees = new List<BehaviourTreeAsset>();
        private BehaviourTreeAsset selectedTree;

        private TreeView treeView;
        private BehaviourView behaviourView;
        private NodesView nodesView;
        private VariablesView variablesView;
        private InspectorView inspectorView;

        private ToolbarMenu toolbarAssetMenu;
        private ToolbarMenu toolbarEditMenu;
        private Label treeNameLabel;
        private Label toolNameLabel;

        private string selectedTab = "Behaviour";

        [MenuItem("Aurora FPS Engine/AI/Behaviour Tree/Behaviour Tree Editor", false, 301)]
        public static void Open()
        {
            ApexSettings settings = ApexSettings.Current;
            string treeUxmlPath = Path.Combine(settings.GetRootPath(), TreeUxmlRelativePath);
            string nodeUxmlPath = Path.Combine(settings.GetRootPath(), NodeUxmlRelativePath);
            string ussPath = Path.Combine(settings.GetRootPath(), UssRelativePath);
            if (!File.Exists(treeUxmlPath) ||
               !File.Exists(nodeUxmlPath) ||
               !File.Exists(ussPath))
            {
                Debug.LogAssertion("<b><color=#FF0000>Missing UXML and USS files!</color></b>\n" +
                   $"<color=#FF0000><i>Tree UXML required: {treeUxmlPath}</i></color>\n" +
                   $"<color=#FF0000><i>Node UXML required: {nodeUxmlPath}</i></color>\n" +
                   $"<color=#FF0000><i>Uss required: {ussPath}</i></color>\n");
                return;
            }

            BehaviourTreeEditor window = GetWindow<BehaviourTreeEditor>();
            window.titleContent = new GUIContent("Behaviour Tree Editor");
            window.Show();
        }

        /// <summary>
        /// Allows you to open the behavior the editor by opening the behavior tree asset.
        /// </summary>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset is BehaviourTreeAsset)
            {
                Open();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Filling in the GUI.
        /// </summary>
        private void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            // Import UXML
            ApexSettings settings = ApexSettings.Current;
            string treeUxmlPath = Path.Combine(settings.GetRootPath(), TreeUxmlRelativePath);
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(treeUxmlPath);
            visualTree.CloneTree(root);


            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            string ussPath = Path.Combine(settings.GetRootPath(), UssRelativePath);
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            root.styleSheets.Add(styleSheet);

            // Toolbar menu
            toolbarAssetMenu = root.Q<ToolbarMenu>("toolbar-asset-menu");
            toolbarEditMenu = root.Q<ToolbarMenu>("toolbar-edit-menu");


            // Views initialization
            treeView = root.Q<TreeView>();

            behaviourView = root.Q<BehaviourView>();
            behaviourView.SetBehaviourTreeEditor(this);

            nodesView = root.Q<NodesView>();
            nodesView.SetBehaviourTreeEditor(this);

            variablesView = root.Q<VariablesView>();
            variablesView.SetBehaviourTreeEditor(this);

            inspectorView = root.Q<InspectorView>();
            inspectorView.SetBehaviourTreeEditor(this);


            // Viewbar buttons
            root.Q<ToolbarButton>("viewbar-behaviour-menu").clicked += () =>
            {
                selectedTab = "Behaviour";
                SelectMenu(selectedTab);
            };
            root.Q<ToolbarButton>("viewbar-nodes-menu").clicked += () =>
            {
                selectedTab = "Nodes";
                SelectMenu(selectedTab);
            };
            root.Q<ToolbarButton>("viewbar-variables-menu").clicked += () =>
            {
                selectedTab = "Variables";
                SelectMenu(selectedTab);
            };
            root.Q<ToolbarButton>("viewbar-inspector-menu").clicked += () =>
            {
                selectedTab = "Inspector";
                SelectMenu(selectedTab);
            };


            // Events
            treeView.OnNodeSelected = OnNodeSelectionChanged;

            // Labels initialization
            treeNameLabel = root.Q<Label>("tree-name");
            toolNameLabel = root.Q<Label>("tool-name");

            if (selectedTree != null)
            {
                SelectTree(selectedTree);
            }
            else
            {
                OnSelectionChange();
            }

        }

        /// <summary>
        /// This function is called when open the EditorWindow window.
        /// </summary>
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            behaviourTrees.Clear();
            BehaviourTreeAsset[] behaviourTreeArray = BehaviourTreeUtilities.FindObjectsOfType<BehaviourTreeAsset>();
            for (int i = 0; i < behaviourTreeArray.Length; i++)
            {
                behaviourTrees.Add(behaviourTreeArray[i]);
            }
        }

        /// <summary>
        /// This function is called when close the EditorWindow window.
        /// </summary>
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            treeView?.OnDisable();
        }

        /// <summary>
        /// Called when the playback mode state changes.
        /// </summary>
        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    UnselectTree();
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        /// <summary>
        /// Called when the window gets keyboard focus.
        /// </summary>
        private void OnFocus()
        {
            if (toolbarAssetMenu == null)
            {
                return;
            }

            List<int> removeIndexes = new List<int>();
            for (int i = 0; i < behaviourTrees.Count; i++)
            {
                if (behaviourTrees[i] == null)
                {
                    removeIndexes.Add(i);
                }
            }

            if (removeIndexes.Count > 0)
            {
                for (int i = 0; i < removeIndexes.Count; i++)
                {
                    behaviourTrees.RemoveAt(removeIndexes[i]);
                }
            }

            // Toolbar assets menu
            toolbarAssetMenu.menu.MenuItems().Clear();
            for (int i = 0; i < behaviourTrees.Count; i++)
            {
                BehaviourTreeAsset tree = behaviourTrees[i];
                toolbarAssetMenu.menu.AppendAction($"{tree.GetName()}", (a) =>
                {
                    Selection.activeObject = tree;
                });
            }
            toolbarAssetMenu.menu.AppendSeparator();
            toolbarAssetMenu.menu.AppendAction("New Tree...", (a) => CreateBehaviourTree("New Behaviour Tree"));

            // Toolbar edit menu
            toolbarEditMenu.menu.MenuItems().Clear();
            toolbarEditMenu.menu.AppendAction("Auto Tree Sort", (action) =>
            {
                treeView.AutoSorting(action.status == DropdownMenuAction.Status.Normal ? true : false);
            }, AutoTreeSort);
            toolbarEditMenu.menu.AppendAction("Tree Sorting/Large sort", (a) => treeView?.SortTree());
            toolbarEditMenu.menu.AppendAction("Frame All [F]", (a) =>
            {
                treeView.FrameAll();
            });
        }

        /// <summary>
        /// Called every time the project is changed.
        /// </summary>
        private void OnProjectChange()
        {
            if (treeView?.GetTree() == null)
            {
                UnselectTree();
            }
        }

        /// <summary>
        /// Called whenever the selection has changed.
        /// </summary>
        private void OnSelectionChange()
        {
            BehaviourTreeAsset tree = Selection.activeObject as BehaviourTreeAsset;
            if (tree == null)
            {
                if (Selection.activeGameObject != null)
                {
                    BehaviourTreeRunner aiController = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                    if (aiController != null)
                    {
                        tree = aiController.GetBehaviourTree();
                        if (tree == null)
                        {
                            tree = aiController.GetSharedBehaviourTree();
                        }
                    }
                }
            }

            if (selectedTree != tree)
            {
                SelectTree(tree);
            }
        }

        /// <summary>
        /// Selecting a tree to display.
        /// </summary>
        private void SelectTree(BehaviourTreeAsset tree)
        {
            if (treeView == null || tree == null || (!Application.isPlaying && !AssetDatabase.IsNativeAsset(tree))) return;

            selectedTree = tree;

            treeView.PopulateView(selectedTree);

            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    treeView.FrameAll();
                };
            };

            treeNameLabel.text = tree.GetName().Split(new char[] { '\\', '/' }).LastOrDefault();
            behaviourView.UpdateBehaviour(tree);
            nodesView.UpdateSelection(tree);
            variablesView.UpdateSelection(tree);
        }

        /// <summary>
        /// Unselecting a tree.
        /// </summary>
        private void UnselectTree()
        {
            treeView?.ClearTree();
            selectedTree = null;
            if (treeNameLabel != null)
            {
                treeNameLabel.text = "Select tree...";
            }
        }

        private void OnNodeSelectionChanged(TreeNodeView nodeView)
        {
            inspectorView.UpdateSelection(nodeView);

            if (nodeView != null)
            {
                SelectMenu("Inspector");
            }
            else
            {
                SelectMenu(selectedTab);
            }
        }

        /// <summary>
        /// Called at 10 frames per second to give the inspector a chance to update.
        /// </summary>
        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {
                treeView?.UpdateNodeStates();
            }
        }

        /// <summary>
        /// Creates a new behavior tree.
        /// </summary>
        public void CreateBehaviourTree(string assetName)
        {
            string path = Path.Combine(ApexSettings.Current.GetRootPath(), $"{assetName}.asset");
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            BehaviourTreeAsset newTree = CreateInstance<BehaviourTreeAsset>();
            newTree.name = assetName;
            newTree.SetName("Behaviour Tree");
            AssetDatabase.CreateAsset(newTree, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = newTree;
            EditorGUIUtility.PingObject(newTree);
            behaviourTrees.Add(newTree);
        }

        private void SelectMenu(string menuName)
        {
            toolNameLabel.text = menuName;
            switch (menuName)
            {
                case "Behaviour":
                    behaviourView.AddToClassList("active-tool");
                    nodesView.RemoveFromClassList("active-tool");
                    variablesView.RemoveFromClassList("active-tool");
                    inspectorView.RemoveFromClassList("active-tool");
                    break;
                case "Nodes":
                    behaviourView.RemoveFromClassList("active-tool");
                    nodesView.AddToClassList("active-tool");
                    variablesView.RemoveFromClassList("active-tool");
                    inspectorView.RemoveFromClassList("active-tool");
                    break;
                case "Variables":
                    behaviourView.RemoveFromClassList("active-tool");
                    nodesView.RemoveFromClassList("active-tool");
                    variablesView.AddToClassList("active-tool");
                    inspectorView.RemoveFromClassList("active-tool");
                    break;
                case "Inspector":
                    behaviourView.RemoveFromClassList("active-tool");
                    nodesView.RemoveFromClassList("active-tool");
                    variablesView.RemoveFromClassList("active-tool");
                    inspectorView.AddToClassList("active-tool");
                    break;
            }
        }

        #region [Auto Tree Sorting]
        private DropdownMenuAction.Status AutoTreeSort(DropdownMenuAction action)
        {
            return treeView.AutoSorting() ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;
        }
        #endregion

        #region [Getter / Setter]
        public BehaviourTreeAsset GetBehaviourTree()
        {
            return selectedTree;
        }

        public TreeView GetTreeView()
        {
            return treeView;
        }
        #endregion
    }
}