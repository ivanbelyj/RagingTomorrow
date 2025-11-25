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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Transition/Collection Transition")]
    [DisallowMultipleComponent]
    [System.Obsolete("Use Group Transition component instead.")]
    public sealed class CollectionTransition : Transition
    {
        [System.Serializable]
        public struct GraphicElement
        {
            [SerializeField]
            [NotNull]
            private Graphic source;

            [SerializeField]
            [Slider(0.1f, 1.0f)]
            private float maxAlpha;

            public GraphicElement(Graphic source)
            {
                this.source = source;
                this.maxAlpha = 1.0f;
            }

            public GraphicElement(Graphic source, float maxAlpha) : this(source)
            {
                this.maxAlpha = maxAlpha;
            }

            #region [Getter / Setter]
            public Graphic GetSource()
            {
                return source;
            }

            public void SetSource(Graphic value)
            {
                source = value;
            }

            public float GetMaxAlpha()
            {
                return maxAlpha;
            }

            public void SetMaxAlpha(float value)
            {
                maxAlpha = value;
            }
            #endregion
        }

        [SerializeField]
        [ReorderableList(ElementLabel = null, OnElementGUICallback = "OnGraphicElementGUI")]
        private GraphicElement[] elements;

        protected override void OnFadeIn(float smooth)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                GraphicElement element = elements[i];
                Color color = element.GetSource().color;
                color.a = element.GetMaxAlpha();
                element.GetSource().color = Color.Lerp(element.GetSource().color, color, smooth);
            }
        }

        protected override void OnFadeOut(float smooth)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                GraphicElement element = elements[i];
                Color color = element.GetSource().color;
                color.a = 0.0f;
                element.GetSource().color = Color.Lerp(element.GetSource().color, color, smooth);
            }
        }

        #region [Editor Section]
#if UNITY_EDITOR
        private void OnGraphicElementGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent content)
        {
            Rect sourcePosition = new Rect(position.x, position.y, position.width - 30, position.height);
            UnityEditor.EditorGUI.PropertyField(sourcePosition, property.FindPropertyRelative("source"), GUIContent.none);

            Rect maxAplhaPosition = new Rect(sourcePosition.xMax + 2, position.y, position.width - sourcePosition.width, position.height);
            UnityEditor.EditorGUI.PropertyField(maxAplhaPosition, property.FindPropertyRelative("maxAlpha"), GUIContent.none);
        }
#endif
        #endregion

        #region [Getter / Setter]
        public GraphicElement[] GetGraphicElements()
        {
            return elements;
        }

        public void SetGraphicElements(GraphicElement[] value)
        {
            elements = value;
        }
        #endregion
    }
}
