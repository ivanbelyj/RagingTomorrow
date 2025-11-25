/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.Attributes
{
    public sealed class ReorderableListAttribute : ViewAttribute
    {
        public ReorderableListAttribute()
        {
            ElementLabel = "Element {index}";
            Draggable = true;
            DisplayHeader = true;
            DrawClearButton = false;
            RightFoldoutToggle = false;
        }

        #region [Optional Parameters]
        /// <summary>
        /// Custom element name display format. Arguments: {index}, {niceIndex}
        /// </summary>
        public string ElementLabel { get; set; }

        /// <summary>
        /// Custom label text which displayed, when list is empty.
        /// </summary>
        public string NoneElementLabel { get; set; }

        /// <summary>
        /// Set false to disable element drag function 
        /// </summary>
        public bool Draggable { get; set; }

        /// <summary>
        /// Set true to display button to clear all list elements.
        /// </summary>
        public bool DrawClearButton { get; set; }

        /// <summary>
        /// Set false to hide header.
        /// </summary>
        public bool DisplayHeader { get; set; }

        /// <summary>
        /// Set true to display separator between elements.
        /// </summary>
        public bool DisplaySeparator { get; set; }

        /// <summary>
        /// Custom GUI when list is empty.
        /// </summary>
        public string OnNoneElementGUICallback { get; set; }

        /// <summary>
        /// Custom element GUI callback.
        /// </summary>
        public string OnElementGUICallback { get; set; }

        /// <summary>
        /// Custom element label callback.
        /// 
        /// MethordName(SerializeProperty property, int index);
        /// </summary>
        public string GetElementLabelCallback { get; set; }

        /// <summary>
        /// Custom element height callback.
        /// 
        /// MethordName(SerializeProperty property, GUIContent label);
        /// </summary>
        public string GetElementHeightCallback { get; set; }

        /// <summary>
        /// Called once when pressed add button.
        /// 
        /// MethordName(SerializeProperty property);
        /// </summary>
        public string OnAddCallbackCallback { get; set; }

        /// <summary>
        /// Called once when pressed remove button.
        /// 
        /// MethordName(SerializeProperty property);
        /// </summary>
        public string OnRemoveCallbackCallback { get; set; }

        /// <summary>
        /// Called once when pressed add dropdown button.
        /// 
        /// MethordName(Rect position, SerializeProperty property);
        /// </summary>
        public string OnDropdownButtonCallback { get; set; }

        /// <summary>
        /// Called header GUI callback.
        /// </summary>
        public string OnHeaderGUICallback { get; set; }

        /// <summary>
        /// Place foldout toggle on right side.
        /// </summary>
        public bool RightFoldoutToggle { get; set; }
        #endregion
    }
}