/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.Animation
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Color/Blend Color")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class BlendColor : MonoBehaviour
    {
        [SerializeField]
        private float minNumber = 0;

        [SerializeField]
        private Color minNumberColor = Color.red;

        [SerializeField]
        private float maxNumber = 1;

        [SerializeField]
        private Color maxNumberColor = Color.white;

        // Stored required components.
        private Text text;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            text = GetComponent<Text>();
        }

        /// <summary>
        /// Called every frame, while the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            float number = System.Convert.ToSingle(text.text);
            float range = Mathf.InverseLerp(minNumber, maxNumber, number);
            text.color = Color.Lerp(minNumberColor, maxNumberColor, range);
        }
    }
}
