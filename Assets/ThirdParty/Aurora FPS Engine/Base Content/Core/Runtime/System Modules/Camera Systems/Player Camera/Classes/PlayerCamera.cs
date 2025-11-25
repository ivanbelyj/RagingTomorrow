/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules.CameraSystems.Effects;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using AuroraFPSRuntime.SystemModules.ControllerModules;

#region [Unity Editor Namespaces]
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
#endif
#endregion

namespace AuroraFPSRuntime.SystemModules.CameraSystems
{

    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class PlayerCamera : PawnCamera, IInputListener
    {
        [SerializeField]
        [NotNull]
        [Order(-959)]
        private new Camera camera;

        [SerializeField]
        [NotNull(Format = "Add one of implementation of player controller and attach it here.")]
        [Order(-958)]
        private PlayerController controller;

        [SerializeReference]
        [Foldout("Camera Effects", Style = "Header")]
        [ReorderableList(
            DisplayHeader = false,
            Draggable = false,
            GetElementLabelCallback = "GetCameraEffectLabelCallback",
            OnDropdownButtonCallback = "OnAddCameraEffectCallback",
            OnRemoveCallbackCallback = "OnRemoveCameraEffectCallback",
            NoneElementLabel = "Add new camera effects...")]
        [Order(-799)]
        private PlayerCameraEffect[] cameraEffects;

        [SerializeField]
        [HideExpandButton]
        [Foldout("Zoom Settings", Style = "Header")]
        [Order(-199)]
        private FieldOfViewSettings zoomSettings = new FieldOfViewSettings(-10, 0.25f, AnimationCurve.Linear(0, 0, 1, 1), true);

        [SerializeField]
        [Label("Hold Interaction")]
        [Foldout("Zoom Settings", Style = "Header")]
        [Order(-198)]
        private bool zoomHoldInteraction = true;

        [SerializeField]
        [Label("Override Sensitivity")]
        [Foldout("Zoom Settings", Style = "Header")]
        [Order(-197)]
        private bool overrideZoomSensitivity = true;

        [SerializeField]
        [Label("Sensitivity")]
        [Foldout("Zoom Settings", Style = "Header")]
        [VisibleIf("overrideZoomSensitivity")]
        [Order(-196)]
        [Indent(1)]
        private Vector2 zoomSensitivity = new Vector2(10, 10);

        [SerializeField]
        [HideExpandButton]
        [Foldout("Default FOV Settings", Style = "Header")]
        [Order(-196)]
        private FieldOfViewSettings defaultFOVSettings = new FieldOfViewSettings(85.0f, 0.25f, AnimationCurve.Linear(0, 0, 1, 1), false);

        // Stored required components.
        private CameraShaker shaker;
        
        // Stored required properties.
        private bool isZooming;
        private CoroutineObject<FieldOfViewSettings> changeFOVCoroutine;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(camera != null, $"<b><color=#FF0000>Attach reference of main camera to the {gameObject.name}<i>(gameobject)</i> -> {GetType().Name} component -> Camera<i>(field)</i>.</color></b>");
            Debug.Assert(controller != null, $"<b><color=#FF0000>Attach reference of the player controller to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Controller<i>(field)</i>.</color></b>");

            if(!camera.TryGetComponent<CameraShaker>(out shaker))
            {
                shaker = camera.gameObject.AddComponent<CameraShaker>();
            }

            changeFOVCoroutine = new CoroutineObject<FieldOfViewSettings>(this);

            for (int i = 0; i < cameraEffects.Length; i++)
            {
                cameraEffects[i].Initialization(controller, this);
            }
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            RegisterInputActions();
        }

        /// <summary>
        /// Called when reading input value to software sensitivity.
        /// </summary>
        /// <returns>Software sensitivity.</returns>
        protected override Vector2 CalculateSensitivity()
        {
            if (isZooming && overrideZoomSensitivity)
                return zoomSensitivity;
            return base.CalculateSensitivity();
        }

