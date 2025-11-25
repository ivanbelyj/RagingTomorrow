/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    public class InputBindingField
    {
        private InputBinding binding;
        private Text inputField;

        #region [Constructor]
        public InputBindingField(InputBinding binding, Text inputField)
        {
            this.binding = binding;
            this.inputField = inputField;
        }
        #endregion

        #region [Getter / Setter]
        public InputBinding GetBinding()
        {
            return binding;
        }

        public void SetBinding(InputBinding value)
        {
            binding = value;
        }

        public Text GetInputField()
        {
            return inputField;
        }

        public void SetInputField(Text value)
        {
            inputField = value;
        }
        #endregion
    }
}
