/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    public static class InputBindingsStore
    {
        private static readonly List<InputBindingField> InputBindingFields = new List<InputBindingField>();

        /// <summary>
        /// Add new pair of binding and input field to the store.
        /// </summary>
        /// <param name="binding">Input binding.</param>
        /// <param name="inputField">Input field.</param>
        public static void Add(InputBinding binding, Text inputField)
        {
            InputBindingFields.Add(new InputBindingField(binding, inputField));
        }

        /// <summary>
        /// Get input field assigned to a certain binding.
        /// </summary>
        /// <param name="binding">Input binding.</param>
        /// <returns>Input field assigned to the binding.</returns>
        public static Text GetInputField(InputBindingField binding) 
        {
            foreach(InputBindingField bindingField in InputBindingFields)
            {
                if(bindingField.Equals(binding))
                {
                    return bindingField.GetInputField();
                }
            }
            return null;
        }

        /// <summary>
        /// Get pair of binding and input field.
        /// </summary>
        /// <param name="binding">Input binding.</param>
        /// <returns>Pair of input binding and text field assigned to it.</returns>
        public static InputBindingField GetBindingFieldPair(InputBinding binding)
        {
            foreach (InputBindingField bindingField in InputBindingFields)
            {
                if (bindingField.GetBinding() == binding)
                {
                    return bindingField;
                }
            }
            return null;
        }

        /// <summary>
        /// Replace a stored binding with a new instance of it.
        /// </summary>
        /// <param name="inputField">Input field assigned to the binding.</param>
        /// <param name="newBinding">New instance of binding</param>
        public static void ReplaceBinding(Text inputField, InputBinding newBinding)
        {
            InputBindingField bindingField = InputBindingFields.Find(bf =>
            bf.GetInputField() == inputField);
            if (bindingField != null)
            {
                bindingField.SetBinding(newBinding);
            }
        }

        /// <summary>
        /// Replace a stored binding with a new instance of it.
        /// </summary>
        /// <param name="oldBinding">Binding to replace</param>
        /// <param name="newBinding">New instance of binding</param>
        public static void ReplaceBinding(InputBinding oldBinding, InputBinding newBinding)
        {
            InputBindingField bindingField = GetBindingFieldPair(oldBinding);
            if (bindingField != null)
            {
                bindingField.SetBinding(newBinding);
            }
        }

        public static List<InputBindingField> GetStore()
        {
            return InputBindingFields;
        }
    }
}
