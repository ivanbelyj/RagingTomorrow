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
using UnityEngine.EventSystems;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Fitter/Preferred Size Fitter")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class PreferredSizeFitter : UIBehaviour
    {
        [SerializeField]
        [MinValue(0)]
        private float offset;

        [SerializeField]
        [ReorderableList(ElementLabel = null)]
        private RectTransform[] contents;


        // Stored required components.
        private new RectTransform transform;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            transform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            float width = offset;
            for (int i = 0; i < contents.Length; i++)
            {
                width += contents[i].offsetMax.x;
            }
            transform.sizeDelta = new Vector2(width, transform.sizeDelta.y);
        }
    }
}