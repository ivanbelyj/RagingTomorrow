/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.UI;

#region [Editor Section]
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

namespace AuroraFPSRuntime.UIModules.UIElements.Animation
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Transition/UI Shader Transition Handler")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    public sealed class UIShaderTransitionHandler : MonoBehaviour
    {
        [SerializeField]
        [NotEmpty]
        [CustomView(ViewInitialization = "OnPropertyInitialization", ViewGUI = "OnPropertyGUI")]
        private string property;

        [SerializeField]
        private float value;

        [SerializeField]
        private bool sharedMaterial = false;

        // Stored required components.
        private Material material;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Graphic graphic = GetComponent<Graphic>();
            if (sharedMaterial)
            {
                material = graphic.material;
            }
            else
            {
                material = Instantiate(graphic.material);
                graphic.material = material;
            }
        }

        /// <summary>
        /// Called on the frame when a script is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            if (material.HasProperty(property))
            {
                Transition transition = GetComponentInParent<Transition>();
                if (transition != null)
                {
                    transition.OnFadeInCallback += OnFadeIn;
                    transition.OnFadeOutCallback += OnFadeOut;
                }
            }
        }

        /// <summary>
        /// Called when performing fade in transition.
        /// </summary>
        /// <param name="smooth">Interpolation value evaluated by curve.</param>
        private void OnFadeIn(float smooth)
        {
            material.SetFloat(property, Mathf.Lerp(material.GetFloat(property), value, smooth));
        }

        /// <summary>
        /// Called when performing fade out transition.
        /// </summary>
        /// <param name="smooth">Interpolation value evaluated by curve.</param>
        private void OnFadeOut(float smooth)
        {
            material.SetFloat(property, Mathf.Lerp(material.GetFloat(property), 0, smooth));
        }

        #region [Editor Section]
#if UNITY_EDITOR
        private Graphic graphic;
        private Material localMaterial;
        private GenericMenu properties;
        private string propertyDisplayName;

        private void OnPropertyInitialization(SerializedProperty property, GUIContent label)
        {
            properties = new GenericMenu();
            propertyDisplayName = string.Empty;

            graphic = GetComponent<Graphic>();
            localMaterial = graphic.material;

            MaterialProperty[] materialProperties = MaterialEditor.GetMaterialProperties(new Material[1] { localMaterial });
            for (int i = 0; i < materialProperties.Length; i++)
            {
                MaterialProperty materialProperty = materialProperties[i];
                if (materialProperty.type == MaterialProperty.PropType.Float || materialProperty.type == MaterialProperty.PropType.Range)
                {
                    properties.AddItem(new GUIContent(materialProperty.displayName), false, () =>
                    {
                        this.property = materialProperty.name;
                        propertyDisplayName = materialProperty.displayName;
                    });

                    if(materialProperty.name == this.property)
                    {
                        propertyDisplayName = materialProperty.displayName;
                    }
                }
            }

            if (string.IsNullOrEmpty(propertyDisplayName))
            {
                propertyDisplayName = "Select property...";
                this.property = string.Empty;
            }
        }

        private void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(graphic.material != localMaterial)
            {
                OnPropertyInitialization(property, label);
            }

            position = EditorGUI.PrefixLabel(position, label);

            if(GUI.Button(position, propertyDisplayName, EditorStyles.popup))
            {
                properties.DropDown(position);
            }
        }
#endif
#endregion

        #region [Getter / Setter]
        public Material GetMaterial()
        {
            return material;
        }

        public float GetValue()
        {
            return value;
        }

        public void SetValue(float value)
        {
            this.value = value;
        }
#endregion
    }
}