/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.UIModules.IMGUI.Crosshair
{
    public static class CrosshairUtility
    {
        /// <summary>
        /// Get screen center position in Vector2 representation.
        /// </summary>
        /// <returns>Screen center position.</returns>
        public static Vector2 GetScreenCenter()
        {
            return new Vector2(Screen.width / 2, Screen.height / 2);
        }

        /// <summary>
        /// Create a new square texture.
        /// </summary>
        /// <returns>Created texture.</returns>
        public static Texture2D CreateTexture()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Create a new square texture.
        /// </summary>
        /// <param name="color">Texture color.</param>
        /// <returns>Created texture with specific color.</returns>
        public static Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}

