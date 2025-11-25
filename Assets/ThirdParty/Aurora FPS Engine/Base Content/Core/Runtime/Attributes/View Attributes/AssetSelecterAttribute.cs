/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using System.IO;

namespace AuroraFPSRuntime.Attributes
{
    public sealed class AssetSelecterAttribute : ViewAttribute
    {
        public AssetSelecterAttribute()
        {
            this.AssetType = null;
            this.Path = "Assets";
            this.Search = SearchOption.AllDirectories;
        }

        #region [Parameters]
        /// <summary>
        /// Target asset search type.
        /// </summary>
        public Type AssetType { get; set; }
        
        /// <summary>
        /// Search asset path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Search asset option.
        /// </summary>
        public SearchOption Search { get; set; }
        #endregion
    }
}