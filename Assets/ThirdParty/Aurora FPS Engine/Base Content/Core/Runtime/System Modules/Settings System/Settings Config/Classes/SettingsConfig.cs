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
using System.IO;
using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class SettingsConfig : ScriptableObject
    {
        [Serializable]
        public struct Section
        {
            public enum Directory
            {
                Persistent,
                Installed
            }

            [SerializeField]
            private Directory directory;

            [SerializeField]
            [NotEmpty]
            private string name;

            [SerializeField]
            [NotEmpty]
            private string fileName;

            [SerializeField]
            private string relativePath;

            [SerializeField]
            [NotEmpty]
            [Prefix("*.", Style = "Parameter")]
            private string fileExtension;

            public Section(Directory directory, string name, string fileName, string relativePath, string fileExtension)
            {
                this.directory = directory;
                this.name = name;
                this.fileName = fileName;
                this.relativePath = relativePath;
                this.fileExtension = fileExtension;
            }

            #region [Getter / Setter]
            public Directory GetDirectory()
            {
                return directory;
            }

            public void SetDirectory(Directory value)
            {
                directory = value;
            }

            public string GetName()
            {
                return name;
            }

            public void SetName(string value)
            {
                name = value;
            }

            public string GetFileName()
            {
                return fileName;
            }

            public void SetFileName(string value)
            {
                fileName = value;
            }

            public string GetRelativePath()
            {
                return relativePath;
            }

            public void SetRelativePath(string value)
            {
                relativePath = value;
            }

            public string GetFileExtension()
            {
                return fileExtension;
            }

            public void SetFileExtension(string value)
            {
                fileExtension = value;
            }
            #endregion
        }

        [SerializeField]
        [ReorderableList(GetElementLabelCallback = "GetSettingsSectionLabel")]
        private Section[] sections = DefalutSections;

        #region [Abstract methods]
        /// <summary>
        /// Save all current game settings to file
        /// </summary>
        /// <param name="settingsDataPath">Data file path</param>
        /// <param name="settings">Settings</param>
        protected internal abstract void Save(string path, Dictionary<string, object> settings);

        /// <summary>
        /// Load all game settings from file
        /// </summary>
        /// <param name="path">Data file Path</param>
        /// <param name="settings">Settings</param>
        protected internal abstract void Load(string path, out Dictionary<string, object> settings);
        #endregion

        /// <summary>
        /// Absolute path to file of specified section name.
        /// </summary>
        public string GetSectionPath(string name)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                Section section = sections[i];
                if (section.GetName() == name)
                {
                    string directory = null;
                    switch (section.GetDirectory())
                    {
                        default:
                        case Section.Directory.Persistent:
                            directory = Application.persistentDataPath;
                            break;
                        case Section.Directory.Installed:
                            directory = Application.dataPath;
                            break;
                    }

                    directory = Path.Combine(directory, section.GetRelativePath());
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    string fileNameWithExtension = string.Format("{0}.{1}", section.GetFileName(), section.GetFileExtension());
                    return Path.Combine(directory, fileNameWithExtension);
                }
            }
            return null;
        }

        #region [Static Methods]
        public static Section[] DefalutSections
        {
            get
            {
                return new Section[4]
                {
                    new Section(Section.Directory.Persistent, "General", "general", "Config", "txt"),
                    new Section(Section.Directory.Persistent, "Video", "video", "Config", "txt"),
                    new Section(Section.Directory.Persistent, "Audio", "audio", "Config", "txt"),
                    new Section(Section.Directory.Persistent, "Input", "input", "Config", "txt")
                };
            }
        }
        #endregion

        #region [Editor Section]
#if UNITY_EDITOR
        private string GetSettingsSectionLabel(UnityEditor.SerializedProperty property, int index)
        {
            string label = property.FindPropertyRelative("name").stringValue;
            return !string.IsNullOrEmpty(label) ? label : string.Format("Section {0}", index + 1);
        }
#endif
        #endregion

        #region [Getter / Setter]
        public Section[] GetSections()
        {
            return sections;
        }

        public Section GetSection(int index)
        {
            return sections[index];
        }

        public int GetSectionCount()
        {
            return sections.Length;
        }
        #endregion
    }
}

