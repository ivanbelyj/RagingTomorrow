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
using System.Collections;
using UnityEngine;
using System;

namespace AuroraFPSRuntime
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Grab/Grab Joint")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class GrabJoint : MonoBehaviour
    {
        [SerializeField] 
        private Vector3 anchor = new Vector3(-0.35f, -0.5f, 2.0f);

        [SerializeField] 
        private float smooth = 5.0f;

        [SerializeField] 
        private float breakVelocity = 15.0f;

        [SerializeField] 
        private float breakDistance = 2.5f;

        [SerializeField] 
        private bool freezeRotaion = true;

        [SerializeField] 
        private Vector3 customRotation;

        // Stored required components
        private new Rigidbody rigidbody;
        private new Collider collider;

        // Stored required properties
        private CoroutineObject<Transform> grabCoroutine;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            grabCoroutine = new CoroutineObject<Transform>(this);
        }

        /// <summary>
        /// Update transform position by anchor connection position. 
        /// </summary>
        protected virtual IEnumerator SavingConnection(Transform body)
        {
            while (true)
            {
                Vector3 connectPosition = body.TransformPoint(anchor) - transform.position;
#if UNITY_6000_0_OR_NEWER
                rigidbody.linearVelocity = connectPosition * smooth;
#else
                rigidbody.velocity = connectPosition * smooth;
#endif
                rigidbody.rotation = Quaternion.Lerp(rigidbody.rotation, body.rotation * Quaternion.Euler(customRotation), smooth * Time.deltaTime);

                if(Vector3.Distance(transform.position, body.root.position) >= breakDistance)
                {
                    ApplyDefaultSettings();
                    OnBreakCallback?.Invoke();
                    yield break;
                }
                yield return null;
            }
        }

        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun
        /// touching another rigidbody/collider.
        /// </summary>
        /// <param name="other">The Collision data associated with this collision.</param>
        protected virtual void OnCollisionEnter(Collision other)
        {
            if (other.relativeVelocity.magnitude >= breakVelocity)
            {
                ApplyDefaultSettings();
                OnBreakCallback?.Invoke();
            }
        }

        /// <summary>
        /// Connect transform to body.
        /// </summary>
        public virtual void ConnectBody(Transform body)
        {
            ApplyGrabSettings();
            grabCoroutine.Start(SavingConnection, body);
            OnConnectBodyCallback?.Invoke(body);
        }

        /// <summary>
        /// Disconnect transform from current connection.
        /// </summary>
        public virtual void DisconnectBody()
        {
            grabCoroutine.Stop();
            ApplyDefaultSettings();
            OnDisconectBodyCallback?.Invoke();
        }

        /// <summary>
        /// Connected object settings.
        /// Called once when object connect to the body.
        /// </summary>
        protected virtual void ApplyGrabSettings()
        {
            if (rigidbody != null)
            {
                rigidbody.useGravity = false;
                rigidbody.freezeRotation = freezeRotaion;
            }
        }

        /// <summary>
        /// Default object settings.
        /// Called once when object disconected from body.
        /// </summary>
        protected virtual void ApplyDefaultSettings()
        {
            if (rigidbody)
            {
#if UNITY_6000_0_OR_NEWER
                rigidbody.linearVelocity = Vector3.zero;
#else
                rigidbody.velocity = Vector3.zero;
#endif
                rigidbody.useGravity = true;
                rigidbody.freezeRotation = false;
            }
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Called when object enter of collision with other collider >= break velocity.
        /// </summary>
        public event Action OnBreakCallback;

        public event Action<Transform> OnConnectBodyCallback;

        public event Action OnDisconectBodyCallback;
        #endregion

        #region [Getter / Setter]
        public Vector3 GetAnchor()
        {
            return anchor;
        }

        public void SetAnchor(Vector3 value)
        {
            anchor = value;
        }

        public float GetSmooth()
        {
            return smooth;
        }

        public void SetSmooth(float value)
        {
            smooth = value;
        }

        public float GetBreakVelocity()
        {
            return breakVelocity;
        }

        public void SetBreakVelocity(float value)
        {
            breakVelocity = value;
        }

        public float GetBreakDistance()
        {
            return breakDistance;
        }

        public void SetBreakDistance(float value)
        {
            breakDistance = value;
        }

        public bool FreezeRotaion()
        {
            return freezeRotaion;
        }

        public void FreezeRotaion(bool value)
        {
            freezeRotaion = value;
        }

        public Vector3 GetCustomRotation()
        {
            return customRotation;
        }

        public void SetCustomRotation(Vector3 value)
        {
            customRotation = value;
        }

        public Rigidbody GetRigidbody()
        {
            return rigidbody;
        }

        public Collider GetCollider()
        {
            return collider;
        }
        #endregion
    }
}