        /// <summary>
        /// Restore camera to default.
        /// </summary>
        public override void Restore()
        {
            base.Restore();
            isZooming = false;

            changeFOVCoroutine.Stop();
            camera.fieldOfView = defaultFOVSettings.CalculateFieldOfView(camera.fieldOfView);
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            RemoveInputActions();
        }

        /// <summary>
        /// Apply zoom field of view settings to the camera component.
        /// </summary>
        public void ZoomIn()
        {
            if (!isZooming)
            {
                OnStartZoomCallback?.Invoke();
            }
            isZooming = true;
            ChangeFieldOfView(zoomSettings, true);
        }

        /// <summary>
        /// Apply default field of view settings to the camera component.
        /// </summary>
        public void ZoomOut()
        {
            if (isZooming)
            {
                OnStopZoomCallback?.Invoke();
            }
            isZooming = false;
            ChangeFieldOfView(defaultFOVSettings, true);
        }

        /// <summary>
        /// Camera is zooming.
        /// </summary>
        public bool IsZooming()
        {
            return isZooming;
        }

        /// <summary>
        /// Change camera field of view value.
        /// </summary>
        /// <param name="settings">Settings to change current field of view.</param>
        /// <param name="force">
        /// True: If at the moment change field of view processed, terminate it, and apply new field of view settings.
        /// False: Terminate applying new field of view settings, if the other field of view settings are currently being applied.
        /// </param>
        public void ChangeFieldOfView(FieldOfViewSettings settings, bool force = false)
        {
            if (settings != null)
            {
                changeFOVCoroutine.Start(ChangeFOV, settings, force);
            }
        }

        /// <summary>
        /// Increase camera field of view to target value.
        /// </summary>
        protected IEnumerator ChangeFOV(FieldOfViewSettings settings)
        {
            float time = 0.0f;
            float speed = 1.0f / settings.GetDuration();

            float currentFOV = camera.fieldOfView;
            float targetFOV = settings.CalculateFieldOfView(camera.fieldOfView);

            while (time < 1.0f)
            {
                time += Time.deltaTime * speed;
                float smoothTime = settings.EvaluateCurve(time);
                camera.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothTime);
                OnFOVProgressCallback?.Invoke(smoothTime);
                yield return null;
            }
        }

        public virtual void RegisterInputActions()
        {
            InputReceiver.ZoomAction.performed += ZoomAction;
            InputReceiver.ZoomAction.canceled += ZoomAction;
        }

        public virtual void RemoveInputActions()
        {
            InputReceiver.ZoomAction.performed -= ZoomAction;
            InputReceiver.ZoomAction.canceled -= ZoomAction;
        }

        #region [Input Action Wrapper]
        private void ZoomAction(InputAction.CallbackContext context)
        {
            if (!isZooming && context.performed && (ZoomConditionCallback?.Invoke() ?? true))
            {
                ZoomIn();
            }
            else if (isZooming &&
                (context.performed && !zoomHoldInteraction) ||
                (context.canceled && zoomHoldInteraction))
            {
                ZoomOut();
            }
        }
        #endregion

        #region [Event Callback Function]
        /// <summary>
        /// Called when field of view changed.
        /// </summary>
        public event Action<float> OnFOVProgressCallback;

        /// <summary>
        /// Called when camera start zooming.
        /// </summary>
        public event Action OnStartZoomCallback;

        /// <summary>
        /// Called when camera stop zooming.
        /// </summary>
        public event Action OnStopZoomCallback;

        /// <summary>
        /// Called every time before start start zooming to check, can be camera start zoom.
        /// </summary>
        public event Func<bool> ZoomConditionCallback;
        #endregion

        #region [Unity Editor]
