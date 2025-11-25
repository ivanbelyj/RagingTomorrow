/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Editable/Input Frame-Rate Validator")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InputField))]
    public class InputFrameRateValidator : MonoBehaviour
    {
        [SerializeField]
        [MinValue(1)]
        private int maxFrameRate = 60;

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
            if (inputField.text != string.Empty && inputField.text[0] != '0' && char.IsNumber(inputField.text[0]))
            {
                int value = int.Parse(inputField.text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                if (value > maxFrameRate)
                {
                    inputField.text = maxFrameRate.ToString();
                }
            }
            else
            {
                inputField.text = string.Empty;
            }
        }
        #endregion
    }
}