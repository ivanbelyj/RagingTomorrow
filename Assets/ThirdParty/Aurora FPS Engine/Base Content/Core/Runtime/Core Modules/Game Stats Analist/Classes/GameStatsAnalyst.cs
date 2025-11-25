/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Pattern;
using UnityEngine;
using UnityEngine.UI;
using AuroraFPSRuntime.AIModules;
using AuroraFPSRuntime.SystemModules.ControllerSystems;

namespace AuroraFPSRuntime.CoreModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    internal sealed class GameStatsAnalyst : Singleton<GameStatsAnalyst>
    {
        private Text fpsText;
        private Text gpuTimeText;
        private Text controllerProperties;
        private Text controllerValues;
        private Text aiControllerProperties;
        private Text aiControllerValues;
        private float refreshTime = 0.25f;

        // Stored required components.
        private Transform fpsCounter;
        private Transform gpuTimeCounter;
        private Transform controllerStats;
        private Transform aiControllerStats;
        private PlayerController controller;
        private AIController aiController;

        // Stored required properties.
        private float timer;
        private float delta;

        protected override void Awake()
        {
            base.Awake();
            InstantiateUIElements();
            ShowFPS(false);
            ShowGPUTime(false);
            ShowControllerStats(false);
            ShowAIControllerStats(null);
        }

        private void Update()
        {
            delta += (Time.unscaledDeltaTime - delta) * 0.1f;
            if (Time.unscaledTime > timer)
            {
                if (fpsText != null && fpsCounter.gameObject.activeSelf)
                {
                    int fps = (int)(1f / Time.unscaledDeltaTime);
                    fpsText.text = fps.ToString();
                }

                if (gpuTimeText != null && gpuTimeCounter.gameObject.activeSelf)
                {
                    float ms = delta * 1000.0f;
                    gpuTimeText.text = string.Format("{0}ms", ms.ToString("##.##"));
                }

                timer = Time.unscaledTime + refreshTime;
            }

            if (controller != null && controllerStats.gameObject.activeSelf)
            {
                controllerProperties.text = "Speed\nInput\nVelocity\nState\nIsGrounded\nIsMoving\nIsCrouched";
                controllerValues.text = string.Format(":{0}\n:{1}\n:{2}\n:{3}\n:{4}\n:{5}\n:{6}", 
                    controller.GetSpeed(),
                    controller.GetControlInput(),
                    controller.GetVelocity(),
                    controller.GetState().ToString(),
                    controller.IsGrounded(),
                    controller.IsMoving(),
                    controller.IsCrouched());
            }

            if (aiController != null && aiControllerStats.gameObject.activeSelf)
            {
                aiControllerProperties.text = "Speed\nVelocity\nBehaviour";
                aiControllerValues.text = string.Format(":{0}\n:{1}\n:{2}",
                    aiController.GetVelocity().magnitude,
                    aiController.GetVelocity(),
                    aiController.GetActiveBehaviour());
            }
        }

        public void ShowFPS(bool value)
        {
            fpsCounter.gameObject.SetActive(value);
        }

        public void ShowGPUTime(bool value)
        {
            gpuTimeCounter.gameObject.SetActive(value);
        }

        public void ShowControllerStats(bool value)
        {
            if(controller == null)
            {
#if UNITY_2022_2_OR_NEWER
                controller = FindAnyObjectByType<PlayerController>();
#else
                controller = Object.FindObjectOfType<PlayerController>();
#endif
            }
            controllerStats.gameObject.SetActive(value && controller != null);
        }

        public void ShowAIControllerStats(AIController aiController)
        {
            this.aiController = aiController;
            aiControllerStats.gameObject.SetActive(aiController != null);
        }

        public void RefreshTime(float time)
        {
            refreshTime = time;
        }

        private void InstantiateUIElements()
        {
            GameObject canvasObject = new GameObject("Game Stats analysts");
            canvasObject.transform.SetParent(transform);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.sortingOrder = 999999;
            canvas.targetDisplay = 0;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(800, 600);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            canvasScaler.referencePixelsPerUnit = 100;

            CreateSystemStats(canvasObject);
            CreateGridStats(canvasObject);
        }

        private void CreateSystemStats(GameObject canvas)
        {
            GameObject statsPanel = new GameObject("System Stats");
            RectTransform statsRectTransform = statsPanel.AddComponent<RectTransform>();
            statsRectTransform.SetParent(canvas.transform);
            statsRectTransform.anchorMin = new Vector2(0, 1);
            statsRectTransform.anchorMax = new Vector2(0, 1);
            statsRectTransform.pivot = new Vector2(0, 1);
            statsRectTransform.anchoredPosition = Vector2.zero;
            statsRectTransform.anchoredPosition3D = Vector3.zero;
            statsRectTransform.sizeDelta = Vector2.zero;

            statsPanel.AddComponent<CanvasRenderer>();

            HorizontalLayoutGroup horizontalLayoutGroup = statsPanel.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childControlHeight = false;
            horizontalLayoutGroup.childControlWidth = false;
            horizontalLayoutGroup.childScaleHeight = false;
            horizontalLayoutGroup.childScaleWidth = false;
            horizontalLayoutGroup.childForceExpandHeight = false;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            horizontalLayoutGroup.padding = new RectOffset(5, 0, 0, 0);
            horizontalLayoutGroup.spacing = 5;

            GameObject cPanel = new GameObject("FPS");
            RectTransform cPanelRectTransform = cPanel.AddComponent<RectTransform>();
            cPanelRectTransform.SetParent(statsRectTransform);
            cPanelRectTransform.anchorMin = new Vector2(0, 1);
            cPanelRectTransform.anchorMax = new Vector2(0, 1);
            cPanelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            cPanelRectTransform.anchoredPosition = Vector2.zero;
            cPanelRectTransform.anchoredPosition3D = Vector3.zero;
            cPanelRectTransform.sizeDelta = new Vector2(70, 20);

            cPanel.AddComponent<CanvasRenderer>();

            Image fpsBackground = cPanel.AddComponent<Image>();
            fpsBackground.sprite = null;
            fpsBackground.color = new Color(0, 0, 0, 150);
            fpsBackground.material = null;
            fpsBackground.raycastTarget = false;
            fpsBackground.maskable = false;

            GameObject fpsLabel = new GameObject("Label");
            RectTransform fpsLabelRectTransform = fpsLabel.AddComponent<RectTransform>();
            fpsLabelRectTransform.SetParent(cPanelRectTransform);
            fpsLabelRectTransform.anchorMin = Vector2.zero;
            fpsLabelRectTransform.anchorMax = new Vector2(0.57f, 1);
            fpsLabelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            fpsLabelRectTransform.anchoredPosition = Vector2.zero;
            fpsLabelRectTransform.anchoredPosition3D = Vector3.zero;
            fpsLabelRectTransform.sizeDelta = Vector2.zero;

            fpsLabel.AddComponent<CanvasRenderer>();

            Text fpsLabelText = fpsLabel.AddComponent<Text>();
            fpsLabelText.text = "FPS:";
            fpsLabelText.font = Font.CreateDynamicFontFromOSFont("Courier New", 16);
            fpsLabelText.fontStyle = FontStyle.Normal;
            fpsLabelText.fontSize = 17;
            fpsLabelText.lineSpacing = 0;
            fpsLabelText.supportRichText = false;
            fpsLabelText.alignment = TextAnchor.MiddleCenter;
            fpsLabelText.alignByGeometry = true;
            fpsLabelText.horizontalOverflow = HorizontalWrapMode.Wrap;
            fpsLabelText.verticalOverflow = VerticalWrapMode.Truncate;
            fpsLabelText.resizeTextForBestFit = true;
            fpsLabelText.resizeTextMinSize = 1;
            fpsLabelText.resizeTextMaxSize = 17;
            fpsLabelText.color = Color.white;
            fpsLabelText.material = null;
            fpsLabelText.raycastTarget = false;
            fpsLabelText.maskable = false;

            fpsLabel.AddComponent<Outline>();


            GameObject fpsCount = new GameObject("Count");
            RectTransform fpsCountRectTransform = fpsCount.AddComponent<RectTransform>();
            fpsCountRectTransform.SetParent(cPanelRectTransform);
            fpsCountRectTransform.SetAsLastSibling();
            fpsCountRectTransform.anchorMin = new Vector2(0.57f, 0);
            fpsCountRectTransform.anchorMax = Vector2.one;
            fpsCountRectTransform.pivot = new Vector2(0.5f, 0.5f);
            fpsCountRectTransform.anchoredPosition = Vector2.zero;
            fpsCountRectTransform.anchoredPosition3D = Vector3.zero;
            fpsCountRectTransform.sizeDelta = Vector2.zero;

            fpsCount.AddComponent<CanvasRenderer>();

            Text fpsCountText = fpsCount.AddComponent<Text>();
            fpsCountText.text = "---";
            fpsCountText.font = Font.CreateDynamicFontFromOSFont("Courier New", 16);
            fpsCountText.fontStyle = FontStyle.Normal;
            fpsCountText.fontSize = 17;
            fpsCountText.lineSpacing = 0;
            fpsCountText.supportRichText = false;
            fpsCountText.alignment = TextAnchor.MiddleCenter;
            fpsCountText.alignByGeometry = true;
            fpsCountText.horizontalOverflow = HorizontalWrapMode.Wrap;
            fpsCountText.verticalOverflow = VerticalWrapMode.Truncate;
            fpsCountText.resizeTextForBestFit = true;
            fpsCountText.resizeTextMinSize = 1;
            fpsCountText.resizeTextMaxSize = 17;
            fpsCountText.color = new Color(200, 200, 200, 255);
            fpsCountText.material = null;
            fpsCountText.raycastTarget = false;
            fpsCountText.maskable = false;

            fpsCount.AddComponent<Outline>();



            GameObject gpuTimePanel = new GameObject("GPU Time");
            RectTransform gpuTimePanelRectTransform = gpuTimePanel.AddComponent<RectTransform>();
            gpuTimePanelRectTransform.SetParent(statsRectTransform);
            gpuTimePanelRectTransform.anchorMin = new Vector2(0, 1);
            gpuTimePanelRectTransform.anchorMax = new Vector2(0, 1);
            gpuTimePanelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            gpuTimePanelRectTransform.anchoredPosition = Vector2.zero;
            gpuTimePanelRectTransform.anchoredPosition3D = Vector3.zero;
            gpuTimePanelRectTransform.sizeDelta = new Vector2(165, 20);

            gpuTimePanel.AddComponent<CanvasRenderer>();

            Image gpuTimeBackground = gpuTimePanel.AddComponent<Image>();
            gpuTimeBackground.sprite = null;
            gpuTimeBackground.color = new Color(0, 0, 0, 150);
            gpuTimeBackground.material = null;
            gpuTimeBackground.raycastTarget = false;
            gpuTimeBackground.maskable = false;

            GameObject gpuTimeLabel = new GameObject("Label");
            RectTransform gpuTimeLabelRectTransform = gpuTimeLabel.AddComponent<RectTransform>();
            gpuTimeLabelRectTransform.SetParent(gpuTimePanelRectTransform);
            gpuTimeLabelRectTransform.anchorMin = Vector2.zero;
            gpuTimeLabelRectTransform.anchorMax = new Vector2(0.57f, 1);
            gpuTimeLabelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            gpuTimeLabelRectTransform.anchoredPosition = Vector2.zero;
            gpuTimeLabelRectTransform.anchoredPosition3D = Vector3.zero;
            gpuTimeLabelRectTransform.sizeDelta = Vector2.zero;

            gpuTimeLabel.AddComponent<CanvasRenderer>();

            Text gpuTimeLabelText = gpuTimeLabel.AddComponent<Text>();
            gpuTimeLabelText.text = "GPU Time:";
            gpuTimeLabelText.font = Font.CreateDynamicFontFromOSFont("Courier New", 16);
            gpuTimeLabelText.fontStyle = FontStyle.Normal;
            gpuTimeLabelText.fontSize = 17;
            gpuTimeLabelText.lineSpacing = 0;
            gpuTimeLabelText.supportRichText = false;
            gpuTimeLabelText.alignment = TextAnchor.MiddleCenter;
            gpuTimeLabelText.alignByGeometry = true;
            gpuTimeLabelText.horizontalOverflow = HorizontalWrapMode.Wrap;
            gpuTimeLabelText.verticalOverflow = VerticalWrapMode.Truncate;
            gpuTimeLabelText.resizeTextForBestFit = true;
            gpuTimeLabelText.resizeTextMinSize = 1;
            gpuTimeLabelText.resizeTextMaxSize = 17;
            gpuTimeLabelText.color = Color.white;
            gpuTimeLabelText.material = null;
            gpuTimeLabelText.raycastTarget = false;
            gpuTimeLabelText.maskable = false;

            gpuTimeLabel.AddComponent<Outline>();


            GameObject gpuTimeCount = new GameObject("Ms");
            RectTransform gpuTimeCountRectTransform = gpuTimeCount.AddComponent<RectTransform>();
            gpuTimeCountRectTransform.SetParent(gpuTimePanelRectTransform);
            gpuTimeCountRectTransform.SetAsLastSibling();
            gpuTimeCountRectTransform.anchorMin = new Vector2(0.57f, 0);
            gpuTimeCountRectTransform.anchorMax = Vector2.one;
            gpuTimeCountRectTransform.pivot = new Vector2(0.5f, 0.5f);
            gpuTimeCountRectTransform.anchoredPosition = Vector2.zero;
            gpuTimeCountRectTransform.anchoredPosition3D = Vector3.zero;
            gpuTimeCountRectTransform.sizeDelta = Vector2.zero;

            gpuTimeCount.AddComponent<CanvasRenderer>();

            Text gpuTimeCountText = gpuTimeCount.AddComponent<Text>();
            gpuTimeCountText.text = "---";
            gpuTimeCountText.font = Font.CreateDynamicFontFromOSFont("Courier New", 16);
            gpuTimeCountText.fontStyle = FontStyle.Normal;
            gpuTimeCountText.fontSize = 17;
            gpuTimeCountText.lineSpacing = 0;
            gpuTimeCountText.supportRichText = false;
            gpuTimeCountText.alignment = TextAnchor.MiddleCenter;
            gpuTimeCountText.alignByGeometry = true;
            gpuTimeCountText.horizontalOverflow = HorizontalWrapMode.Wrap;
            gpuTimeCountText.verticalOverflow = VerticalWrapMode.Truncate;
            gpuTimeCountText.resizeTextForBestFit = true;
            gpuTimeCountText.resizeTextMinSize = 1;
            gpuTimeCountText.resizeTextMaxSize = 17;
            gpuTimeCountText.color = new Color(200, 200, 200, 255);
            gpuTimeCountText.material = null;
            gpuTimeCountText.raycastTarget = false;
            gpuTimeCountText.maskable = false;

            gpuTimeCount.AddComponent<Outline>();

            fpsCounter = cPanel.transform;
            gpuTimeCounter = gpuTimePanel.transform;
            fpsText = fpsCountText;
            gpuTimeText = gpuTimeCountText;
        }

        private void CreateGridStats(GameObject canvas)
        {
            GameObject statsPanel = new GameObject("Grid Stats");
            RectTransform statsRectTransform = statsPanel.AddComponent<RectTransform>();
            statsRectTransform.SetParent(canvas.transform);
            statsRectTransform.anchorMin = new Vector2(1, 1);
            statsRectTransform.anchorMax = new Vector2(1, 1);
            statsRectTransform.pivot = new Vector2(0, 1);
            statsRectTransform.anchoredPosition = Vector2.zero;
            statsRectTransform.anchoredPosition3D = Vector3.zero;
            statsRectTransform.sizeDelta = Vector2.zero;

            statsPanel.AddComponent<CanvasRenderer>();

            GridLayoutGroup gridLayoutGroup = statsPanel.AddComponent<GridLayoutGroup>();
            gridLayoutGroup.padding = new RectOffset(0, 5, 0, 0);
            gridLayoutGroup.cellSize = new Vector2(175, 80);
            gridLayoutGroup.spacing = new Vector2(5, 0);
            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayoutGroup.childAlignment = TextAnchor.UpperRight;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            gridLayoutGroup.constraintCount = 1;

            GameObject controllerPanel = new GameObject("Controller");
            RectTransform controllerPanelRectTransform = controllerPanel.AddComponent<RectTransform>();
            controllerPanelRectTransform.SetParent(statsRectTransform);
            controllerPanelRectTransform.anchorMin = new Vector2(0, 1);
            controllerPanelRectTransform.anchorMax = new Vector2(0, 1);
            controllerPanelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            controllerPanelRectTransform.anchoredPosition = Vector2.zero;
            controllerPanelRectTransform.anchoredPosition3D = Vector3.zero;
            controllerPanelRectTransform.sizeDelta = new Vector2(70, 20);

            controllerPanel.AddComponent<CanvasRenderer>();

            Image controllerBackground = controllerPanel.AddComponent<Image>();
            controllerBackground.sprite = null;
            controllerBackground.material = null;
            controllerBackground.color = new Color(0, 0, 0, 150);
            controllerBackground.raycastTarget = false;
            controllerBackground.maskable = false;

            GameObject properties = new GameObject("Properties");
            RectTransform propertiesLabelRectTransform = properties.AddComponent<RectTransform>();
            propertiesLabelRectTransform.SetParent(controllerPanelRectTransform);
            propertiesLabelRectTransform.anchorMin = new Vector2(0.01f, 0.005f);
            propertiesLabelRectTransform.anchorMax = new Vector2(0.4f, 0.995f);
            propertiesLabelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            propertiesLabelRectTransform.anchoredPosition = Vector2.zero;
            propertiesLabelRectTransform.anchoredPosition3D = Vector3.zero;
            propertiesLabelRectTransform.sizeDelta = Vector2.zero;

            properties.AddComponent<CanvasRenderer>();

            Text propertiesLabelText = properties.AddComponent<Text>();
            propertiesLabelText.text = string.Empty;
            propertiesLabelText.font = Font.CreateDynamicFontFromOSFont("Courier New", 16);
            propertiesLabelText.fontStyle = FontStyle.Normal;
            propertiesLabelText.fontSize = 0;
            propertiesLabelText.lineSpacing = 1;
            propertiesLabelText.supportRichText = false;
            propertiesLabelText.alignment = TextAnchor.UpperLeft;
            propertiesLabelText.alignByGeometry = true;
            propertiesLabelText.horizontalOverflow = HorizontalWrapMode.Wrap;
            propertiesLabelText.verticalOverflow = VerticalWrapMode.Truncate;
            propertiesLabelText.resizeTextForBestFit = true;
            propertiesLabelText.resizeTextMinSize = 1;
            propertiesLabelText.resizeTextMaxSize = 10;
            propertiesLabelText.color = Color.white;
            propertiesLabelText.material = null;
            propertiesLabelText.raycastTarget = false;
            propertiesLabelText.maskable = false;

            properties.AddComponent<Outline>();


            GameObject values = new GameObject("Values");
            RectTransform valuesRectTransform = values.AddComponent<RectTransform>();
            valuesRectTransform.SetParent(controllerPanelRectTransform);
            valuesRectTransform.SetAsLastSibling();
            valuesRectTransform.anchorMin = new Vector2(0.4f, 0.005f);
            valuesRectTransform.anchorMax = new Vector2(0.99f, 0.995f);
            valuesRectTransform.pivot = new Vector2(0.5f, 0.5f);
            valuesRectTransform.anchoredPosition = Vector2.zero;
            valuesRectTransform.anchoredPosition3D = Vector3.zero;
            valuesRectTransform.sizeDelta = Vector2.zero;

            values.AddComponent<CanvasRenderer>();

            Text valuesText = values.AddComponent<Text>();
            valuesText.text = string.Empty;
            valuesText.font = Font.CreateDynamicFontFromOSFont("Courier New", 16);
            valuesText.fontStyle = FontStyle.Normal;
            valuesText.fontSize = 0;
            valuesText.lineSpacing = 1;
            valuesText.supportRichText = false;
            valuesText.alignment = TextAnchor.UpperLeft;
            valuesText.alignByGeometry = true;
            valuesText.horizontalOverflow = HorizontalWrapMode.Overflow;
            valuesText.verticalOverflow = VerticalWrapMode.Truncate;
            valuesText.resizeTextForBestFit = true;
            valuesText.resizeTextMinSize = 1;
            valuesText.resizeTextMaxSize = 10;
            valuesText.color = new Color(200, 200, 200, 255);
            valuesText.material = null;
            valuesText.raycastTarget = false;
            valuesText.maskable = false;

            values.AddComponent<Outline>();



            GameObject aiControllerPanel = new GameObject("AI Controller");
            RectTransform aiControllerPanelRectTransform = aiControllerPanel.AddComponent<RectTransform>();
            aiControllerPanelRectTransform.SetParent(statsRectTransform);
            aiControllerPanelRectTransform.anchorMin = new Vector2(0, 1);
            aiControllerPanelRectTransform.anchorMax = new Vector2(0, 1);
            aiControllerPanelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            aiControllerPanelRectTransform.anchoredPosition = Vector2.zero;
            aiControllerPanelRectTransform.anchoredPosition3D = Vector3.zero;
            aiControllerPanelRectTransform.sizeDelta = new Vector2(70, 20);

            aiControllerPanel.AddComponent<CanvasRenderer>();

            Image aiControllerBackground = aiControllerPanel.AddComponent<Image>();
            aiControllerBackground.sprite = null;
            aiControllerBackground.material = null;
            aiControllerBackground.color = new Color(0, 0, 0, 150);
            aiControllerBackground.raycastTarget = false;
            aiControllerBackground.maskable = false;

            GameObject aiProperties = new GameObject("Properties");
            RectTransform aiPropertiesLabelRectTransform = aiProperties.AddComponent<RectTransform>();
            aiPropertiesLabelRectTransform.SetParent(aiControllerPanelRectTransform);
            aiPropertiesLabelRectTransform.anchorMin = new Vector2(0.01f, 0.005f);
            aiPropertiesLabelRectTransform.anchorMax = new Vector2(0.4f, 0.995f);
            aiPropertiesLabelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            aiPropertiesLabelRectTransform.anchoredPosition = Vector2.zero;
            aiPropertiesLabelRectTransform.anchoredPosition3D = Vector3.zero;
            aiPropertiesLabelRectTransform.sizeDelta = Vector2.zero;

            aiProperties.AddComponent<CanvasRenderer>();

            Text aiPropertiesLabelText = aiProperties.AddComponent<Text>();
            aiPropertiesLabelText.text = string.Empty;
            aiPropertiesLabelText.font = Font.CreateDynamicFontFromOSFont("Courier New", 16);
            aiPropertiesLabelText.fontStyle = FontStyle.Normal;
            aiPropertiesLabelText.fontSize = 0;
            aiPropertiesLabelText.lineSpacing = 1;
            aiPropertiesLabelText.supportRichText = false;
            aiPropertiesLabelText.alignment = TextAnchor.UpperLeft;
            aiPropertiesLabelText.alignByGeometry = true;
            aiPropertiesLabelText.horizontalOverflow = HorizontalWrapMode.Wrap;
            aiPropertiesLabelText.verticalOverflow = VerticalWrapMode.Truncate;
            aiPropertiesLabelText.resizeTextForBestFit = true;
            aiPropertiesLabelText.resizeTextMinSize = 1;
            aiPropertiesLabelText.resizeTextMaxSize = 10;
            aiPropertiesLabelText.color = Color.white;
            aiPropertiesLabelText.material = null;
            aiPropertiesLabelText.raycastTarget = false;
            aiPropertiesLabelText.maskable = false;

            aiProperties.AddComponent<Outline>();


            GameObject aiValues = new GameObject("Values");
            RectTransform aiValuesRectTransform = aiValues.AddComponent<RectTransform>();
            aiValuesRectTransform.SetParent(aiControllerPanelRectTransform);
            aiValuesRectTransform.SetAsLastSibling();
            aiValuesRectTransform.anchorMin = new Vector2(0.4f, 0.005f);
            aiValuesRectTransform.anchorMax = new Vector2(0.99f, 0.995f);
            aiValuesRectTransform.pivot = new Vector2(0.5f, 0.5f);
            aiValuesRectTransform.anchoredPosition = Vector2.zero;
            aiValuesRectTransform.anchoredPosition3D = Vector3.zero;
            aiValuesRectTransform.sizeDelta = Vector2.zero;

            aiValues.AddComponent<CanvasRenderer>();

            Text aiValuesText = aiValues.AddComponent<Text>();
            aiValuesText.text = string.Empty;
            aiValuesText.font = Font.CreateDynamicFontFromOSFont("Courier New", 16);
            aiValuesText.fontStyle = FontStyle.Normal;
            aiValuesText.fontSize = 0;
            aiValuesText.lineSpacing = 1;
            aiValuesText.supportRichText = false;
            aiValuesText.alignment = TextAnchor.UpperLeft;
            aiValuesText.alignByGeometry = true;
            aiValuesText.horizontalOverflow = HorizontalWrapMode.Overflow;
            aiValuesText.verticalOverflow = VerticalWrapMode.Truncate;
            aiValuesText.resizeTextForBestFit = true;
            aiValuesText.resizeTextMinSize = 1;
            aiValuesText.resizeTextMaxSize = 10;
            aiValuesText.color = new Color(200, 200, 200, 255);
            aiValuesText.material = null;
            aiValuesText.raycastTarget = false;
            aiValuesText.maskable = false;

            aiValues.AddComponent<Outline>();

            controllerStats = controllerPanel.transform;
            controllerProperties = propertiesLabelText;
            controllerValues = valuesText;
            aiControllerStats = aiControllerPanel.transform;
            aiControllerProperties = aiPropertiesLabelText;
            aiControllerValues = aiValuesText;
        }
    }
}
