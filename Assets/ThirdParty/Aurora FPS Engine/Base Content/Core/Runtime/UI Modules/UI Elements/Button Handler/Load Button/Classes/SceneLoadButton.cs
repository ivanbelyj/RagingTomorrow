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
using AuroraFPSRuntime.CoreModules.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Scene Management/Button/Scene Load Button")]
    [DisallowMultipleComponent]
    public sealed class SceneLoadButton : UnityButton, ILoadingProgress
    {
        internal static string TargetScene = string.Empty;

        [SerializeField]
        [SceneSelecter]
        private string targetScene = string.Empty;

        [SerializeField]
        [SceneSelecter]
        private string loadingScene = string.Empty;

        [SerializeField]
        [Slider(0.001f, 1.0f)]
        private float timeMultiplier = 1.0f;

        [SerializeField]
        private Animation.Transition fadeTransition;

        // Stored required properties.
        private float loadingProgress;
        private CoroutineObject coroutineObject;

        protected override void Awake()
        {
            base.Awake();
            coroutineObject = new CoroutineObject(this);
        }

        /// <summary>
        /// Called before invoking OnClick events.
        /// </summary>
        protected override void OnBeforeClick()
        {
            if(fadeTransition != null)
            {
                fadeTransition.FadeIn();
            }
            coroutineObject.Start(LoadScene);
        }

        /// <summary>
        /// Get current scene loading progress.
        /// </summary>
        public float GetLoadingProgress()
        {
            return loadingProgress;
        }

        private IEnumerator LoadScene()
        {
            bool hasLoadScene = !string.IsNullOrEmpty(loadingScene);
            string nextScene = hasLoadScene ? loadingScene : targetScene;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
            if (hasLoadScene)
            {
                TargetScene = targetScene;
            }

            asyncOperation.allowSceneActivation = false;

            loadingProgress = 0.0f;
            while (loadingProgress < 1.0f)
            {
                loadingProgress = Mathf.MoveTowards(loadingProgress, asyncOperation.progress / 0.9f, timeMultiplier * Time.unscaledDeltaTime);
                yield return null;
            }

            while (fadeTransition != null && fadeTransition.IsFading())
            {
                yield return null;
            }

            loadingProgress = 0.0f;
            asyncOperation.allowSceneActivation = true;
        }

        #region [Unity Editor]
#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Aurora FPS Engine/UI/Scene Management/Scene Load Button", false, 30)]
        private static void CreateButton()
        {
            GameObject button = UnityEngine.UI.DefaultControls.CreateButton(new UnityEngine.UI.DefaultControls.Resources());
            button.name = "Scene Load Button";

            DestroyImmediate(button.GetComponent<UnityEngine.UI.Button>());
            button.AddComponent<SceneLoadButton>();

            button.GetComponent<UnityEngine.UI.Image>().sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            button.GetComponentInChildren<UnityEngine.UI.Text>().text = "Load";

#if UNITY_2022_2_OR_NEWER
            Canvas canvas = FindAnyObjectByType<Canvas>();
#else
            Canvas canvas = FindObjectOfType<Canvas>();
#endif
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject();
                canvasGO.name = "Canvas";
                canvasGO.AddComponent<Canvas>();

                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            button.transform.SetParent(canvas.transform);
            button.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

            UnityEditor.Selection.activeGameObject = button;
            UnityEditor.EditorGUIUtility.PingObject(button);
        }

#endif
#endregion

            #region [Getter / Setter]
            public string GetTargetScene()
        {
            return targetScene;
        }

        public void SetTargetScene(string value)
        {
            targetScene = value;
        }

        public string GetLoadingScene()
        {
            return loadingScene;
        }

        public void SetLoadingScene(string value)
        {
            loadingScene = value;
        }

        public Animation.Transition GetFadeTransition()
        {
            return fadeTransition;
        }

        public void SetFadeTransition(Animation.Transition value)
        {
            fadeTransition = value;
        }
        #endregion
    }
}
