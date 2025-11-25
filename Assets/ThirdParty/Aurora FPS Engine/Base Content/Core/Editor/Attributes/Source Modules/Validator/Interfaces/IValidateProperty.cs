/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor;

namespace AuroraFPSEditor.Attributes
{
    public interface IValidateProperty
    {
        /// <summary>
        /// Called every inspector update time before drawing property.
        /// </summary>
        /// <param name="property">Serialized property with ValidatorAttribute.</param>
        void Validate(SerializedProperty property);
    }
}