#if UNITY_EDITOR
        private string GetCameraEffectLabelCallback(SerializedProperty property, int index)
        {
            string[] baseTypeAndAssemblyName = property.managedReferenceFullTypename.Split(' ');
            Type type = Type.GetType(baseTypeAndAssemblyName[1]);
            ReferenceContent menuAttribute = type.GetCustomAttribute<ReferenceContent>();
            if (menuAttribute != null)
            {
                return menuAttribute.name;
            }
            else
            {
                return type.Name;
            }
        }

        private void OnAddCameraEffectCallback(Rect position, SerializedProperty property)
        {
            Assembly assembly = typeof(PlayerCameraEffect).Assembly;
            Type[] assemblyTypes = assembly.GetTypes();
            Type[] effectTypes = assemblyTypes.Where(t => t.IsSubclassOf(typeof(PlayerCameraEffect))).ToArray();

            GenericMenu genericMenu = new GenericMenu();
            for (int i = 0; i < effectTypes.Length; i++)
            {
                Type effect = effectTypes[i];
                if(effect == typeof(PlayerCameraEffect) || effect == typeof(PlayerCameraHingeEffect))
                {
                    continue;
                }

                ReferenceContent menuAttribute = effect.GetCustomAttribute<ReferenceContent>();
                string name = menuAttribute?.name ?? effect.Name;
                string path = menuAttribute?.path ?? effect.ToString().Replace(".", "/");
                genericMenu.AddItem(new GUIContent(path), false, () =>
                {
                    if (effect.IsSubclassOf(typeof(PlayerCameraHingeEffect)))
                    {
                        string hingeObjectName = string.Format("{0} [Effect Hinge]", name);
                        Transform hinge = CreateCameraEffectHinge(hingeObjectName);
                        if (hinge != null)
                        {
                            int index = property.arraySize;
                            property.arraySize++;
                            property.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(effect);
                            property.serializedObject.ApplyModifiedProperties();

                            SerializedProperty serializedHinge = property.GetArrayElementAtIndex(index).FindPropertyRelative("hinge");
                            if (serializedHinge != null)
                            {
                                serializedHinge.objectReferenceValue = hinge;
                                property.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    else
                    {
                        int index = property.arraySize;
                        property.arraySize++;
                        property.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(effect);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                });
            }
            genericMenu.DropDown(position);
        }

        private void OnRemoveCameraEffectCallback(SerializedProperty property, int index)
        {
            SerializedProperty serializedCameraEffect = property.GetArrayElementAtIndex(index);
            string[] baseTypeAndAssemblyName = serializedCameraEffect.managedReferenceFullTypename.Split(' ');
            Type type = Type.GetType(baseTypeAndAssemblyName[1]);
            if (type.IsSubclassOf(typeof(PlayerCameraHingeEffect)))
            {
                if (serializedCameraEffect != null)
                {
                    SerializedProperty serializedHinge = serializedCameraEffect.FindPropertyRelative("hinge");
                    if (serializedHinge != null)
                    {
                        Transform hinge = serializedHinge.objectReferenceValue as Transform;
                        if (hinge != null)
                        {
                            RemoveCameraEffectHinge(hinge);
                        }
                    }
                }
            }
            property.DeleteArrayElementAtIndex(index);
            property.serializedObject.ApplyModifiedProperties();
        }

        private static Transform CreateCameraEffectHinge(string name)
        {
            GameObject selectionObject = Selection.activeGameObject;
            if (PrefabUtility.IsPartOfAnyPrefab(selectionObject))
            {
                PrefabStage stage = PrefabStageUtility.GetPrefabStage(selectionObject);
                if (stage != null)
                {
                    selectionObject = stage.prefabContentsRoot;
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Aurora FPS Engine: Cannot restructure Prefab instance",
                        "This action need to restructure Prefab instance.\n\n" +
                        "You can open Prefab in Prefab Mode to restructure the Prefab Asset itself, or unpack the Prefab instance to remove its Prefab connection.",
                        "Open Prefab",
                        "Cancel"))
                    {
                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectionObject);
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        AssetDatabase.OpenAsset(asset);
                    }
                    return null;
                }
            }

            Transform rootHinge = null;
            for (int i = 0; i < selectionObject.transform.childCount; i++)
            {
                Transform child = selectionObject.transform.GetChild(i);
                if (child.CompareTag("RootHinge"))
                {
                    rootHinge = child.transform;
                    break;
                }
            }

            if (rootHinge == null)
            {
                EditorUtility.DisplayDialog("Aurora FPS Engine: Root hinge not found", "Not found.", "Ok");
                return null;
            }

            GameObject hingeObject = new GameObject(name);
            StageUtility.PlaceGameObjectInCurrentStage(hingeObject);

            Transform hinge = hingeObject.transform;
            hinge.SetParent(rootHinge);
            hinge.localPosition = Vector3.zero;
            hinge.localRotation = Quaternion.identity;
            hinge.SetAsFirstSibling();

            List<Transform> copyChildren = new List<Transform>(rootHinge.childCount - 1);
            for (int i = 1; i < rootHinge.childCount; i++)
            {
                copyChildren.Add(rootHinge.GetChild(i));
            }

            for (int i = 0; i < copyChildren.Count; i++)
            {
                copyChildren[i].SetParent(hinge);
            }

            return hinge;
        }

        private static void RemoveCameraEffectHinge(Transform target)
        {
            GameObject rootObject = target.root.gameObject;
            if (PrefabUtility.IsPartOfAnyPrefab(rootObject))
            {
                PrefabStage stage = PrefabStageUtility.GetPrefabStage(rootObject);
                if (stage != null)
                {
                    rootObject = stage.prefabContentsRoot;
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Aurora FPS Engine: Cannot restructure Prefab instance",
                        "This action need to restructure Prefab instance.\n\n" +
                        "You can open Prefab in Prefab Mode to restructure the Prefab Asset itself, or unpack the Prefab instance to remove its Prefab connection.",
                        "Open Prefab",
                        "Cancel"))
                    {
                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(rootObject);
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        AssetDatabase.OpenAsset(asset);
                    }
                    return;
                }
            }

            Transform parent = target.parent;
            List<Transform> copyChildren = new List<Transform>(target.childCount);
            for (int i = 0; i < target.childCount; i++)
            {
                Transform child = target.GetChild(i);
                copyChildren.Add(child);
            }

            for (int i = 0; i < copyChildren.Count; i++)
            {
                Transform child = copyChildren[i];
                child.SetParent(parent);
            }

            DestroyImmediate(target.gameObject);
        }
