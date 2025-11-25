/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.NotificationSystem
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Notification/Notification Board")]
    [DisallowMultipleComponent]
    public class NotificationBoard : MonoBehaviour, IEnumerable<NotificationLog>
    {
        [SerializeField]
        private RectTransform container;

        [SerializeField]
        private NotificationLog template;

        [SerializeField]
        private int capacity;

        // Stored required components.
        private int index;
        private NotificationLog[] logs;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            logs = new NotificationLog[capacity];
            for (int i = 0; i < capacity; i++)
            {
                NotificationLog log = Instantiate<NotificationLog>(template, Vector3.zero, Quaternion.identity, container);
                log.gameObject.SetActive(false);
                logs[i] = log;
            }
        }

        public NotificationLog Show()
        {
            NotificationLog log = logs[index++];
            log.transform.SetAsFirstSibling();
            log.Show();
            if(index >= capacity)
            {
                index = 0;
            }
            return log;
        }

        #region [IEnumerable<T> Implementation]
        public IEnumerator<NotificationLog> GetEnumerator()
        {
            for (int i = 0; i < capacity; i++)
            {
                yield return logs[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return logs.GetEnumerator();
        }
        #endregion

        #region [Getter / Setter]
        public RectTransform GetContainer()
        {
            return container;
        }

        public NotificationLog GetTemplate()
        {
            return template;
        }

        public int GetCapacity()
        {
            return capacity;
        }
        #endregion
    }
}