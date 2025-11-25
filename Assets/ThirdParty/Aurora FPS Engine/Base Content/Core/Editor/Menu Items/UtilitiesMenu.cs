/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS
   Publisher :    Tamerlan Global Inc.
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using System.IO;
using UnityEditor;

namespace AuroraFPSEditor
{
    public static class UtilitiesMenu
    {
        //[MenuItem("Aurora FPS Engine/Utilities/Install Project Settings", false, 999)]
        public static void InstallProjectSettings()
        {
            const string RELATIVE_PATH = "/Base Content/Core/Editor/Editor Resources/Library Assets/ProjectSettings.unitypackage";
            string path = Path.Combine(ApexSettings.Current.GetRootPath() + RELATIVE_PATH);
            AssetDatabase.ImportPackage(path, false);
        }

        //[MenuItem("Aurora FPS Engine/Utilities/Export Project Settings", false, 999)]
        public static void ExportProjectSettings()
        {
            AssetDatabase.ExportPackage("", "INTERNAL_PROJECT_SETTINGS.unitypackage", ExportPackageOptions.IncludeLibraryAssets);
        }
    }
}