#endif
        #endregion

        #region [Getter / Setter]
        public Camera GetCamera()
        {
            return camera;
        }

        public void SetCamera(Camera camera)
        {
            this.camera = camera;
        }

        public FieldOfViewSettings GetZoomFOVSettings()
        {
            return zoomSettings;
        }

        public void SetZoomFOVSettings(FieldOfViewSettings fieldOfViewSettings)
        {
            zoomSettings = fieldOfViewSettings;
        }

        public FieldOfViewSettings GetDefaultFOVSettings()
        {
            return defaultFOVSettings;
        }

        public void SetDefaultFOVSettings(FieldOfViewSettings fieldOfViewSettings)
        {
            defaultFOVSettings = fieldOfViewSettings;
        }

        public bool ZoomHoldInteraction()
        {
            return zoomHoldInteraction;
        }

        public void ZoomHoldInteraction(bool value)
        {
            zoomHoldInteraction = value;
        }

        public bool GetOverrideZoomSensitivity()
        {
            return overrideZoomSensitivity;
        }

        public void SetOverrideZoomSensitivity(bool value)
        {
            overrideZoomSensitivity = value;
        }

        public Vector2 GetZoomSensitivity()
        {
            return zoomSensitivity;
        }

        public void SetZoomSensitivity(Vector2 value)
        {
            zoomSensitivity = value;
        }

        public PlayerController GetPlayerController()
        {
            return controller;
        }

        public CameraShaker GetShaker()
        {
            return shaker;
        }
        #endregion
    }
}