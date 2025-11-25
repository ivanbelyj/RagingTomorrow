/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor;

namespace AuroraFPSEditor
{
    static class DocsMenu
    {
        const string ManualURL = "https://renowned-games.gitbook.io/aurora-fps-toolkit-manual/";

        [MenuItem("Aurora FPS Engine/Documentation/Manual", false, 10)]
        static void OpenManual()
        {
            Help.BrowseURL(ManualURL);
        }
    }
}
