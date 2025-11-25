/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    internal class SearchableMenuWindow : EditorWindow
    {
        internal static class Styles
        {
            public static GUIStyle MessageStyle
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 12;
                    style.fontStyle = FontStyle.Italic;
                    return style;
                }
            }
        }

        // Items properties.
        private List<SearchItem> items;
        private List<SearchItem> searchItems;

        // Search field properties.
        private SearchField searchField;
        private string searchText;
        private string previousSearchText;

        // Scroll view properties.
        private Vector2 scrollPosition;

        // Stored required properties.
        private float maxHeight;

        public static void Create(Rect buttonRect, Vector2 size, List<SearchItem> searchItems, string startSearchText, Action<string> onSearchFieldChangedCallback)
        {
            SearchableMenuWindow window = CreateInstance<SearchableMenuWindow>();
            window.searchField = new SearchField();
            window.searchField.SetFocus();

            for (int i = 0; i < searchItems.Count; i++)
            {
                searchItems[i].OnClickCallback += () => GUI.changed = true;
                searchItems[i].OnClickCallback += window.Close;
            }

            window.items = searchItems;
            window.searchItems = new List<SearchItem>(searchItems);
            window.searchText = startSearchText;
            window.onSearchFieldChangedCallback = onSearchFieldChangedCallback;

            buttonRect.position = GUIUtility.GUIToScreenPoint(buttonRect.position);

            window.maxHeight = size.y;
            window.ShowAsDropDown(buttonRect, size);
            window.FixHeight();
        }

        private void OnGUI()
        {
            DrawSearchField();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            DrawItems();
            GUILayout.EndScrollView();

            UpdateSearchResults();
            DrawWindowBorders();
        }

        private void DrawSearchField()
        {
            Rect toolbarPosition = GUILayoutUtility.GetRect(0, 22);
            toolbarPosition.x += 5;
            toolbarPosition.y += 3;
            toolbarPosition.width -= 5;
            searchText = searchField.OnToolbarGUI(toolbarPosition, searchText);
        }

        private void DrawItems()
        {
            if (searchItems.Count > 0)
            {
                for (int i = 0; i < searchItems.Count; i++)
                {
                    searchItems[i].OnItemGUI();

                }
            }
            else
            {
                GUILayout.Label("Not found...", Styles.MessageStyle);
            }
        }

        private void UpdateSearchResults()
        {
            if (searchText != previousSearchText)
            {
                if (searchText != null)
                {
                    searchItems.Clear();
                    for (int i = 0; i < items.Count; i++)
                    {
                        SearchItem item = items[i];

                        string itemCopy = item.GetLabel().text.ToLower();
                        string searchTextCopy = searchText.ToLower();
                        if (itemCopy.Contains(searchTextCopy))
                        {
                            searchItems.Add(item);
                        }
                    }
                }
                else
                {
                    searchItems = new List<SearchItem>(items);
                }
                onSearchFieldChangedCallback?.Invoke(searchText);
                FixHeight();
                previousSearchText = searchText;
            }
        }

        private void DrawWindowBorders()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, 1), Color.black);
            EditorGUI.DrawRect(new Rect(0, 0, 1, position.height), Color.black);
            EditorGUI.DrawRect(new Rect(position.width - 1, 0, 1, position.height), Color.black);
            EditorGUI.DrawRect(new Rect(0, position.height - 2, position.width, 1), Color.black);
        }

        private void FixHeight()
        {
            const float itemSize = 20f;
            float height = 23;
            height = Mathf.Clamp(height + (itemSize * searchItems.Count), 45, maxHeight);
            minSize = new Vector2(position.width, height);
            maxSize = new Vector2(position.width, height);
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Called when editing search field.
        /// </summary>
        public Action<string> onSearchFieldChangedCallback;
        #endregion
    }
}