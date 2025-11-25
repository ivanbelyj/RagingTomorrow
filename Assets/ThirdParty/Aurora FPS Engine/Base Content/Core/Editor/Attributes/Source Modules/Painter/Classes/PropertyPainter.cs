/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    /// <summary>
    /// Additional visual elements on property.
    /// </summary>
    public abstract class PropertyPainter : IPainterInitialization, IPainterGUI,  IPainterHeight
    {
        // Stored property info.
        private Rect propertyPosition;

        /// <summary>
        /// Called once, when initializing PropertyPainter.
        /// </summary>
        /// <param name="property">Serialized property reference with current painter attribute.</param>
        /// <param name="painterAttribute">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public virtual void OnInitialize(SerializedProperty property, PainterAttribute painterAttribute, GUIContent label)
        {

        }

        /// <summary>
        /// Called before drawing property.
        /// </summary>
        public virtual void BeforePropertyGUI()
        {

        }

        /// <summary>
        /// Called for drawing additional visual elements on property.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the painter GUI.</param>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public virtual void OnPainterGUI(Rect position, SerializedProperty property, GUIContent label)
        {

        }

        /// <summary>
        /// Called after drawing property.
        /// </summary>
        public virtual void AfterPropertyGUI()
        {

        }

        /// <summary>
        /// Get the height of the painter, which required to display it.
        /// Calculate only the size of the current painter, not the entire property.
        /// The painter height will be added to the total size of the property with other painters.
        /// </summary>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public virtual float GetPainterHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        #region [Getter / Setter]
        /// <summary>
        /// Original property position.
        /// </summary>
        public Rect GetPropertyPosition()
        {
            return propertyPosition;
        }

        public void SetPropertyPosition(Rect value)
        {
            propertyPosition = value;
        }
        #endregion
    }
}
