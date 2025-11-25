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

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Interactive/Interactive Object Viewer")]
    [DisallowMultipleComponent]
    public sealed class InteractiveObjectViewer : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private Transform viewPoint;

        [SerializeField]
        [MinValue(0.01f)]
        private float range = 1.75f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        private LayerMask cullingLayer = 1 << 0;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        private QueryTriggerInteraction triggerInteraction;

        // Stored required properties.
        private InteractiveObject activeObject;
        private bool isViewing = true;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Debug.Assert(viewPoint != null, $"<b><color=#FF0000>Attach reference of the view point to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> View Point<i>(field)</i>.</color></b>");
        }

        private void LateUpdate()
        {
            if (isViewing)
            {
                if (viewPoint != null &&
                    Physics.Raycast(viewPoint.position, viewPoint.forward, out RaycastHit hitInfo, range, cullingLayer, triggerInteraction))
                {
                    InteractiveObject interactiveObject = hitInfo.transform.GetComponent<InteractiveObject>();
                    if (interactiveObject != null && activeObject != interactiveObject)
                    {
                        if(activeObject != null)
                        {
                            activeObject.Diactivate();
                        }
                        activeObject = interactiveObject;
                        activeObject.Activate(transform);
                    }
                }
                else if (activeObject != null)
                {
                    activeObject.Diactivate();
                    activeObject = null;
                }
            }
            else if (activeObject != null)
            {
                activeObject.Diactivate();
                activeObject = null;
            }
        }

        public void IsViewing(bool value)
        {
            isViewing = value;
        }
    }
}
