/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.UIModules.UIElements.Animation;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Scene Management/Loading Tips")]
    [DisallowMultipleComponent]
    public sealed class LoadingTips : MonoBehaviour
    {
        [System.Serializable]
        private struct Tip
        {
            [SerializeField]
            private string text;

            [SerializeField]
            private float time;

            public Tip(string text, float time)
            {
                this.text = text;
                this.time = time;
            }

            public bool Equals(Tip tip)
            {
                return text == tip.GetText();
            }

            #region [Getter / Setter]
            public string GetText()
            {
                return text;
            }

            public void SetText(string value)
            {
                text = value;
            }

            public float GetTime()
            {
                return time;
            }

            public void SetTime(float value)
            {
                time = value;
            }
            #endregion
        }

        public enum FetchType
        {
            Sequental,
            Random
        }

        public enum ShowMethod
        {
            OnEnable,
            Manually
        }

        [SerializeField]
        private ShowMethod showMethod = ShowMethod.OnEnable;

        [SerializeField]
        private FetchType fetchType = FetchType.Random;

        [SerializeField]
        [NotNull]
        private Text text;

        [SerializeField]
        private Transition transition;

        [SerializeField]
        [VisibleIf("fetchType", "Random")]
        [MinValue(0)]
        private int bufferSize = 0;

        [SerializeField]
        [ReorderableList(OnElementGUICallback = "OnTipGUI")]
        private Tip[] tips;

        // Stored required properties.
        private Queue<string> buffer;
        private HashSet<string> bufferHash;
        private CoroutineObject coroutineObject;

        
        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            buffer = new Queue<string>(bufferSize);
            bufferHash = new HashSet<string>();
            coroutineObject = new CoroutineObject(this);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            if (showMethod == ShowMethod.OnEnable)
            {
                ShowTips();
            }
        }

        /// <summary>
        /// Start showing tips.
        /// </summary>
        public void ShowTips()
        {
            coroutineObject.Start(ShowingTipsProcessing, true);
        }

        /// <summary>
        /// Pause showing tips.
        /// </summary>
        public void Pause()
        {
            coroutineObject.Stop();
        }

        /// <summary>
        /// Showing tips processing coroutine.
        /// </summary>
        private IEnumerator ShowingTipsProcessing()
        {
            int lastIndex = 0;
            while (true)
            {
                Tip tip = default;
                switch (fetchType)
                {
                    case FetchType.Sequental:
                        tip = tips[lastIndex++];
                        if(lastIndex >= tips.Length)
                        {
                            lastIndex = 0;
                        }
                        break;

                    case FetchType.Random:
                        if(bufferSize > 0)
                        {
                            do
                            {
                                tip = tips[Random.Range(0, tips.Length)];
                                yield return null;
                            }
                            while (!bufferHash.Add(tip.GetText()));

                            if (buffer.Count >= bufferSize)
                            {
                                bufferHash.Remove(buffer.Dequeue());
                            }
                            buffer.Enqueue(tip.GetText());
                        }
                        else
                        {
                            tip = tips[Random.Range(0, tips.Length)];
                        }
                        break;
                }
                text.text = tip.GetText();

                if(transition != null)
                {
                    yield return transition.WaitForFadeIn();
                }

                yield return new WaitForSeconds(tip.GetTime());

                if (transition != null)
                {
                    yield return transition.WaitForFadeOut();
                }
            }
        }

        #region [Editor Section]
#if UNITY_EDITOR
        private void OnTipGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.SerializedProperty textProperty = property.FindPropertyRelative("text");
            Rect textPosition = new Rect(position.x, position.y, position.width - 33, UnityEditor.EditorGUIUtility.singleLineHeight);
            textProperty.stringValue = UnityEditor.EditorGUI.TextField(textPosition, textProperty.stringValue);

            UnityEditor.SerializedProperty timeProperty = property.FindPropertyRelative("time");
            Rect timePosition = new Rect(textPosition.xMax + 2, position.y, position.width - textPosition.width, UnityEditor.EditorGUIUtility.singleLineHeight);
            timeProperty.floatValue = UnityEditor.EditorGUI.FloatField(timePosition, timeProperty.floatValue);
        }

        [UnityEditor.MenuItem("GameObject/Aurora FPS Engine/UI/Scene Management/Loading Tips", false, 31)]
        private static void CreateTip()
        {
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
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            GameObject tipObject = new GameObject("Tips");
            tipObject.transform.SetParent(canvas.transform);

            RectTransform rectTransform = tipObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
            rectTransform.offsetMin = new Vector2(100, 50);
            rectTransform.offsetMax = new Vector2(-100, 50);
            rectTransform.sizeDelta += new Vector2(0, 50);

            Text text = tipObject.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 1;
            text.resizeTextMaxSize = 20;
            text.raycastTarget = false;
            text.maskable = true;

            SingleTransition singleTransition = tipObject.AddComponent<SingleTransition>();
            singleTransition.SetElement(text);
            singleTransition.SetMaxAlpha(1.0f);

            LoadingTips loadingTips = tipObject.AddComponent<LoadingTips>();
            loadingTips.showMethod = ShowMethod.OnEnable;
            loadingTips.fetchType = FetchType.Random;
            loadingTips.text = text;
            loadingTips.transition = singleTransition;
            loadingTips.bufferSize = 3;
            loadingTips.tips = new Tip[3]
            {
                new Tip("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", 1.5f),
                new Tip("Nam eget vulputate velit, ac iaculis leo.", 1.5f),
                new Tip("Suspendisse tristique nunc a metus dictum, in interdum velit sodales.", 1.5f)
            };

            tipObject.AddComponent<Outline>();
            tipObject.AddComponent<Shadow>();

            UnityEditor.Selection.activeGameObject = tipObject;
            UnityEditor.EditorGUIUtility.PingObject(tipObject);
        }
#endif
#endregion

            #region [Getter / Setter]
            public ShowMethod GetShowMethod()
        {
            return showMethod;
        }

        public void SetShowMethod(ShowMethod value)
        {
            showMethod = value;
        }

        public FetchType GetFetchType()
        {
            return fetchType;
        }

        public void SetFetchType(FetchType value)
        {
            fetchType = value;
        }

        public Text GetTextComponent()
        {
            return text;
        }

        public void SetTextComponent(Text value)
        {
            text = value;
        }

        public int GetBufferSize()
        {
            return bufferSize;
        }

        public void SetBufferSize(int value)
        {
            bufferSize = value;
        }

        private Tip[] GetTips()
        {
            return tips;
        }

        private void SetTips(Tip[] value)
        {
            tips = value;
        }
        #endregion
    }
}
