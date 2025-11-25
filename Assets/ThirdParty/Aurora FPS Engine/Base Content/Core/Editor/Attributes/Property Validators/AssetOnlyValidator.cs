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
    /// The attribute checks that the asset you want to add to the field is an asset from the project window.
    /// </summary>
    [ValidatorTarget(typeof(AssetOnlyAttribute))]
    public sealed class AssetOnlyValidator : PropertyValidator
    {
        private bool isActive;

        /// <summary>
        /// Called before drawing property.
        /// </summary>
        /// <param name="property">Serialized property with ValidatorAttribute.</param>
        public override void Validate(SerializedProperty property)
        {
            isActive = true;

            Object[] objects = DragAndDrop.objectReferences;
            if (objects != null && objects.Length > 0)
            {
                GameObject go = objects[0] as GameObject;
                if (go != null)
                {
                    isActive = !(PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.NotAPrefab);
                }
            }

            if (Event.current.type == EventType.DragExited)
            {
                isActive = true;
            }
        }

        /// <summary>
        /// Called before drawing property.
        /// </summary>
        public override void BeforePropertyGUI()
        {
            EditorGUI.BeginDisabledGroup(!isActive);
        }

        /// <summary>
        /// Called after drawing property.
        /// </summary>
        public override void AfterPropertyGUI()
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}