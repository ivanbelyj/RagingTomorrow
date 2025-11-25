/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;


namespace AuroraFPSRuntime.UIModules.IMGUI.Crosshair
{
    [System.Serializable]
    public abstract class CrosshairPreset : ICrosshairPreset
    {
        public enum RotationSide
        {
            None,
            Left,
            Right
        }

        // Base crosshair preset properties.
        [SerializeField] 
        [Order(200)]
        private RotationSide rotationSide = RotationSide.None;

        [SerializeField]
        [VisibleIf("rotationSide", "Left,Right")]
        [Order(201)]
        private float angle = 0.0f;

        [SerializeField]
        [VisibleIf("rotationSide", "Left,Right")]
        [Order(202)]
        private float rotationSpeed = 25.0f;

        [SerializeField]
        [Order(203)]
        private ControllerState hideState = ControllerState.Disabled;

        // Stored required properties.
        private PlayerController controller;
        private Vector2 screenCenter;
        private Color storedColor;
        private float updatedAngle;


        /// <summary>
        /// Initialize crosshair preset.
        /// </summary>
        /// <param name="controller">First person controller reference.</param>
        public virtual void Initialize(PlayerController controller)
        {
            this.controller = controller;
            this.screenCenter = CrosshairUtility.GetScreenCenter();
        }

        /// <summary>
        /// Draw crosshair elements GUI layout taking into account hide state.
        /// </summary>
        /// <param name="spread"></param>
        public virtual void DrawElementsLayout(float spread)
        {
            if (hideState == ControllerState.Disabled || (controller.GetState() & hideState) == 0)
            {
                ProcesssingRotation();
                GUIUtility.RotateAroundPivot(updatedAngle, screenCenter);
                OnElementsGUI(spread);
            }

        }

        /// <summary>
        /// Crosshair elements GUI layout.
        /// </summary>
        /// <param name="spread">Specific spread value calculated by controller state.</param>
        protected abstract void OnElementsGUI(float spread);

        /// <summary>
        /// Processing crosshair rotation to specific side.
        /// </summary>
        protected void ProcesssingRotation()
        {
            if (rotationSide != RotationSide.None)
            {
                updatedAngle += (rotationSpeed * (rotationSide == RotationSide.Right ? 1 : -1)) * Time.deltaTime;
                updatedAngle = Math.Loop(updatedAngle, -360, 360);
            }
            else if (updatedAngle != angle)
            {
                updatedAngle = angle;
            }
        }

        /// <summary>
        /// Draw crosshair element on GUI.
        /// </summary>
        /// <param name="texture">Crosshair element texture.</param>
        /// 
        /// <param name="color">Crosshair element color.</param>
        /// <param name="visible">Crosshair element visibility.</param>
        public void DrawElement(Texture2D texture, Rect position, Color color, float outlineAmount, Color outlineColor, bool visible)
        {
            if (!visible)
                return;

            storedColor = GUI.color;

            if (outlineAmount > 0)
            {
                GUI.color = outlineColor;
                Rect outlinePosition = new Rect(position.x - outlineAmount, position.y - outlineAmount, position.width + 2 * outlineAmount, position.height + 2 * outlineAmount);
                GUI.DrawTexture(outlinePosition, texture);
            }

            GUI.color = color;
            GUI.DrawTexture(position, texture);
            GUI.color = storedColor;
        }

        /// <summary>
        /// Draw crosshair element on GUI.
        /// </summary>
        /// <param name="element">CrosshairElement reference.</param>
        /// <param name="x">Crosshair element position by X axis.</param>
        /// <param name="y">Crosshair element position by Y axis.</param>
        /// <param name="width">Crosshair element width.</param>
        /// <param name="height">Crosshair element height.</param>
        public void DrawElement(CrosshairElement element, float x, float y, float width, float height)
        {
            Rect position = new Rect(x + element.GetXPositionOffset(), y + element.GetYPositionOffset(), width, height);
            DrawElement(element.GetTexture(), position, element.GetColor(), element.GetOutlineAmount(), element.GetOutlineColor(), element.Visible());
        }

        /// <summary>
        /// Draw crosshair element on GUI.
        /// </summary>
        /// <param name="element">CrosshairElement reference.</param>
        /// <param name="x">Crosshair element position by X axis.</param>
        /// <param name="y">Crosshair element position by Y axis.</param>
        public void DrawElement(CrosshairElement element, float x, float y)
        {
            DrawElement(element, x + element.GetXPositionOffset(), y + element.GetYPositionOffset(), element.GetWidth(), element.GetHeight());
        }

        /// <summary>
        /// Draw crosshair element on GUI.
        /// </summary>
        /// <param name="element">CrosshairElement reference.</param>
        /// <param name="position">Element position in Rectangle representation.</param>
        public void DrawElement(CrosshairElement element, Rect position)
        {
            DrawElement(element, position.x, position.y, position.width, position.height);
        }

        /// <summary>
        /// Create new squad texture with specific color.
        /// </summary>
        /// <param name="color">Texture color.</param>
        /// <returns>Texture with specific color</returns>
        public Texture2D CreateSquadTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        #region [Getter / Setter]
        public ControllerState GetHideState()
        {
            return hideState;
        }

        public void SetHideState(ControllerState value)
        {
            hideState = value;
        }

        public Controller GetController()
        {
            return controller;
        }
        #endregion
    }
}