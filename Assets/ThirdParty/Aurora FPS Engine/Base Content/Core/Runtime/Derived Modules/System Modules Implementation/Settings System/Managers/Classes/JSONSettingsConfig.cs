/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov, Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using System.IO;
using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.Utilities.FullSerializer;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    public class JSONSettingsConfig : SettingsConfig
    {
        private fsSerializer serializer = new fsSerializer();

        /// <summary>
        /// Save all game settings to JSON file.
        /// </summary>
        protected internal override void Save(string path, Dictionary<string, object> buffer)
        {
            if (buffer != null && buffer.Count > 0)
            {
                serializer.TrySerialize(typeof(Dictionary<string, object>), buffer, out fsData data).AssertSuccessWithoutWarnings();
                string json = fsJsonPrinter.CompressedJson(data);
                OnCompressedJson(json, out json);
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    writer.WriteLine(json);
                }
            }
        }

        /// <summary>
        /// Load all game settings from JSON file.
        /// </summary>
        protected internal override void Load(string path, out Dictionary<string, object> buffer)
        {
            buffer = new Dictionary<string, object>();
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string json = reader.ReadLine();
                    if (!string.IsNullOrEmpty(json))
                    {
                        OnReadJson(json, out json);
                        fsData data = fsJsonParser.Parse(json);
                        object deserialized = null;
                        serializer.TryDeserialize(data, typeof(Dictionary<string, object>), ref deserialized).AssertSuccessWithoutWarnings();
                        buffer = (Dictionary<string, object>)deserialized;
                    }
                }
            }
        }

        /// <summary>
        /// <br>Called when settings processors compressed to json text, before write it to file.</br>
        /// <br>Implement this method to override json text, which will be saved.</br>
        /// </summary>
        /// <param name="json">Compressed json text.</param>
        /// <param name="text">Text, to save in file.</param>
        protected virtual void OnCompressedJson(string json, out string text)
        {
            text = json;
        }

        /// <summary>
        /// <br>Called at the end of the text reading stream from the file.</br>
        /// <br>Implement this method to override streamed text before trying deserialize json to settings processors.</br>
        /// </summary>
        /// <param name="text">Streamed text.</param>
        /// <param name="json">Json text for deserializing to settings processors.</param>
        protected virtual void OnReadJson(string text, out string json)
        {
            json = text;
        }
    }
}
