/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Editable/Input Number Validator")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InputField))]
    public class InputNumberValidator : MonoBehaviour
    {
        [SerializeField]
        private float minValue = 0;

        [SerializeField]
        private float maxValue = 1;

        [SerializeField]
        [MinValue(0)]
        private int decimalPoint;

        // Stored required components.
        private InputField inputField;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            inputField = GetComponent<InputField>();
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            inputField.onEndEdit.AddListener(Validate);
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            inputField.onEndEdit.RemoveListener(Validate);
        }

        #region [Event Action Wrappers]
        private void Validate(string text)
        {
            if (text != string.Empty)
            {
                float value = float.Parse(text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                if(!Math.InRange(value, minValue, maxValue))
                {
                    value = Mathf.Clamp(value, minValue, maxValue);
                    inputField.text = value.ToString($"F{decimalPoint}", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                }
            }
            else
            {
                inputField.text = minValue.ToString($"F{decimalPoint}", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            }
        }
        #endregion
    }
}