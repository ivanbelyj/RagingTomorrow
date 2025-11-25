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

namespace AuroraFPSRuntime.UIModules.IMGUI.Crosshair
{
    [System.Serializable]
    public class CrosshairElement
    {
        [SerializeField] 
        private Texture2D texture;

        [SerializeField] 
        [MinValue(0)]
        private float width = 2.0f;

        [SerializeField]
        [MinValue(0)]
        private float height = 17.0f;

        [SerializeField]
        private float xPositionOffset = 0.0f;

        [SerializeField]
        private float yPositionOffset = 0.0f;

        [SerializeField] 
        private Color color = Color.white;

        [SerializeField] 
        [MinValue(0.0f)]
        private float outlineAmount = 0.0f;

        [SerializeField] 
        [VisibleIf("outlineAmount", ">", "0")]
        private Color outlineColor = Color.black;

        [SerializeField] 
        private bool visible = true;

        public CrosshairElement() { }

        /// <summary>
        /// Crosshair element constructor.
        /// </summary>
        /// <param name="texture">Crosshair element texture.</param>
        /// <param name="width">Crosshair element width.</param>
        /// <param name="height">Crosshair element height.</param>
        /// <param name="color">Crosshair element color.</param>
        /// <param name="outlineAmount">Crosshair element ouline amount value.</param>
        /// <param name="outlineColor">Crosshair element outline color.</param>
        /// <param name="visible">Crosshair element visibility.</param>
        public CrosshairElement(Texture2D texture, float width, float height, float xPositionOffset, float yPositionOffset, Color color, float outlineAmount, Color outlineColor, bool visible)
        {
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.xPositionOffset = xPositionOffset;
            this.yPositionOffset = yPositionOffset;
            this.color = color;
            this.outlineAmount = outlineAmount;
            this.outlineColor = outlineColor;
            this.visible = visible;
        }

        /// <summary>
        /// Crosshair element constructor.
        /// </summary>
        /// <param name="texture">Crosshair element texture.</param>
        /// <param name="width">Crosshair element width.</param>
        /// <param name="height">Crosshair element height.</param>
        /// <param name="visible">Crosshair element visibility.</param>
        public CrosshairElement(Texture2D texture, float width, float height, bool visible)
        {
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.xPositionOffset = 0.0f;
            this.yPositionOffset = 0.0f;
            this.color = Color.white;
            this.outlineAmount = 1.0f;
            this.outlineColor = Color.black;
            this.visible = visible;
        }

        /// <summary>
        /// Crosshair element constructor.
        /// </summary>
        /// <param name="texture">Crosshair element texture.</param>
        /// <param name="width">Crosshair element width.</param>
        /// <param name="height">Crosshair element height.</param>
        public CrosshairElement(Texture2D texture, float width, float height)
        {
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.xPositionOffset = 0.0f;
            this.yPositionOffset = 0.0f;
            this.color = Color.white;
            this.outlineAmount = 1.0f;
            this.outlineColor = Color.black;
            this.visible = true;
        }

        #region [Getter / Setter]
        public Texture2D GetTexture()
        {
            return texture;
        }

        public void SetTexture(Texture2D value)
        {
            texture = value;
        }

        public float GetWidth()
        {
            return width;
        }

        public void SetWidth(float value)
        {
            width = value;
        }

        public float GetHeight()
        {
            return height;
        }

        public void SetHeight(float value)
        {
            height = value;
        }

        public float GetXPositionOffset()
        {
            return xPositionOffset;
        }

        public void SetXPositionOffset(float value)
        {
            xPositionOffset = value;
        }

        public float GetYPositionOffset()
        {
            return yPositionOffset;
        }

        public void SetYPositionOffset(float value)
        {
            yPositionOffset = value;
        }

        public Color GetColor()
        {
            return color;
        }

        public void SetColor(Color value)
        {
            color = value;
        }

        public float GetOutlineAmount()
        {
            return outlineAmount;
        }

        public void SetOutlineAmount(float value)
        {
            outlineAmount = value;
        }

        public Color GetOutlineColor()
        {
            return outlineColor;
        }

        public void SetOutlineColor(Color value)
        {
            outlineColor = value;
        }

        public bool Visible()
        {
            return visible;
        }

        public void Visible(bool value)
        {
            visible = value;
        }
        #endregion
    }
}