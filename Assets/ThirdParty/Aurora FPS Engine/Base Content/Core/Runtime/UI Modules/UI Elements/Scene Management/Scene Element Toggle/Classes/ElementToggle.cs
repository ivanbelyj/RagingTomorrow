/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Scene Management/Element Toggle")]
    [DisallowMultipleComponent]
    public sealed class ElementToggle : MonoBehaviour
    {
        [SerializeField]
        [ReorderableList(ElementLabel = null)]
        private string[] vaildScenes;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Scene active = SceneManager.GetActiveScene();
            bool isValid = vaildScenes.Any(s => s == active.name);
            gameObject.SetActive(isValid);
        }
    }
}