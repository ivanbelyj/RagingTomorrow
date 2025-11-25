/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Other/Canvas Camera Initializer")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasCameraInitializer : MonoBehaviour
    {
        [SerializeField]
        [TagPopup]
        private string cameraTag;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [MinValue(0.01f)]
        private float searchRate = 0.5f;

        private IEnumerator Start()
        {
            Canvas canvas = GetComponent<Canvas>();

            WaitForSeconds delay = new WaitForSeconds(searchRate);

            while (canvas.worldCamera == null)
            {
                GameObject cameraObject = GameObject.FindGameObjectWithTag(cameraTag);
                if (cameraObject != null)
                {
                    Camera camera = cameraObject.GetComponent<Camera>();
                    if (camera != null)
                    {
                        canvas.worldCamera = camera;
                        yield break;
                    }
                }
                yield return delay;
            }
            
        }
    }
}