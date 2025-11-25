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
using AuroraFPSRuntime.CoreModules.ValueTypes;

namespace AuroraFPSRuntime
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Event System/Event Callback Receiver")]
    [DisallowMultipleComponent]
    public sealed class EventCallbackReceiver : MonoBehaviour
    {
        [System.Serializable]
        public class CallbackReceiver : CallbackEvent<UnityEvent> { }

        [SerializeField]
        [Array]
        private CallbackReceiver[] callbacks;

        private void Awake()
        {
            for (int i = 0; i < callbacks.Length; i++)
            {
                callbacks[i].RegisterCallback(InvokeUnityEvent);
            }
        }

        public void InvokeUnityEvent(UnityEvent unityEvent)
        {
            unityEvent?.Invoke();
        }
    }
}
