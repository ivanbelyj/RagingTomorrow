/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Sight/Sniper Sight")]
    [DisallowMultipleComponent]
    public sealed class SniperSight : MonoBehaviour
    {
        [System.Serializable]
        private struct RenderComponent
        {
            public MeshRenderer renderer;
            public string colorProperty;
        }

        [SerializeField]
        [ReorderableList]
        private RenderComponent[] sightComponents;

        [SerializeField]
        [ReorderableList]
        private RenderComponent[] plugComponents;

        // Stored required properties.
        private PlayerController controller;

        private void Awake()
        {
            controller = GetComponentInParent<PlayerController>();
            if(controller != null)
            {
                controller.GetPlayerCamera().OnFOVProgressCallback += OnFOVChanged;
            }
        }

        private void OnDestroy()
        {
            if (controller != null)
            {
                controller.GetPlayerCamera().OnFOVProgressCallback -= OnFOVChanged;
            }
        }

        private void OnFOVChanged(float progress)
        {
            PlayerCamera cameraControl = controller.GetPlayerCamera();
            for (int i = 0; i < sightComponents.Length; i++)
            {
                RenderComponent renderComponent = sightComponents[i];
                MeshRenderer meshRenderer = renderComponent.renderer;
                if(meshRenderer != null)
                {
                    Material material = meshRenderer.material;
                    float target = cameraControl.IsZooming() ? 1.0f : 0.0f;
                    if (!string.IsNullOrEmpty(renderComponent.colorProperty))
                    {
                        string property = renderComponent.colorProperty;
                        Color targetColor = material.GetColor(property);
                        targetColor.a = target;
                        material.SetColor(property, Color.Lerp(material.color, targetColor, progress));
                    }
                    else
                    {
                        Color targetColor = material.color;
                        targetColor.a = target;
                        material.color = Color.Lerp(material.color, targetColor, progress);
                    }
                }
            }

            for (int i = 0; i < plugComponents.Length; i++)
            {
                RenderComponent renderComponent = plugComponents[i];
                MeshRenderer meshRenderer = renderComponent.renderer;
                if (meshRenderer != null)
                {
                    Material material = meshRenderer.material;
                    float target = cameraControl.IsZooming() ? 0.0f : 1.0f;
                    if (!string.IsNullOrEmpty(renderComponent.colorProperty))
                    {
                        string property = renderComponent.colorProperty;
                        Color targetColor = material.GetColor(property);
                        targetColor.a = target;
                        material.SetColor(property, Color.Lerp(material.color, targetColor, progress));
                    }
                    else
                    {
                        Color targetColor = material.color;
                        targetColor.a = target;
                        material.color = Color.Lerp(material.color, targetColor, progress);
                    }
                }
            }
        }
    }
}