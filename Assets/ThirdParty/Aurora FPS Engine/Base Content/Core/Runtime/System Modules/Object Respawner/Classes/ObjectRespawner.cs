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
using AuroraFPSRuntime.SystemModules.HealthModules;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Spawn/Object Respawner")]
    public sealed class ObjectRespawner : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private ObjectHealth target;

        [SerializeField]
        private bool inCircle = false;

        [SerializeField]
        [VisibleIf("inCircle", true)]
        [MinValue(1)]
        [Indent(1)]
        private float radius = 2.5f;

        [SerializeField]
        [MinValue(0)]
        private float timer = 2.5f;

        [SerializeField]
        private bool randomizeTimer = false;

        [SerializeField]
        [VisibleIf("randomizeTimer", true)]
        [MinValue("timer")]
        [Indent(1)]
        private float maxTimer = 5.0f;

        // Stored required properties.
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            coroutineObject = new CoroutineObject(this);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            target.OnDeadCallback += OnDeadCallback;
            target.OnReviveCallback += OnReviveCallback;
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            target.OnDeadCallback -= OnDeadCallback;
            target.OnReviveCallback -= OnReviveCallback;
        }

        private IEnumerator Respawn()
        {
            if (!target.IsAlive())
            {
                if (randomizeTimer)
                    yield return new WaitForSeconds(Random.Range(timer, maxTimer));
                else
                    yield return new WaitForSeconds(timer);

                Vector3 point = transform.position;
                if (inCircle)
                {
                    Vector2 randomPoint = Random.insideUnitCircle * radius;
                    point += new Vector3(randomPoint.x, point.y, randomPoint.y);
                }
                target.transform.position = point;
                target.transform.rotation = Quaternion.LookRotation(transform.forward);
                target.ApplyHealth(target.GetMaxHealth());
            }
        }

        private void OnDeadCallback(Transform other)
        {
            coroutineObject.Start(Respawn);
        }

        private void OnReviveCallback()
        {
            coroutineObject.Stop();
        }

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (inCircle)
            {
                UnityEditor.Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.1f);
                UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, radius);
                UnityEditor.Handles.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius);
                UnityEditor.Handles.ArrowHandleCap(1, transform.position, Quaternion.LookRotation(transform.forward), 0.75f, EventType.Repaint);
            }
            else
            {
                UnityEditor.Handles.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                UnityEditor.Handles.SphereHandleCap(0, transform.position, Quaternion.LookRotation(Vector3.down), 0.25f, EventType.Repaint);
                UnityEditor.Handles.ArrowHandleCap(1, transform.position, Quaternion.LookRotation(transform.forward), 0.75f, EventType.Repaint);
            }
        }

        [UnityEditor.MenuItem("GameObject/Aurora FPS Engine/Spawn/Object Respawner", false, 20)]
        private static void Create()
        {
            GameObject objectRespawner = new GameObject("Object Respawner");
            objectRespawner.AddComponent<ObjectRespawner>();
            UnityEditor.Selection.activeGameObject = objectRespawner;
            UnityEditor.EditorGUIUtility.PingObject(objectRespawner);
        }
#endif
#endregion

        #region [Getter / Setter]
        public ObjectHealth GetTarget()
        {
            return target;
        }

        public void SetTarget(ObjectHealth value)
        {
            if(target != value)
            {
                target.OnDeadCallback -= OnDeadCallback;
                target.OnReviveCallback -= OnReviveCallback;

                value.OnDeadCallback += OnDeadCallback;
                value.OnReviveCallback += OnReviveCallback;
            }
            target = value;
        }

        public bool InCircle()
        {
            return inCircle;
        }

        public void InCircle(bool value)
        {
            inCircle = value;
        }

        public float GetRadius()
        {
            return radius;
        }

        public void SetRadius(float value)
        {
            radius = value;
        }

        public float GetTimer()
        {
            return timer;
        }

        public void SetTimer(float value)
        {
            timer = value;
        }

        public bool RandomizeTimer()
        {
            return randomizeTimer;
        }

        public void RandomizeTimer(bool value)
        {
            randomizeTimer = value;
        }

        public float GetMaxTimer()
        {
            return maxTimer;
        }

        public void SetMaxTimer(float value)
        {
            maxTimer = value;
        }
        #endregion
    }
}