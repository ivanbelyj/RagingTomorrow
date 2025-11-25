/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright ? 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.CoreModules.Pattern;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.CoreModules.CommandLine
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Core Modules/Developer Tools/Console")]
    [DisallowMultipleComponent]
    public sealed class Console : Singleton<Console>
    {

        public static class DefaultInputs
        {
            public static InputAction OpenSmall
            {
                get
                {
                    return new InputAction("Open Small", InputActionType.Button, "<Keyboard>/backquote", null, null, null);
                }
            }

            public static InputAction OpenFull
            {
                get
                {
                    InputAction inputAction = new InputAction("Open Small", InputActionType.Button, null, null, null, null);
                    inputAction.AddCompositeBinding("ButtonWithOneModifier")
                        .With("Modifier", "<Keyboard>/leftShift")
                        .With("Button", "<Keyboard>/backquote");
                    return inputAction;
                }
            }

            public static InputAction Close
            {
                get
                {
                    return new InputAction("Close", InputActionType.Button, "<Keyboard>/escape", null, null, null);
                }
            }

            public static InputAction Apply
            {
                get
                {
                    return new InputAction("Apply", InputActionType.Button, "<Keyboard>/enter", null, null, null);
                }
            }

            public static InputAction AutoComplete
            {
                get
                {
                    return new InputAction("Auto Complete", InputActionType.Button, "<Keyboard>/Tab", null, null, null);
                }
            }

            public static InputAction NextCommand
            {
                get
                {
                    return new InputAction("Next Command", InputActionType.Button, "<Keyboard>/downArrow", null, null, null);
                }
            }

            public static InputAction PreviousCommand
            {
                get
                {
                    return new InputAction("Previous Command", InputActionType.Button, "<Keyboard>/upArrow", null, null, null);
                }
            }
        }
        internal static class ContentProperties
        {
            public static GUIStyle GetWindowStyle(Texture2D texture, Font font, int fontSize, Color color)
            {
                GUIStyle style = new GUIStyle();
                style = new GUIStyle();
                style.normal.background = texture;
                style.padding = new RectOffset(4, 4, 4, 4);
                style.normal.textColor = color;
                style.font = font;
                style.fontSize = fontSize;
                return style;
            }

            public static GUIStyle GetLabelStyle(Font font, int fontSize, Color color)
            {
                GUIStyle style = new GUIStyle();
                style = new GUIStyle();
                style.font = font;
                style.fontSize = fontSize;
                style.normal.textColor = color;
                style.wordWrap = true;
                return style;
            }

            public static GUIStyle GetInputStyle(Texture2D texture, Font font, int fontSize, Color color)
            {
                GUIStyle style = new GUIStyle();
                style.padding = new RectOffset(4, 4, 4, 4);
                style.fontSize = fontSize;
                style.font = font;
                style.fixedHeight = fontSize * 1.6f;
                style.normal.textColor = color;
                style.normal.background = texture;
                return style;
            }

            public static Texture2D GetWindowTexture(Color color)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, color);
                texture.Apply();
                return texture;
            }

            public static Texture2D GetInputFieldTexture(Color windowColor, float contrast, float alpha)
            {
                Texture2D texture = new Texture2D(1, 1);
                Color color = new Color(windowColor.r - contrast, windowColor.g - contrast, windowColor.b - contrast, alpha);
                texture.SetPixel(0, 0, color);
                texture.Apply();
                return texture;
            }
        }

        public static readonly ConsoleLog Buffer = new ConsoleLog(512);
        public static readonly CommandShell Shell = new CommandShell();
        public static readonly CommandHistory History = new CommandHistory();
        public static readonly CommandAutoComplete Autocomplete = new CommandAutoComplete();

        public enum State
        {
            Close,
            OpenSmall,
            OpenFull
        }

        [Serializable]
        public class OnSwitchEvent : UnityEvent<State> { }

        #region [UI Settings Fields]
        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        private Font consoleFont;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        private bool autoResizeFont = true;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        [VisibleIf("autoResizeFont", false)]
        private int fontSize = 15;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        [VisibleIf("autoResizeFont", true)]
        private int fontSizeRatio = 39;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        private string inputCaret = ">";

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        private bool showGUIButtons = false;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        private bool rightAlignButtons = false;

        [SerializeField]
        [Slider(0, 1)]
        [TabGroup("Main", "UI Settings")]
        private float inputContrast = 0.0f;

        [SerializeField]
        [Slider(0, 1)]
        [TabGroup("Main", "UI Settings")]
        private float inputAlpha = 0.5f;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        [Foldout("Colors", Style = "Header")]
        private Color windowColor = Color.black;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        [Foldout("Colors", Style = "Header")]
        private Color foregroundColor = Color.white;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        [Foldout("Colors", Style = "Header")]
        private Color shellColor = Color.white;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        [Foldout("Colors", Style = "Header")]
        private Color inputColor = Color.cyan;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        [Foldout("Colors", Style = "Header")]
        private Color warningColor = Color.yellow;

        [SerializeField]
        [TabGroup("Main", "UI Settings")]
        [Foldout("Colors", Style = "Header")]
        private Color errorColor = Color.red;
        #endregion

        #region [Input Settings Fields]
        [SerializeField]
        [TabGroup("Main", "Input Settings")]
        private InputAction openSmallInput = DefaultInputs.OpenSmall;

        [SerializeField]
        [TabGroup("Main", "Input Settings")]
        private InputAction openFullInput = DefaultInputs.OpenFull;

        [SerializeField]
        [TabGroup("Main", "Input Settings")]
        private InputAction closeInput = DefaultInputs.Close;

        [SerializeField]
        [TabGroup("Main", "Input Settings")]
        private InputAction applyInput = DefaultInputs.Apply;

        [SerializeField]
        [TabGroup("Main", "Input Settings")]
        private InputAction autoCompleteInput = DefaultInputs.AutoComplete;

        [SerializeField]
        [TabGroup("Main", "Input Settings")]
        private InputAction nextCommandInput = DefaultInputs.NextCommand;

        [SerializeField]
        [TabGroup("Main", "Input Settings")]
        private InputAction previousCommandInput = DefaultInputs.PreviousCommand;
        #endregion

        #region [Advanced Settings Fields]
        [SerializeField]
        [TabGroup("Main", "Advanced Settings")]
        private int bufferSize = 512;

        [SerializeField]
        [Slider(0, 1)]
        [TabGroup("Main", "Advanced Settings")]
        private float maxHeight = 0.7f;

        [SerializeField]
        [Slider(0, 1)]
        [TabGroup("Main", "Advanced Settings")]
        [Indent(1)]
        private float smallRatio = 0.33f;

        [SerializeField]
        [TabGroup("Main", "Advanced Settings")]
        private bool hardwareCursor = true;

        [SerializeField]
        [TabGroup("Main", "Advanced Settings")]
        [Tooltip("Disable current EventSystem instance for blocking receive.")]
        private bool lockGameUI = true;

        [SerializeField]
        [TabGroup("Main", "Advanced Settings")]
        private bool crossfade = true;

        [SerializeField]
        [Label("Duration")]
        [MinValue(0.01f)]
        [TabGroup("Main", "Advanced Settings")]
        [VisibleIf("crossfade")]
        [Indent(1)]
        private float crossfadeDuration = 0.25f;

        [SerializeField]
        [Label("Curve")]
        [TabGroup("Main", "Advanced Settings")]
        [VisibleIf("crossfade")]
        [Indent(1)]
        private AnimationCurve crossfadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        [TabGroup("Main", "Advanced Settings")]
        private bool freezeTime = true;

        [SerializeField]
        [Label("Scale")]
        [Slider(0.0f, 0.99f)]
        [TabGroup("Main", "Advanced Settings")]
        [VisibleIf("freezeTime")]
        [Indent(1)]
        private float freezeScale = 0.0f;

        [SerializeField]
        [TabGroup("Main", "Advanced Settings")]
        [Foldout("Lock Map Names", Style = "Header")]
        [ReorderableList(ElementLabel = null,DisplayHeader = false)]
        private string[] lockMapNames = new string[2] { "Player", "UI" };

        [SerializeField]
        [TabGroup("Main", "Advanced Settings")]
        [Foldout("Event Callbacks", Style = "Header")]
        private OnSwitchEvent onSwitchEvent;
        #endregion

        // Stored required components.
        private EventSystem eventSystem;

        // Stored required properties.
        private State state;
        private bool isFocused;
        private float previousTimeScale;
        private string command;
        private Rect window;
        private Vector2 scrollPosition;
        private Vector2 screenResolution;
        private GUIStyle windowStyle;
        private GUIStyle labelStyle;
        private GUIStyle inputStyle;
        private Texture2D windowTexture;
        private Texture2D inputFieldTexture;
        private CoroutineObject<float> coroutineObject;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            command = string.Empty;
            coroutineObject = new CoroutineObject<float>(this);

            if (consoleFont == null)
            {
                consoleFont = Font.CreateDynamicFontFromOSFont("Courier New", fontSize);
            }

            Buffer.SetMaxItems(bufferSize);
            Shell.RegisterCommands();
            foreach (var command in Shell.GetCommands())
            {
                Autocomplete.Register(command.Key);
            }

            InitializeStyles();

            // Register input actions.
            openSmallInput.performed += OpenSmallAction;
            openFullInput.performed += OpenFullAction;
            closeInput.performed += CloseAction;
            applyInput.performed += ApplyAction;
            autoCompleteInput.performed += AutoCompleteAction;
            nextCommandInput.performed += NextCompleteAction;
            previousCommandInput.performed += PreviousCompleteAction;

            // Register unity event actions.
            OnSwitchStateCallback += onSwitchEvent.Invoke;
        }

        /// <summary>
        /// Initialize GUI styles of console.
        /// </summary>
        private void InitializeStyles()
        {
            windowTexture = ContentProperties.GetWindowTexture(windowColor);
            inputFieldTexture = ContentProperties.GetInputFieldTexture(windowColor, inputContrast, inputAlpha);
            windowStyle = ContentProperties.GetWindowStyle(windowTexture, consoleFont, fontSize, foregroundColor);
            inputStyle = ContentProperties.GetInputStyle(inputFieldTexture, consoleFont, fontSize, inputColor);
            labelStyle = ContentProperties.GetLabelStyle(consoleFont, fontSize, foregroundColor);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            screenResolution = new Vector2(Screen.width, Screen.height);

            if (autoResizeFont)
            {
                fontSize = Mathf.Min(Screen.width, Screen.height) / fontSizeRatio;
            }

            openSmallInput.Enable();
            openFullInput.Enable();
            closeInput.Enable();

            // Hook Unity log events
            Application.logMessageReceivedThreaded += HandleUnityLog;
        }

        /// <summary>
        /// Called on the frame when a script is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            window = new Rect(0, 0, Screen.width, 0);

            if (Shell.GetIssuedErrorMessage() != null)
            {
                Log(ConsoleLog.LogType.Error, "Error: {0}", Shell.GetIssuedErrorMessage());
            }
        }

        /// <summary>
        /// Called for rendering and handling GUI events.
        /// </summary>
        private void OnGUI()
        {
            if (screenResolution.x != Screen.width || screenResolution.y != Screen.height)
            {
                window.width = Screen.width;
                if (autoResizeFont)
                {
                    fontSize = Mathf.Min(Screen.width, Screen.height) / fontSizeRatio;
                    screenResolution.x = Screen.width;
                    screenResolution.y = Screen.height;
                    inputStyle.fontSize = fontSize;
                    inputStyle.fixedHeight = fontSize * 1.6f;
                    labelStyle.fontSize = fontSize;
                    windowStyle.fontSize = fontSize;
                }
            }

            if (showGUIButtons)
            {
                DrawGUIButtons();
            }

            if (IsClosed())
            {
                return;
            }

            window = GUILayout.Window(88, window, DrawConsole, GUIContent.none, windowStyle);
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            SwitchState(State.Close);
            openSmallInput.Disable();
            openSmallInput.Disable();
            openSmallInput.Disable();
            Application.logMessageReceivedThreaded -= HandleUnityLog;
        }

        /// <summary>
        /// Switch console state.
        /// </summary>
        /// <param name="state">New state.</param>
        public void SwitchState(State state)
        {
            float windowHeight = 0.0f;
            switch (state)
            {
                case State.Close:
                    OnConsoleClose();
                    windowHeight = 0;
                    break;
                case State.OpenSmall:
                    OnConsoleOpen();
                    windowHeight = Screen.height * maxHeight * smallRatio;
                    break;
                default:
                case State.OpenFull:
                    OnConsoleOpen();
                    windowHeight = Screen.height * maxHeight;
                    break;
            }

            if (crossfade)
                coroutineObject.Start(CrossfadeAnimation, windowHeight, true);
            else
                window.height = windowHeight;

            this.state = state;
            OnSwitchStateCallback?.Invoke(this.state);
        }

        /// <summary>
        /// Called when console being opened.
        /// </summary>
        private void OnConsoleOpen()
        {
            if (state == State.Close)
            {
                isFocused = true;
                scrollPosition.y = int.MaxValue;

                InputReceiver.EnableMapPredicate += InputPredicate;
                for (int i = 0; i < lockMapNames.Length; i++)
                {
                    InputReceiver.DisableMap(lockMapNames[i]);
                }

                // Freezing time.
                if (freezeTime)
                {
                    previousTimeScale = Time.timeScale;
                    Time.timeScale = freezeScale;
                }

                if (lockGameUI)
                {
                    eventSystem = EventSystem.current;
                    if(eventSystem != null)
                    {
                        eventSystem.enabled = false;
                    }
                }

                applyInput.Enable();
                autoCompleteInput.Enable();
                nextCommandInput.Enable();
                previousCommandInput.Enable();
            }
        }

        /// <summary>
        /// Called when console being closed.
        /// </summary>
        private void OnConsoleClose()
        {
            if(state != State.Close)
            {
                command = string.Empty;

                // Unlocking specified input maps
                InputReceiver.EnableMapPredicate -= InputPredicate;
                for (int i = 0; i < lockMapNames.Length; i++)
                {
                    InputReceiver.EnableMap(lockMapNames[i]);
                }

                // Unfreezing time.
                if (freezeTime)
                {
                    Time.timeScale = previousTimeScale;
                }

                if (lockGameUI && eventSystem != null)
                {
                    eventSystem.enabled = true;
                }

                // 
                applyInput.Disable();
                autoCompleteInput.Disable();
                nextCommandInput.Disable();
                previousCommandInput.Disable();
            }
        }

        /// <summary>
        /// Toggle console state.
        /// </summary>
        /// <param name="state">New state.</param>
        public void ToggleState(State state)
        {
            if (this.state == state)
            {
                SwitchState(State.Close);
            }
            else
            {
                SwitchState(state);
            }
        }

        /// <summary>
        /// Console is completely closed.
        /// </summary>
        public bool IsClosed()
        {
            return state == State.Close && window.height == 0.0f;
        }

        /// <summary>
        /// Called for rendering and handling GUI events.
        /// </summary>
        private void DrawConsole(int Window2D)
        {
            GUILayout.BeginVertical();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUIStyle.none, GUIStyle.none);
            GUILayout.FlexibleSpace();
            DrawLogs();
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();

            if (inputCaret != "")
            {
                GUILayout.Label(inputCaret, inputStyle, GUILayout.Width(fontSize));
            }

            GUI.SetNextControlName("command_text_field");
            command = GUILayout.TextField(command, inputStyle);

            if (isFocused)
            {
                GUI.FocusControl("command_text_field");
                isFocused = false;
            }

            if (showGUIButtons && GUILayout.Button("| run", inputStyle, GUILayout.Width(Screen.width / 10)))
            {
                EnterCommand();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Iterating buffer logs and draw it on console.
        /// </summary>
        private void DrawLogs()
        {
            for (int i = 0; i < Buffer.Logs.Count; i++)
            {
                ConsoleLog.LogItem logItem = Buffer.Logs[i];
                switch (logItem.type)
                {
                    case ConsoleLog.LogType.Message:
                        labelStyle.normal.textColor = foregroundColor;
                        break;
                    case ConsoleLog.LogType.Warning:
                        labelStyle.normal.textColor = warningColor;
                        break;
                    case ConsoleLog.LogType.Input:
                        labelStyle.normal.textColor = inputColor;
                        break;
                    case ConsoleLog.LogType.ShellMessage:
                        labelStyle.normal.textColor = shellColor;
                        break;
                    default:
                        labelStyle.normal.textColor = errorColor;
                        break;
                }
                GUILayout.Label(logItem.message, labelStyle);
            }
        }

        /// <summary>
        /// Draw open console buttons.
        /// </summary>
        private void DrawGUIButtons()
        {
            int size = fontSize;
            float x_position = rightAlignButtons ? Screen.width - 7 * size : 0;

            // 7 is the number of chars in the button plus some padding, 2 is the line height.
            // The layout will resize according to the font size.
            GUILayout.BeginArea(new Rect(x_position, window.height, 7 * size, size * 2));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Small", windowStyle))
            {
                ToggleState(State.OpenSmall);
            }
            else if (GUILayout.Button("Full", windowStyle))
            {
                ToggleState(State.OpenFull);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Enter and execute command.
        /// </summary>
        private void EnterCommand()
        {
            Log(ConsoleLog.LogType.Input, "{0} {1}", inputCaret, command);
            Shell.RunCommand(command);
            History.Push(command);

            if (Shell.GetIssuedErrorMessage() != null)
            {
                Log(ConsoleLog.LogType.Error, "Error: {0}", Shell.GetIssuedErrorMessage());
            }

            command = string.Empty;
            scrollPosition.y = int.MaxValue;
        }

        /// <summary>
        /// Autocomplete current comand text to nearest command.
        /// </summary>
        private void AutoCompleteCommand()
        {
            string headText = command;
            int formatWidth = 0;

            string[] completionBuffer = Autocomplete.Complete(ref headText, ref formatWidth);
            int completionLength = completionBuffer.Length;

            if (completionLength != 0)
            {
                command = headText;
            }

            if (completionLength > 1)
            {
                // Print possible completions
                var logBuffer = new StringBuilder();

                foreach (string completion in completionBuffer)
                {
                    logBuffer.Append(completion.PadRight(formatWidth + 4));
                }

                Log("{0}", logBuffer);
                scrollPosition.y = int.MaxValue;
            }
        }

        private IEnumerator CrossfadeAnimation(float height)
        {
            float time = 0.0f;
            float speed = 1 / crossfadeDuration;
            float desiredHeight = 0.0f;

            if(height > 0.0f)
            {
                float difference = Mathf.Abs(window.height - height);
                window.y = -difference;
                window.height = height;
            }
            else
            {
                desiredHeight = -window.height;
            }

            while (time < 1.0f)
            {
                time += Time.unscaledDeltaTime * speed;
                float smoothTime = crossfadeCurve.Evaluate(time);
                window.y = Mathf.Lerp(window.y, desiredHeight, smoothTime);
                yield return null;
            }

            if(height == 0.0f)
            {
                window.height = 0.0f;
            }
        }

        #region [Static Methods]
        public static void Log(string format, params object[] message)
        {
            Log(ConsoleLog.LogType.ShellMessage, format, message);
        }

        public static void Log(ConsoleLog.LogType type, string format, params object[] message)
        {
            Buffer.HandleLog(string.Format(format, message), type);
        }
        #endregion

        #region [Action Wrappers]
        private void OpenSmallAction(InputAction.CallbackContext context)
        {
            if (state == State.Close)
            {
                SwitchState(State.OpenSmall);
            }
        }

        private void OpenFullAction(InputAction.CallbackContext context)
        {
            SwitchState(State.OpenFull);
        }

        private void CloseAction(InputAction.CallbackContext context)
        {
            SwitchState(State.Close);
        }

        private void ApplyAction(InputAction.CallbackContext context)
        {
            EnterCommand();
        }

        private void AutoCompleteAction(InputAction.CallbackContext context)
        {
            AutoCompleteCommand();
        }

        private void NextCompleteAction(InputAction.CallbackContext context)
        {
            command = History.Next();
        }

        private void PreviousCompleteAction(InputAction.CallbackContext context)
        {
            command = History.Previous();
        }

        private void HandleUnityLog(string message, string stack_trace, LogType type)
        {
            Buffer.HandleLog(message, stack_trace, (ConsoleLog.LogType)type);
            scrollPosition.y = int.MaxValue;
        }
        #endregion

        #region [Event Callback Function]
        /// <summary>
        /// Called when console state was changed.
        /// </summary>
        public event Action<State> OnSwitchStateCallback;
        #endregion

        #region [Event Predicate]
        private bool InputPredicate(string name)
        {
            if (lockMapNames != null)
            {
                for (int i = 0; i < lockMapNames.Length; i++)
                {
                    if (name == lockMapNames[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region [Editor Section]
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Aurora FPS Engine/Create/Tools/Console", false, 121)]
        [UnityEditor.MenuItem("GameObject/Aurora FPS Engine/Tools/Console", false, 20)]
        private static void CreateConsole()
        {
            Console console = GetRuntimeInstance();
            UnityEditor.EditorGUIUtility.PingObject(console);
            UnityEditor.Selection.activeObject = console;
        }
#endif
        #endregion

        #region [Getter / Setter]
        public Font GetConsoleFont()
        {
            return consoleFont;
        }

        public void SetConsoleFont(Font value)
        {
            consoleFont = value;
        }

        public bool AutoResizeFont()
        {
            return autoResizeFont;
        }

        public void AutoResizeFont(bool value)
        {
            autoResizeFont = value;
        }

        public int GetFontSize()
        {
            return fontSize;
        }

        public void SetFontSize(int value)
        {
            fontSize = value;
        }

        public int GetFontSizeRatio()
        {
            return fontSizeRatio;
        }

        public void SetFontSizeRatio(int value)
        {
            fontSizeRatio = value;
        }

        public string InputCaret()
        {
            return inputCaret;
        }

        public void InputCaret(string value)
        {
            inputCaret = value;
        }

        public bool ShowGUIButtons()
        {
            return showGUIButtons;
        }

        public void ShowGUIButtons(bool value)
        {
            showGUIButtons = value;
        }

        public bool RightAlignButtons()
        {
            return rightAlignButtons;
        }

        public void RightAlignButtons(bool value)
        {
            rightAlignButtons = value;
        }

        public float GetInputContrast()
        {
            return inputContrast;
        }

        public void SetInputContrast(float value)
        {
            inputContrast = value;
        }

        public float GetInputAlpha()
        {
            return inputAlpha;
        }

        public void SetInputAlpha(float value)
        {
            inputAlpha = value;
        }

        public Color GetWindowColor()
        {
            return windowColor;
        }

        public void SetWindowColor(Color value)
        {
            windowColor = value;
        }

        public Color GetForegroundColor()
        {
            return foregroundColor;
        }

        public void SetForegroundColor(Color value)
        {
            foregroundColor = value;
        }

        public Color GetShellColor()
        {
            return shellColor;
        }

        public void SetShellColor(Color value)
        {
            shellColor = value;
        }

        public Color GetInputColor()
        {
            return inputColor;
        }

        public void SetInputColor(Color value)
        {
            inputColor = value;
        }

        public Color GetWarningColor()
        {
            return warningColor;
        }

        public void SetWarningColor(Color value)
        {
            warningColor = value;
        }

        public Color GetErrorColor()
        {
            return errorColor;
        }

        public void SetErrorColor(Color value)
        {
            errorColor = value;
        }

        public int GetBufferSize()
        {
            return bufferSize;
        }

        public void SetBufferSize(int value)
        {
            bufferSize = value;
            Buffer.SetMaxItems(bufferSize);
        }

        public float GetMaxHeight()
        {
            return maxHeight;
        }

        public void SetMaxHeight(float value)
        {
            maxHeight = value;
        }

        public float GetSmallRatio()
        {
            return smallRatio;
        }

        public void SetSmallRatio(float value)
        {
            smallRatio = value;
        }

        public bool HardwareCursor()
        {
            return hardwareCursor;
        }

        public void HardwareCursor(bool value)
        {
            hardwareCursor = value;
        }

        public bool LockGameUI()
        {
            return lockGameUI;
        }

        public void LockGameUI(bool value)
        {
            lockGameUI = value;
        }

        public bool Crossfade()
        {
            return crossfade;
        }

        public void Crossfade(bool value)
        {
            crossfade = value;
        }

        public float GetCrossfadeDuration()
        {
            return crossfadeDuration;
        }

        public void SetCrossfadeDuration(float value)
        {
            crossfadeDuration = value;
        }

        public AnimationCurve GetCrossfadeCurve()
        {
            return crossfadeCurve;
        }

        public void SetCrossfadeCurve(AnimationCurve value)
        {
            crossfadeCurve = value;
        }

        public bool FreezeTime()
        {
            return freezeTime;
        }

        public void FreezeTime(bool value)
        {
            freezeTime = value;
        }

        public float GetFreezeScale()
        {
            return freezeScale;
        }

        public void SetFreezeScale(float value)
        {
            freezeScale = value;
        }

        public string[] GetLockMapNames()
        {
            return lockMapNames;
        }

        public void SetLockMapNames(string[] value)
        {
            lockMapNames = value;
        }
        #endregion
    }
}
