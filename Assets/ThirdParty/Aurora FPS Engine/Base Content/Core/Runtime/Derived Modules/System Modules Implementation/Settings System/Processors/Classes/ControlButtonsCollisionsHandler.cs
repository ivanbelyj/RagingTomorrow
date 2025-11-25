/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Control Buttons Settings Processor/Control Buttons Collisions Handler")]
    [DisallowMultipleComponent]
    public sealed class ControlButtonsCollisionsHandler : Singleton<ControlButtonsCollisionsHandler>
    {
        [Serializable]
        public struct IgnoreAction
        {
            [SerializeField]
            [NotEmpty]
            private string a;

            [SerializeField]
            [NotEmpty]
            private string b;

            public IgnoreAction(string a, string b)
            {
                this.a = a;
                this.b = b;
            }

            #region [Getter / Setter]
            public string GetA()
            {
                return a;
            }

            public void SetA(string value)
            {
                a = value;
            }

            public string GetB()
            {
                return b;
            }

            public void SetB(string value)
            {
                b = value;
            }
            #endregion
        }

        public enum CollisionSolving
        {
            Highlight,
            Swap
        }

        [SerializeField]
        [NotNull]
        private InputActionAsset inputActions;

        [SerializeField]
        [NotNull]
        private string mapName;

        [SerializeField]
        private CollisionSolving collisionSolving;

        [SerializeField]
        [Label("Default")]
        [VisibleIf("collisionSolving", "Highlight")]
        [Indent]
        private Color defaultColor;

        [SerializeField]
        [Label("Highlight")]
        [VisibleIf("collisionSolving", "Highlight")]
        [Indent]
        private Color highlightColor;

        [SerializeField]
        private bool compareInteractions = true;

        [SerializeField]
        [ReorderableList(OnElementGUICallback = "OnIgnoreActionGUI")]
        private IgnoreAction[] ignoreActions;

        private InputActionMap map;
        private int processorCount;
        private int loadedProcessorCount;
        private List<InputBindingField[]> collisions = new List<InputBindingField[]>();

        /// <summary>
        /// Called when the scrip instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            map = inputActions.FindActionMap(mapName);
#if UNITY_2022_2_OR_NEWER
            processorCount = FindObjectsByType<ControlButtonsSettingsProcessor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
#else
            processorCount = FindObjectsOfType<ControlButtonsSettingsProcessor>().Length;
#endif
            InputBindingsStore.GetStore().Clear();
            loadedProcessorCount = 0;
            OnAllControlsLoaded += CheckCollisions;
            StartCoroutine("WaitProcessorsLoading");
        }

        /// <summary>
        /// Wait until all control buttons settings are loaded.
        /// </summary>
        private IEnumerator WaitProcessorsLoading()
        {
            yield return new WaitUntil(() => loadedProcessorCount == processorCount);
            OnAllControlsLoaded.Invoke();
        }

        /// <summary>
        /// Check if there are collisions and solve them if needed.
        /// </summary>
        private void CheckCollisions()
        {
            if (collisionSolving == CollisionSolving.Highlight)
            {
                List<InputBindingField> stash = new List<InputBindingField>(InputBindingsStore.GetStore());
                while (stash.Count > 0)
                {
                    InputBindingField bindingField = stash[0];
                    if (FindCollision(bindingField, out InputBindingField collised))
                    {
                        HighlightCollision(bindingField, collised);
                        stash.Remove(collised);
                    }
                    stash.Remove(bindingField);
                }
            }
        }

        /// <summary>
        /// Check if there are collisions and solve them if needed.
        /// </summary>
        /// <param name="operation">Rebinding operation.</param>
        /// <param name="binding">Changed binding.</param>
        /// <param name="oldKey">Binding old control key.</param>
        public void CheckRebindCollisions(InputActionRebindingExtensions.RebindingOperation operation,
            InputBinding binding, string oldKey)
        {
            InputBindingField bindingField = InputBindingsStore.GetBindingFieldPair(binding);
            if(FindCollision(bindingField, out InputBindingField collised))
            {
                switch (collisionSolving)
                {
                    case CollisionSolving.Highlight:
                        HighlightCollision(bindingField, collised);
                        break;
                    case CollisionSolving.Swap:
                        Bind(collised, oldKey);
                        break;
                }
            }
            CheckSolvedCollisions();
            operation.Dispose();
        }


        /// <summary>
        /// Find collised binding.
        /// </summary>
        /// <param name="changedBindingField">Changed binding field.</param>
        /// <param name="collided">Collised binding.</param>
        /// <returns>Whether or not collision was finded.</returns>
        private bool FindCollision(InputBindingField changedBindingField, out InputBindingField collided)
        {
            List<InputBindingField> bindingFields = InputBindingsStore.GetStore();
            foreach (InputBindingField bindingField in bindingFields)
            {
                if (IsCollided(changedBindingField, bindingField))
                {
                    collided = bindingField;
                    return true;
                }
            }
            collided = default(InputBindingField);
            return false;
        }

        /// <summary>
        /// Check if two bindings are collided.
        /// </summary>
        /// <param name="bindingField1">Binding field 1.</param>
        /// <param name="bindingField2">Binding field 2.</param>
        /// <returns>The result of checking</returns>
        private bool IsCollided(InputBindingField bindingField1, InputBindingField bindingField2)
        {
            if(ignoreActions != null && ignoreActions.Length > 0)
            {
                for (int i = 0; i < ignoreActions.Length; i++)
                {
                    IgnoreAction ignoreAction = ignoreActions[i];
                    if((ignoreAction.GetA() == bindingField1.GetBinding().action && 
                        ignoreAction.GetB() == bindingField2.GetBinding().action) ||
                        (ignoreAction.GetA() == bindingField2.GetBinding().action &&
                        ignoreAction.GetB() == bindingField1.GetBinding().action))
                    {
                        return false;
                    }
                }
            }

            bool isInteractionsEqual = false;
            if (compareInteractions)
            {
                InputAction action1 = inputActions.FindAction(string.Format("{0}/{1}", mapName, bindingField1.GetBinding().action));
                InputAction action2 = inputActions.FindAction(string.Format("{0}/{1}", mapName, bindingField2.GetBinding().action));
                isInteractionsEqual = action1.interactions == action2.interactions;
            }

            return (!compareInteractions || (compareInteractions && isInteractionsEqual)) &&
                !bindingField1.Equals(bindingField2) &&
                (bindingField1.GetBinding().effectivePath == bindingField2.GetBinding().effectivePath);
        }

        /// <summary>
        /// Highlight collision.
        /// </summary>
        /// <param name="changedBindingField">Changed binding field.</param>
        /// <param name="collisedBindingField">Collised binding field.</param>
        private void HighlightCollision(InputBindingField changedBindingField,
            InputBindingField collisedBindingField)
        {
            Highlight(changedBindingField);
            Highlight(collisedBindingField);
            InputBindingField[] collision = { changedBindingField, collisedBindingField };
            collisions.Add(collision);
        }

        /// <summary>
        /// Highlight bindings.
        /// </summary>
        /// <param name="changedBinding">Binding to highlight.</param>
        private void Highlight(InputBindingField bindingField)
        {
            Text inputField = bindingField.GetInputField();
            inputField.color = highlightColor;
        }

        /// <summary>
        /// Remove highlight from binding.
        /// </summary>
        /// <param name="binding">Binding to dehighlight.</param>
        private void RemoveHighlight(InputBindingField bindingField)
        {
            Text inputField = bindingField.GetInputField();
            inputField.color = defaultColor;
        }

        /// <summary>
        /// Bind new key.
        /// </summary>
        /// <param name="bindingField">Input binding field.</param>
        /// <param name="key">Key to bind.</param>
        private void Bind(InputBindingField bindingField, string key)
        {
            InputBinding binding = bindingField.GetBinding();
            InputAction action = map.FindAction(binding.action);
            int bindingIndex = action.GetBindingIndex(binding);
            string group = binding.effectivePath.Split('/')[0];
            string newPath = string.Format("{0}/{1}", group, key);
            action.ApplyBindingOverride(bindingIndex, newPath);
            InputBindingsStore.ReplaceBinding(binding, action.bindings[bindingIndex]);
            Text inputField = bindingField.GetInputField();
            inputField.text = key;
        }

        /// <summary>
        /// Check if there are old collisions that have been solved.
        /// </summary>
        private void CheckSolvedCollisions()
        {
            List<InputBindingField[]> solved = new List<InputBindingField[]>();
            foreach (InputBindingField[] collision in collisions)
            {
                if (!IsCollided(collision[0], collision[1]))
                {
                    RemoveHighlight(collision[0]);
                    RemoveHighlight(collision[1]);
                    solved.Add(collision);
                }
            }
            foreach(InputBindingField[] collision in solved)
            {
                collisions.Remove(collision);
            }
        }

        #region [Editor Section]
