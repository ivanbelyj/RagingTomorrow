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
    public interface IValidatorInitialization
    {
        /// <summary>
        /// Called once when initializing PropertyValidator.
        /// </summary>
        /// <param name="property">Serialized property with ValidatorAttribute.</param>
        /// <param name="attribute">ValidatorAttribute of serialized property.</param>
        /// <param name="label">Label of serialized property.</param>
        void OnInitialize(SerializedProperty property, ValidatorAttribute attribute, GUIContent label);
    }
}