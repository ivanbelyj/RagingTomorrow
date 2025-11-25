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
using AuroraFPSRuntime.UIModules.UIElements;
using AuroraFPSRuntime.UIModules.UIElements.Animation;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AuroraFPSRuntime.CoreModules.SceneManagement
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Core Modules/Scene Management/Target Scene Loader")]
    [DisallowMultipleComponent]
    public sealed class TargetSceneLoader : MonoBehaviour, ILoadingProgress
    {
        [SerializeField]
        [Order(501)]
        private Transition transition = null;

        [SerializeField]
        [Slider(0.001f, 1.0f)]
        [Order(601)]
        private float timeMultiplier = 0.25f;

        // Stored required properties.
        private float loadingProgress;
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            coroutineObject = new CoroutineObject(this);
            coroutineObject.Start(LoadProcessing);
        }

        /// <summary>
        /// Get current scene loading progress.
        /// </summary>
        public float GetLoadingProgress()
        {
            return loadingProgress;
        }

        /// <summary>
        /// Loading processing with transition.
        /// </summary>
        private IEnumerator LoadProcessing()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneLoadButton.TargetScene, LoadSceneMode.Single);
            asyncOperation.allowSceneActivation = false;

            loadingProgress = 0.0f;
            while (loadingProgress < 1.0f)
            {
                loadingProgress = Mathf.MoveTowards(loadingProgress, asyncOperation.progress / 0.9f, timeMultiplier * Time.unscaledDeltaTime);
                yield return null;
            }

            if (transition != null)
            {
                yield return transition.WaitForFadeIn();
            }

            SceneLoadButton.TargetScene = string.Empty;
            loadingProgress = 0.0f;
            asyncOperation.allowSceneActivation = true;
        }

        #region [Getter / Setter]
        public Transition GetTransition()
        {
            return transition;
        }

        public void SetTransition(Transition value)
        {
            transition = value;
        }

        public float GetTimeMultiplier()
        {
            return timeMultiplier;
        }

        public void SetTimeMultiplier(float value)
        {
            timeMultiplier = value;
        }
        #endregion
    }
}