#if UNITY_EDITOR
        private void OnIgnoreActionGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            Rect leftPosition = new Rect(position.x, position.y, (position.width / 2) - 1, position.height);
            UnityEditor.EditorGUI.PropertyField(leftPosition, property.FindPropertyRelative("a"), GUIContent.none);

            Rect rightPosition = new Rect(leftPosition.xMax + 3, position.y, leftPosition.width, position.height);
            UnityEditor.EditorGUI.PropertyField(rightPosition, property.FindPropertyRelative("b"), GUIContent.none);
        }
#endif
#endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when all all control buttons settings are loaded.
        /// </summary>
        public event Action OnAllControlsLoaded;
        #endregion

        #region [Getter / Setter]
        public InputActionAsset GetInputActions()
        {
            return inputActions;
        }

        public void SetInputActions(InputActionAsset value)
        {
            inputActions = value;
        }

        public string GetMapName()
        {
            return mapName;
        }

        public void SetMapName(string value)
        {
            mapName = value;
        }

        public CollisionSolving GetCollisionSolving()
        {
            return collisionSolving;
        }

        public void SetCollisionSolving(CollisionSolving value)
        {
            collisionSolving = value;
        }

        public Color GetDefaultColor()
        {
            return defaultColor;
        }

        public void SetDefaultColor(Color value)
        {
            defaultColor = value;
        }

        public Color GetHihlightColor()
        {
            return highlightColor;
        }

        public void SetHighlightColor(Color value)
        {
            highlightColor = value;
        }

        public InputActionMap GetMap()
        {
            return map;
        }

        public void SetMap(InputActionMap value)
        {
            map = value;
        }

        public int GetProcessorCount()
        {
            return processorCount;
        }

        public void SetProcessorCount(int value)
        {
            processorCount = value;
        }

        public int GetLoadedProcessorCount()
        {
            return loadedProcessorCount;
        }

        public void SetLoadedProcessorCount(int value)
        {
            loadedProcessorCount = value;
        }

        public List<InputBindingField[]> GetCollisions()
        {
            return collisions;
        }

        public void SetCollisions(List<InputBindingField[]> value)
        {
            collisions = value;
        }
        #endregion
    }
}
