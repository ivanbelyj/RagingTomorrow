/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using UnityEngine;
using UnityEditor;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;

namespace AuroraFPSEditor
{
    /// <summary>
    /// Custom property drawers SerializableStackBase class.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableStackBase), true)]
    public class SerializableStackDrawer : PropertyDrawer
    {
        private const string ValuesFieldName = "values";

        /// <summary>
        /// Override this method to make your own IMGUI based GUI for the property.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty values = property.FindPropertyRelative(ValuesFieldName);
            EditorGUI.PropertyField(position, values, label, true);
        }

        /// <summary>
        /// Override this method to specify how tall the GUI for this field is in pixels.
        ///
        /// The default is one line high.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>The height in pixels.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            SerializedProperty values = property.FindPropertyRelative(ValuesFieldName);
            if (values.isExpanded)
            {
                height += 2;
                height *= 2 + values.arraySize;
            }
            return height;
        }
    }
}