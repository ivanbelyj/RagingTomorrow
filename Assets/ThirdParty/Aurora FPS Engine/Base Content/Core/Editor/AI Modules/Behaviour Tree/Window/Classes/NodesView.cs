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
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public class NodesView : BehaviourTreeEditorView
    {
        public new class UxmlFactory : UxmlFactory<NodesView, VisualElement.UxmlTraits> { }

        private class Foldout
        {
            public Dictionary<string, Foldout> nested = new Dictionary<string, Foldout>();
            public string name;
            public Type value = null;

            private NodesView nodesView;
            private bool[] foldouts;

            public void Init(NodesView nodesView)
            {
                this.nodesView = nodesView;
                foldouts = new bool[nested.Count];

                foreach (var item in nested)
                {
                    item.Value.Init(nodesView);
                }
            }

            public void Add(string path, Type type)
            {
                string directory = path.Split('/', '\\')[0];

                if (!nested.ContainsKey(directory))
                {
                    nested.Add(directory, new Foldout());
                }

                int index = IndexOf(path, '/', '\\');
                if (index != -1)
                {
                    path = path.Remove(0, index + 1);
                    nested[directory].Add(path, type);
                }
                else
                {
                    nested[directory].name = directory;
                    nested[directory].value = type;
                }
            }

            public void Show(string search)
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    search = null;
                }

                int index = 0;

                foreach (var item in nested)
                {
                    if (search != null && !item.Value.ValidSearch(search))
                    {
                        continue;
                    }

                    if (item.Value.value == null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(3);
                            Rect boxPosition = EditorGUILayout.BeginVertical("box");
                            {
                                foldouts[index] = EditorGUILayout.Foldout(foldouts[index], item.Key, true);
                                if (foldouts[index])
                                {
                                    item.Value.Show(search);
                                }
                            }
                            EditorGUILayout.EndVertical();
                            Outline(boxPosition);
                            GUILayout.Space(3);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        if (search != null && item.Key.ToLower().IndexOf(search.ToLower()) == -1)
                        {
                            continue;
                        }

                        if (GUILayout.Button(item.Key))
                        {
                            GraphView graphView = nodesView.behaviourTreeEditor.GetTreeView();
                            Vector2 localPos =  new Vector2(graphView.viewport.contentRect.center.x, Event.current.mousePosition.y);
                            Vector2 pos = graphView.ChangeCoordinatesTo(graphView.contentViewContainer, localPos);
                            nodesView.behaviourTreeEditor.GetTreeView().CreateNode(item.Value.value, pos);
                        }
                    }

                    index++;
                }
            }

            public void Sort()
            {
                nested = nested.OrderBy(n => n.Key).ToDictionary(n => n.Key, n => n.Value);

                foreach (var n in nested)
                {
                    n.Value.Sort();
                }
            }

            private int IndexOf(string path, params char[] chars)
            {
                int index = -1;
                for (int i = 0; i < chars.Length; i++)
                {
                    index = path.IndexOf(chars[i]);
                    if (index != -1)
                    {
                        break;
                    }
                }

                return index;
            }

            private bool ValidSearch(string search)
            {
                if (value != null)
                {
                    return name.ToLower().IndexOf(search.ToLower()) != -1;
                }
                else
                {
                    return nested.Any(n => n.Value.ValidSearch(search));
                }
            }

            private void Outline(Rect rect)
            {
                Color color = new Color(32 / 255f, 32 / 255f, 32 / 255f);
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), color);
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), color);
                EditorGUI.DrawRect(new Rect(rect.x + rect.width, rect.y, 1, rect.height), color);
                EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, 1), color);
            }
        }

        private SearchField search;
        private Foldout foldout;
        private Vector2 scrollPosition;
        private string searchText;

        public NodesView()
        {
            search = new SearchField();
            foldout = new Foldout();
            var types = TypeCache.GetTypesDerivedFrom<TreeNode>();
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsGenericType)
                {
                    continue;
                }

                TreeNodeContentAttribute attribute = ApexReflection.GetAttribute<TreeNodeContentAttribute>(type);
                if (attribute != null && !attribute.Hide)
                {
                    string path = attribute?.Path ?? type.Name;
                    foldout.Add(path, type);
                }
            }

            foldout.Init(this);

            UpdateSelection(null);
        }

        public void UpdateSelection(BehaviourTreeAsset behaviourTree)
        {
            Clear();
            Add(new IMGUIContainer(() => OnGUI(behaviourTree)));
        }

        private void OnGUI(BehaviourTreeAsset behaviourTree)
        {
            if (behaviourTree == null)
            {
                EditorGUILayout.HelpBox("No behaviour tree selected. Create a new behavior tree or select one of the created ones.", MessageType.Info);
            }

            if (behaviourTree != null)
            {
                EditorGUILayout.Space(3);
                searchText = search.OnGUI(searchText);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.Space(3);
                foldout.Show(searchText?.Trim());
                GUILayout.EndScrollView();
            }
        }
    }
}