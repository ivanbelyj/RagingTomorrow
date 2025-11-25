/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Graphics/Shadow Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class ShadowSettingsProcessor : SettingsProcessor
    {
        [System.Serializable]
        public class ShadowLevel
        {
            [SerializeField]
            [NotEmpty]
            private string name = "Level 1";

            [SerializeField]
            private ShadowQuality quality = ShadowQuality.HardOnly;

            [SerializeField]
            private ShadowResolution resolution = ShadowResolution.Medium;

            [SerializeField]
            private ShadowProjection projection = ShadowProjection.StableFit;

            [SerializeField]
            private ShadowmaskMode mode = ShadowmaskMode.DistanceShadowmask;

            [SerializeField]
            [MinValue(0)]
            private int distance = 75;

            [SerializeField]
            [MinValue(0)]
            private int nearPlaneOffset = 3;

            public ShadowLevel(string name, ShadowQuality quality, ShadowResolution resolution, ShadowProjection projection, ShadowmaskMode mode, int distance, int nearPlaneOffset)
            {
                this.name = name;
                this.quality = quality;
                this.resolution = resolution;
                this.projection = projection;
                this.mode = mode;
                this.distance = distance;
                this.nearPlaneOffset = nearPlaneOffset;
            }

            #region [Getter / Setter]
            public string GetName()
            {
                return name;
            }

            public void SetName(string value)
            {
                name = value;
            }

            public ShadowQuality GetQuality()
            {
                return quality;
            }

            public void SetQuality(ShadowQuality value)
            {
                quality = value;
            }

            public ShadowResolution GetResolution()
            {
                return resolution;
            }

            public void SetResolution(ShadowResolution value)
            {
                resolution = value;
            }

            public ShadowProjection GetProjection()
            {
                return projection;
            }

            public void SetProjection(ShadowProjection value)
            {
                projection = value;
            }

            public ShadowmaskMode GetMode()
            {
                return mode;
            }

            public void SetMode(ShadowmaskMode value)
            {
                mode = value;
            }

            public int GetDistance()
            {
                return distance;
            }

            public void SetDistance(int value)
            {
                distance = value;
            }

            public int GetNearPlaneOffset()
            {
                return nearPlaneOffset;
            }

            public void SetNearPlaneOffset(int value)
            {
                nearPlaneOffset = value;
            }
            #endregion
        }

        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        [SerializeField]
        [NotEmpty]
        private string defaultLevel = DefaultLevels[3].GetName();

        [SerializeField]
        [ReorderableList(GetElementLabelCallback = "GetLevelLabel")]
        private ShadowLevel[] levels = DefaultLevels;


        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");
            Debug.Assert(levels != null, $"<b><color=#FF0000>Add shadow level to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Levels<i>(field)</i>.</color></b>");
            Debug.Assert(levels.Length != 0, $"<b><color=#FF0000>Add shadow level to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Levels<i>(field)</i>.</color></b>");

            dropdown.options.Clear();
            for (int i = 0; i < levels.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData(levels[i].GetName()));
            }
        }

        /// <summary>
        /// <br>Called when the settings manager saves the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Write</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to save processor value.</br>
        /// </summary>
        protected override object OnSave()
        {
            string level = dropdown.options[dropdown.value].text;
            for (int i = 0; i < levels.Length; i++)
            {
                ShadowLevel shadowLevel = levels[i];
                if(shadowLevel.GetName() == level)
                {
                    QualitySettings.shadows = shadowLevel.GetQuality();
                    QualitySettings.shadowResolution = shadowLevel.GetResolution();
                    QualitySettings.shadowProjection = shadowLevel.GetProjection();
                    QualitySettings.shadowmaskMode = shadowLevel.GetMode();
                    QualitySettings.shadowDistance = shadowLevel.GetDistance();
                    QualitySettings.shadowNearPlaneOffset = shadowLevel.GetNearPlaneOffset();
                }
            }
            return level;
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Read</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            string level = value.ToString();
            for (int i = 0; i < levels.Length; i++)
            {
                ShadowLevel shadowLevel = levels[i];
                if (shadowLevel.GetName() == level)
                {
                    QualitySettings.shadows = shadowLevel.GetQuality();
                    QualitySettings.shadowResolution = shadowLevel.GetResolution();
                    QualitySettings.shadowProjection = shadowLevel.GetProjection();
                    QualitySettings.shadowmaskMode = shadowLevel.GetMode();
                    QualitySettings.shadowDistance = shadowLevel.GetDistance();
                    QualitySettings.shadowNearPlaneOffset = shadowLevel.GetNearPlaneOffset();
                }
            }
            dropdown.value = dropdown.options.FindIndex(t => t.text == level);
        }

        /// <summary>
        /// <br>Called when settings file is not found or 
        /// target processor GUID is not found in loaded buffer.</br>
        /// <br>Implement this method to determine default value for this processor.</br>
        /// </summary>
        /// <returns>Default value of processor.</returns>
        public override object GetDefaultValue()
        {
            return defaultLevel;
        }

        #region [Static Properties]
        public static ShadowLevel[] DefaultLevels
        {
            get
            {
                return new ShadowLevel[5]
                {
                    new ShadowLevel("Disabled", ShadowQuality.Disable, ShadowResolution.Low, ShadowProjection.CloseFit, ShadowmaskMode.DistanceShadowmask, 0, 0),
                    new ShadowLevel("Low", ShadowQuality.HardOnly, ShadowResolution.Low, ShadowProjection.CloseFit, ShadowmaskMode.DistanceShadowmask, 35, 3),
                    new ShadowLevel("Medium", ShadowQuality.HardOnly, ShadowResolution.Medium, ShadowProjection.CloseFit, ShadowmaskMode.DistanceShadowmask, 75, 3),
                    new ShadowLevel("High", ShadowQuality.All, ShadowResolution.High, ShadowProjection.StableFit, ShadowmaskMode.DistanceShadowmask, 100, 3),
                    new ShadowLevel("Ultra", ShadowQuality.All, ShadowResolution.VeryHigh, ShadowProjection.StableFit, ShadowmaskMode.DistanceShadowmask, 125, 3)
                };
            }
        }
        #endregion

        #region [Editor Section]
#if UNITY_EDITOR
        private string GetLevelLabel(UnityEditor.SerializedProperty property, int index)
        {
            string label = property.FindPropertyRelative("name").stringValue;
            return !string.IsNullOrEmpty(label) ? label : string.Format("Level {0}", index + 1);
        }
#endif
#endregion
    }
}