/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Managers/Canvas Manager")]
    [DisallowMultipleComponent]
    public sealed class CanvasManager : MonoBehaviour
    {
        [SerializeField]
        [Array(GetElementLabelCallback = "GetCanvasLabel")]
        private List<CanvasProperty> properties;

        // Stored required properties.
        private CanvasProperty activeProperty;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            for (int i = 0; i < properties.Count; i++)
            {
                CanvasProperty property = properties[i];
                property.Inititalize();
            }
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            for (int i = 0; i < properties.Count; i++)
            {
                CanvasProperty property = properties[i];
                if(property.GetInputAction() != null)
                {
                    property.GetInputAction().performed += OnPerformedAction;
                }
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            for (int i = 0; i < properties.Count; i++)
            {
                properties[i].UnlockInputMaps();
            }
        }

        /// <summary>
        /// Called once before the behaviour will destroyed.
        /// </summary>
        private void OnDestroy()
        {
            for (int i = 0; i < properties.Count; i++)
            {
                CanvasProperty property = properties[i];
                property.DisableInteractions();
                if (property.GetInputAction() != null)
                {
                    property.GetInputAction().performed -= OnPerformedAction;
                }
            }
        }

        /// <summary>
        /// Add new canvas property.
        /// </summary>
        /// <param name="property">Canvas property reference.</param>
        public void AddProperty(CanvasProperty property)
        {
            property.Inititalize();
            if (property.GetInputAction() != null)
            {
                property.GetInputAction().performed += OnPerformedAction;
            }
            properties.Add(property);
        }

        /// <summary>
        /// Remove canvas property.
        /// </summary>
        /// <param name="property">Canvas property reference.</param>
        public void RemoveProperty(CanvasProperty property)
        {
            property.DisableInteractions();
            if (property.GetInputAction() != null)
            {
                property.GetInputAction().performed -= OnPerformedAction;
            }
            properties.Remove(property);
        }

        /// <summary>
        /// Remove canvas property.
        /// </summary>
        /// <param name="property">Index of canvas property.</param>
        public void RemoveProperty(int index)
        {
            CanvasProperty property = properties[index];
            property.DisableInteractions();
            if (property.GetInputAction() != null)
            {
                property.GetInputAction().performed -= OnPerformedAction;
            }
            properties.RemoveAt(index);
        }

        /// <summary>
        /// Clear canvas properties.
        /// </summary>
        public void ClearProperties()
        {
            for (int i = 0; i < properties.Count; i++)
            {
                CanvasProperty property = properties[i];
                property.DisableInteractions();
                if (property.GetInputAction() != null)
                {
                    property.GetInputAction().performed -= OnPerformedAction;
                }
            }
            properties.Clear();
        }

        /// <summary>
        /// Search canvas switcher property by specified canvas object.
        /// </summary>
        /// <param name="canvas">Canvas object key.</param>
        /// <param name="canvasProperty">Reference of canvas property.</param>
        public bool TryGetCanvasProperty(Canvas canvas, out CanvasProperty canvasProperty)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                canvasProperty = properties[i];
                if(canvas == canvasProperty.GetCanvas())
                {
                    return true;
                }
            }
            canvasProperty = null;
            return false;
        }

        #region [Input Action Wrapper]
        private void OnPerformedAction(InputAction.CallbackContext context)
        {
            if (!activeProperty?.IsActive() ?? false)
            {
                activeProperty = null;
            }

            for (int i = 0; i < properties.Count; i++)
            {
                CanvasProperty property = properties[i];
                if (property != null && property.GetInputAction().id == context.action.id)
                {
                    if (activeProperty != null)
                    {
                        if (activeProperty == property)
                        {
                            property.Toggle();
                        }
                        else if (property.GetOrder() > activeProperty.GetOrder() || property.GetOrder() == activeProperty.GetOrder())
                        {
                            activeProperty.Toggle(false);
                            property.Toggle(true);
                            activeProperty = property;
                        }
                    }
                    else
                    {
                        property.Toggle(true);
                        activeProperty = property;
                    }
                }
            }
        }
        #endregion

        #region [Editor Section]
#if UNITY_EDITOR
        private string GetCanvasLabel(UnityEditor.SerializedProperty property, int index)
        {
            return property.GetArrayElementAtIndex(index).FindPropertyRelative("canvas").objectReferenceValue?.name ?? string.Format("Canvas {0}", index + 1);
        }
#endif
        #endregion

        #region [Getter / Setter]
        public List<CanvasProperty> GetProperties()
        {
            return properties;
        }

        public CanvasProperty GetProperty(int index)
        {
            return properties[index];
        }

        public void SetProperties(List<CanvasProperty> value)
        {
            properties = value;
        }

        public void SetProperty(int index, CanvasProperty value)
        {
            properties[index] = value;
        }

        public int GetPropertyCount()
        {
            return properties.Count;
        }

        public CanvasProperty GetActiveProperty()
        {
            return activeProperty;
        }
        #endregion
    }
}
