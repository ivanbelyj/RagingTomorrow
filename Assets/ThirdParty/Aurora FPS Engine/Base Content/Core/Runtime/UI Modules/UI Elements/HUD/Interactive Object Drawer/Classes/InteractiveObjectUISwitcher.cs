/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Interactive Object/Interactive Object UI Switcher")]
    [DisallowMultipleComponent]
    public sealed class InteractiveObjectUISwitcher : MonoBehaviour
    {
        [System.Serializable]
        private sealed class StateElements
        {
            [SerializeField]
            private int messageCode;

            [SerializeField]
            [ReorderableList(ElementLabel = "Element", DisplayHeader = false)]
            private RectTransform[] elements;


            public void EnableElements()
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i].gameObject.SetActive(true);
                }
            }

            public void DisableElements()
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i].gameObject.SetActive(false);
                }
            }

            #region [Getter / Setter]
            public int GetMessageCode()
            {
                return messageCode;
            }

            public void SetMessageCode(int value)
            {
                messageCode = value;
            }

            public RectTransform[] GetElements()
            {
                return elements;
            }

            public void SetElements(RectTransform[] value)
            {
                elements = value;
            }
            #endregion
        }

        [SerializeField]
        [NotNull]
        private InteractiveObject interactiveObject;

        [SerializeField]
        [Array]
        private StateElements[] states;

        private int? lastMessageCode;

        private void OnEnable()
        {
            interactiveObject.OnCalculateMessageCodeCallback += UpdateUIElements;
            interactiveObject.OnBecomeInactiveCallback += DisableUIElements;
        }

        private void OnDisable()
        {
            interactiveObject.OnCalculateMessageCodeCallback -= UpdateUIElements;
            interactiveObject.OnBecomeInactiveCallback -= DisableUIElements;
        }

        private void UpdateUIElements(Transform other, int messageCode)
        {
            if(lastMessageCode != null && lastMessageCode == messageCode)
            {
                return;
            }

            StateElements state = null;
            for (int i = 0; i < states.Length; i++)
            {
                StateElements _state = states[i];
                if (_state.GetMessageCode() == messageCode)
                {
                    state = _state;
                }
                else
                {
                    _state.DisableElements();
                }
            }
            state?.EnableElements();
            lastMessageCode = messageCode;
        }

        private void DisableUIElements()
        {
            for (int i = 0; i < states.Length; i++)
            {
                states[i].DisableElements();
            }
            lastMessageCode = null;
        }

        #region [Getter / Setter]
        public InteractiveObject GetInteractiveObject()
        {
            return interactiveObject;
        }

        public void SetInteractiveObject(InteractiveObject value)
        {
            interactiveObject = value;
        }
        #endregion
    }
}