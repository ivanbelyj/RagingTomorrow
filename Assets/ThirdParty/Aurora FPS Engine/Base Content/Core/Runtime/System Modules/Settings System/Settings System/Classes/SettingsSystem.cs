/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov, Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    public static class SettingsSystem
    {
        /// <summary>
        /// Current settings config asset.
        /// </summary>
        public static SettingsConfig Config;

        /// <summary>
        /// Buffer containing information of loaded settings.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, object>> Buffer;

        static SettingsSystem()
        {
            Buffer = new Dictionary<string, Dictionary<string, object>>();
        }

        /// <summary>
        /// Called once before splash screen.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Config = Resources.LoadAll<SettingsConfig>(string.Empty).FirstOrDefault();
            if (Config == null)
            {
                Config = ScriptableObject.CreateInstance<JSONSettingsConfig>();
            }
            LoadAll();
        }

        /// <summary>
        /// Save specified settings section.
        /// </summary>
        public static void Save(string section)
        {
            if (Buffer.ContainsKey(section))
            {
                OnBeforeSaveCallback?.Invoke(section);
                string path = Config.GetSectionPath(section);
                if (!string.IsNullOrEmpty(path))
                {
                    Config.Save(path, Buffer[section]);
                }
                OnSaveCallback?.Invoke(section);
            }
        }

        /// <summary>
        /// Load specified settings section.
        /// </summary>
        public static void Load(string section)
        {
            string path = Config.GetSectionPath(section);
            if (!string.IsNullOrEmpty(path))
            {
                Config.Load(path, out Dictionary<string, object> sectionData);
                Buffer[section] = sectionData;
                OnLoadCallback?.Invoke(section);
            }
        }

        public static void SaveAll()
        {
            for (int i = 0; i < Config.GetSectionCount(); i++)
            {
                Save(Config.GetSection(i).GetName());
            }
        }

        public static void LoadAll()
        {
            for (int i = 0; i < Config.GetSectionCount(); i++)
            {
                Load(Config.GetSection(i).GetName());
            }
        }

        public static void AddValue(string section, string guid, object value)
        {
            Buffer[section][guid] = value;
        }

        public static void RemoveValue(string section, string guid)
        {
            if (Buffer.TryGetValue(section, out Dictionary<string, object> sectionData))
            {
                if (sectionData.ContainsKey(guid))
                {
                    sectionData.Remove(guid);
                }
            }
        }

        public static bool TryGetValue(string section, string guid, out object value)
        {
            if (Buffer.TryGetValue(section, out Dictionary<string, object> sectionBuffer))
            {
                return sectionBuffer.TryGetValue(guid, out value);
            }
            value = null;
            return false;
        }

        #region[Event Callback Functions]
        /// <summary>
        /// Called when specified section being loaded.
        /// </summary>
        public static event Action<string> OnLoadCallback;

        /// <summary>
        /// Called when specified section starts to be saved.
        /// </summary>
        public static event Action<string> OnBeforeSaveCallback;

        /// <summary>
        /// Called when specified section being saved.
        /// </summary>
        public static event Action<string> OnSaveCallback;
        #endregion
    }
}