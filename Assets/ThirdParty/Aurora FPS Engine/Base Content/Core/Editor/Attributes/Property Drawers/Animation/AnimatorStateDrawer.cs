 /* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using AuroraFPSRuntime.CoreModules.ValueTypes;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [DrawerTarget(typeof(AnimatorState))]
    internal sealed class AnimatorStateDrawer : PropertyDrawer
    {
        private SerializedProperty name;
        private SerializedProperty layer;
        private SerializedProperty fixedTime;
        private SerializedProperty nameHash;

        /// <summary>
        /// Called once when initializing PropertyDrawer.
        /// </summary>
        /// <param name="property">Serialized property with DrawerAttribute.</param>
        /// <param name="drawerAttribute">DrawerAttribute of serialized property.</param>
        /// <param name="label">Label of serialized property.</param>
        public override void OnInitialize(SerializedProperty property, GUIContent label)
        {
            name = property.FindPropertyRelative("name");
            layer = property.FindPropertyRelative("layer");
            fixedTime = property.FindPropertyRelative("fixedTime");
            nameHash = property.FindPropertyRelative("nameHash");
        }

        /// <summary>
        /// Override this method to make your own IMGUI based GUI for the property.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string storedName = name.stringValue;
            Rect namePosition = new Rect(position.x, position.y, position.width - 50, position.height);
            name.stringValue = EditorGUI.DelayedTextField(namePosition, label, name.stringValue);
            if (storedName != name.stringValue)
            {
                nameHash.intValue = Animator.StringToHash(name.stringValue);
            }

            int storedIndexLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            Rect layerPosition = new Rect(namePosition.xMax + 1, namePosition.y, 14, namePosition.height);
            layer.intValue = EditorGUI.IntField(layerPosition, layer.intValue);

            Rect fixedTimePosition = new Rect(layerPosition.xMax + 1, layerPosition.y, 33, layerPosition.height);
            fixedTime.floatValue = EditorGUI.FloatField(fixedTimePosition, fixedTime.floatValue);
            EditorGUI.indentLevel = storedIndexLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}