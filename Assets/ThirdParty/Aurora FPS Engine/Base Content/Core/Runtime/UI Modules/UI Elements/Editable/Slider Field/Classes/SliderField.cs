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
using System.Globalization;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Editable/Slider Field")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InputField))]
    public sealed class SliderField : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private Slider slider;

        [SerializeField]
        [Slider(0, 3)]
        private int decimalPoint = 2;

        [SerializeField]
        private bool editNormalized = false;

        [SerializeField]
        [VisibleIf("editNormalized")]
        [Indent(1)]
        private bool asPercent = false;

        // Stored required components.
        private InputField inputField;

        // Stored required properties.
        private float lastSliderValue;
        private CultureInfo cultureInfo;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            inputField = GetComponent<InputField>();
            inputField.contentType = slider.wholeNumbers ? InputField.ContentType.IntegerNumber : InputField.ContentType.DecimalNumber;
            cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            inputField.onEndEdit.AddListener(OnInputFieldChanged);
        }

        /// <summary>
        /// Called every frame, if the Behaviour is enabled.
        /// </summary>
        private void LateUpdate()
        {
            if (lastSliderValue != slider.value)
            {
                if(decimalPoint > 0)
                    slider.value = Math.AllocatePart(slider.value, (float)System.Math.Pow(10, decimalPoint));
                FormatField();
                lastSliderValue = slider.value;
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            inputField.onEndEdit.RemoveListener(OnInputFieldChanged);
        }

        /// <summary>
        /// Format text of input field.
        /// </summary>
        private void FormatField()
        {
            float inputValue = slider.value;

            if (editNormalized)
            {
                inputValue = slider.normalizedValue;
                if (asPercent)
                {
                    inputValue *= 100;
                }
            }

            inputField.text = inputValue.ToString($"F{decimalPoint}", cultureInfo);
        }

        #region [Event Action Wrappers]
        private void OnInputFieldChanged(string text)
        {
            if (text != string.Empty)
            {
                float value = float.Parse(text, cultureInfo.NumberFormat);

                if (decimalPoint > 0)
                    value = Math.AllocatePart(value, (float)System.Math.Pow(10, decimalPoint));

                float minValue = slider.minValue;
                float maxValue = slider.maxValue;
                if (editNormalized)
                {
                    minValue = 0;
                    maxValue = asPercent ? 100 : 1;
                }

                value = Mathf.Clamp(value, minValue, maxValue);

                if (editNormalized)
                    slider.normalizedValue = Mathf.Lerp(0, 1, Mathf.InverseLerp(minValue, maxValue, value));
                else
                    slider.value = value;
            }
            else
            {
                slider.value = slider.minValue;
            }
            FormatField();
        }
        #endregion
    }
}