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
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public sealed class SearchableMenu
    {
        private List<SearchItem> items;
        private string startSearchText;

        public SearchableMenu()
        {
            items = new List<SearchItem>();
        }

        public void AddItem(GUIContent label, bool isActive, OnItemClickCallback clickCallback)
        {
            items.Add(new SearchItem(label, isActive, clickCallback));
        }

        public void StartSearchText(string text)
        {
            startSearchText = text;
        }

        public void SortItems()
        {
            items?.Sort((s1, s2) => s1.GetLabel().text.CompareTo(s2.GetLabel().text));
        }

        public void ShowAsDropdown(Rect buttonRect, Vector2 size)
        {
            SearchableMenuWindow.Create(buttonRect, size, items, startSearchText, OnSearchFieldChanged);
        }

        #region [Event Callback Functions]
        public event Action<string> OnSearchFieldChanged;
        #endregion
    }
}