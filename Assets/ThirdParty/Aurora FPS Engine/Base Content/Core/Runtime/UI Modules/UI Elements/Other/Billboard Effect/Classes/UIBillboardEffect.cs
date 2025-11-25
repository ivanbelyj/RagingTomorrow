/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Other/UI Billboard Effect")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class UIBillboardEffect : MonoBehaviour
    {
        public enum Target
        {
            Automatically,
            Manual
        }

        [SerializeField]
        private Target target;

        [SerializeField]
        [TagPopup]
        [VisibleIf("target", "Automatically")]
        private string targetTag;

        [SerializeField]
        [VisibleIf("target", "Manual")]
        private Transform targetReference;

        // Stored required components.
        private new RectTransform transform;

        // Stored required properties.
        private CoroutineObject searchCoroutine;
        private Quaternion originalRotation;


        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            transform = GetComponent<RectTransform>();
            originalRotation = transform.rotation;
            searchCoroutine = new CoroutineObject(this);

            if(targetReference == null)
            {
                GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
                if(targetObject != null)
                {
                    targetReference = targetObject.transform;
                }
            }
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            if(targetReference != null)
            {
                transform.rotation = targetReference.rotation;
            }
            else if (!searchCoroutine.IsProcessing())
            {
                searchCoroutine.Start(SearchTarget, true);
            }
        }

        private IEnumerator SearchTarget()
        {
            WaitForSeconds delay = new WaitForSeconds(1.0f);
            GameObject targetObject = null;
            while (targetObject == null)
            {
                targetObject = GameObject.FindGameObjectWithTag(targetTag);
                yield return delay;
            }
            targetReference = targetObject.transform;
        }
    }
}