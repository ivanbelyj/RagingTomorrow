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
    /// Check properties for the valid of the specified conditions.
    /// </summary>
    public abstract class PropertyValidator : IValidatorInitialization, IValidateProperty
    {
        /// <summary>
        /// Called once when initializing PropertyValidator.
        /// </summary>
        /// <param name="property">Serialized property with ValidatorAttribute.</param>
        /// <param name="validatorAttribute">ValidatorAttribute of serialized property.</param>
        /// <param name="label">Label of serialized property.</param>
        public virtual void OnInitialize(SerializedProperty property, ValidatorAttribute validatorAttribute, GUIContent label)
        {
        
        }

        /// <summary>
        /// Called before drawing property.
        /// </summary>
        /// <param name="property">Serialized property with ValidatorAttribute.</param>
        public abstract void Validate(SerializedProperty property);

        /// <summary>
        /// Called before drawing property.
        /// </summary>
        public virtual void BeforePropertyGUI()
        {

        }

        /// <summary>
        /// Called for drawing validation GUI elements.
        /// </summary>
        /// <param name="originalPosition">Stored original position of the property.</param>
        /// <param name="validatorPosition">Rectangle on the screen to use for the validrator GUI.</param>
        /// <param name="property">Reference of serialized property validator attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public virtual void OnValidatorGUI(Rect originalPosition, Rect validatorPosition, SerializedProperty property, GUIContent label)
        {

        }

        /// <summary>
        /// Called after drawing property.
        /// </summary>
        public virtual void AfterPropertyGUI()
        {

        }

        /// <summary>
        /// Get the height of the validator, which required to display it.
        /// Calculate only the size of the current validator, not the entire property.
        /// The validator height will be added to the total size of the property.
        /// </summary>
        /// <param name="property">Reference of serialized property validator attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public virtual float GetValidatorHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}