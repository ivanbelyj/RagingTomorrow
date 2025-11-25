/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Scene Management/Slider Loading Progress")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public sealed class SliderLoadingProgress : MonoBehaviour
    {
        [SerializeReference]
        [NotNull]
        private TargetSceneLoader loader;

        // Stored required components.
        private Slider slider;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            slider.value = loader.GetLoadingProgress();
        }
    }
}
