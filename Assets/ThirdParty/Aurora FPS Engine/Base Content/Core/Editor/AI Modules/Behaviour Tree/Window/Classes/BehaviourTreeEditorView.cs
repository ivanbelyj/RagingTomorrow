/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine.UIElements;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public class BehaviourTreeEditorView : VisualElement
    {
        public BehaviourTreeEditor behaviourTreeEditor;

        public void SetBehaviourTreeEditor(BehaviourTreeEditor editor)
        {
            behaviourTreeEditor = editor;
        }
    }
}