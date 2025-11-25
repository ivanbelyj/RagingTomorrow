/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace AuroraFPSRuntime
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Event System/Trigger Event Callback")]
    [DisallowMultipleComponent]
    public sealed class TriggerEventReceiver : MonoBehaviour
    {
        [Serializable]
        private class TriggerEvent : UnityEvent<Transform> { }

        [SerializeField]
        private bool observe = true;

        [SerializeField]
        [Foldout("On Enter Event", Style = "Header")]
        private TriggerEvent onEnterEvent;

        [SerializeField]
        [Foldout("On Stay Event", Style = "Header")]
        private TriggerEvent onStayEvent;

        [SerializeField]
        [Foldout("On Exit Event", Style = "Header")]
        private TriggerEvent onExitEvent;

        private void OnTriggerEnter(Collider other)
        {
            if (enabled)
                onEnterEvent?.Invoke(other.transform);
        }

        private void OnTriggerStay(Collider other)
        {
            if (enabled)
                onStayEvent?.Invoke(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (enabled)
                onExitEvent?.Invoke(other.transform);
        }

        public void RegisterEnterEvent(UnityAction<Transform> callback)
        {
            onEnterEvent.AddListener(callback);
        }

        public void RegisterStayEvent(UnityAction<Transform> callback)
        {
            onStayEvent.AddListener(callback);
        }

        public void RegisterExitEvent(UnityAction<Transform> callback)
        {
            onExitEvent.AddListener(callback);
        }

        public void RemoveEnterEvent(UnityAction<Transform> callback)
        {
            onEnterEvent.RemoveListener(callback);
        }

        public void RemoveStayEvent(UnityAction<Transform> callback)
        {
            onStayEvent.RemoveListener(callback);
        }

        public void RemoveExitEvent(UnityAction<Transform> callback)
        {
            onExitEvent.RemoveListener(callback);
        }

        public void RemoveAllEnterEvents()
        {
            onEnterEvent.RemoveAllListeners();
        }

        public void RemoveAllStayEvents()
        {
            onStayEvent.RemoveAllListeners();
        }

        public void RemoveAllExitEvents()
        {
            onExitEvent.RemoveAllListeners();
        }

        #region [Getter / Setter]
        public bool Observe()
        {
            return observe;
        }

        public void Observe(bool observe)
        {
            this.observe = observe;
        }
        #endregion
    }
}
