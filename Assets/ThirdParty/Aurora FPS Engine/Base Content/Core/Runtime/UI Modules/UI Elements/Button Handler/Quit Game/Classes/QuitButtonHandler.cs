/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.UIModules.UIElements.Animation;
using UnityEngine;
using System.Collections;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Button Handler/Quit Button Handler")]
    [DisallowMultipleComponent]
    public sealed class QuitButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private Transition transition;

        // Stored required properties.
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            coroutineObject = new CoroutineObject(this);
        }

        public void QuitGame()
        {
            if (transition != null)
                coroutineObject.Start(DelayedQuit, true);
            else
                Quit();
        }

        private IEnumerator DelayedQuit()
        {
            yield return transition.WaitForFadeIn();
            Quit();
        }

        private void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
