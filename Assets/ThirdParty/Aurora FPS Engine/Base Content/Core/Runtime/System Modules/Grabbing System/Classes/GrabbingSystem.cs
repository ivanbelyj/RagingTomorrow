/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.HealthModules;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.SystemModules.InventoryModules;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace AuroraFPSRuntime
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Grab/Grabbing System")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class GrabbingSystem : MonoBehaviour, IInputListener
    {
        [SerializeField] 
        private Transform playerCamera;

        [SerializeField] 
        private Transform attachBody;

        [SerializeField]
        [Label("Force")]
        [Foldout("Throw Settings", Style = "Header")]
        [MinValue(0.0f)]
        private float throwForce = 20.0f;

        [SerializeField]
        [Label("Sound")]
        [Foldout("Throw Settings", Style = "Header")]
        [MinValue(0.0f)]
        private AudioClip throwSound;

        [SerializeField]
        [Label("Range")]
        [Foldout("Advanced Settings", Style = "Header")]
        [MinValue(0.0f)]
        private float grabRange = 1.5f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [MinValue(0.0f)]
        private LayerMask cullingLayer = Physics.AllLayers;

        // Stored required components.
        private new Collider collider;
        private AudioSource audioSource;
        private GrabJoint storedGrabJoint;

        // Stored required properties.
        private bool isGrabbing;
        private CoroutineObject grabCoroutine;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            collider = GetComponent<Collider>();
            grabCoroutine = new CoroutineObject(this);

            // If CharacterHealth contained in the controller, then register the necessary callback.
            // If controller is die, automatically drop grabbed object.
            CharacterHealth health = GetComponent<CharacterHealth>();
            if (health != null)
            {
                health.OnDeadCallback += (other) => Drop();
            }

            // If DynamicRagdoll contained in the controller, then register the necessary callback.
            // If controller is ragdolled, automatically drop grabbed object.
            DynamicRagdoll dynamicRagdoll = GetComponent<DynamicRagdoll>();
            if (dynamicRagdoll != null)
            {
                dynamicRagdoll.OnSimulateRagdollCallback += Drop;
            }

            // If InventorySystem contained in the controller, then register the necessary callback.
            InventorySystem inventorySystem = GetComponent<InventorySystem>();
            if (inventorySystem != null)
            {
                OnGrabCallback += _ => inventorySystem.HideItem(true);
                OnThrowCallback += () => inventorySystem.HideItem(false);
                OnDropCallback += () => inventorySystem.HideItem(false);
                inventorySystem.OnEquipStartedCallback += _ => Drop();
            }
        }

        private void OnEnable()
        {
            RegisterInputActions();
        }

        private void OnDisable()
        {
            RemoveInputActions();
        }

        /// <summary>
        /// Start grabbing object.
        /// </summary>
        private void Grab(GrabJoint grabJoint)
        {   
            if(grabJoint == null)
            {
                return;
            }

            storedGrabJoint = grabJoint;
            storedGrabJoint.ConnectBody(attachBody);
            Physics.IgnoreCollision(collider, grabJoint.GetCollider(), true);
            storedGrabJoint.OnBreakCallback += Drop;
            OnGrabCallback?.Invoke(storedGrabJoint.gameObject);
            isGrabbing = true;
        }

        /// <summary>
        /// Drop current grabbed object.
        /// </summary>
        private void Drop()
        {
            if(storedGrabJoint == null)
            {
                return;
            }

            storedGrabJoint.DisconnectBody();
            Physics.IgnoreCollision(collider, storedGrabJoint.GetCollider(), false);
            storedGrabJoint.OnBreakCallback -= Drop;
            storedGrabJoint = null;
            OnDropCallback?.Invoke();
            isGrabbing = false;
        }

        /// <summary>
        /// Throw current grabbed object and stop grabbing.
        /// </summary>
        private void Throw(float force)
        {
             if(storedGrabJoint == null)
            {
                return;
            }

            storedGrabJoint.OnBreakCallback -= Drop;
            storedGrabJoint.DisconnectBody();
            Physics.IgnoreCollision(collider, storedGrabJoint.GetCollider(), false);
            Rigidbody rigidbody = storedGrabJoint.GetRigidbody();
            rigidbody.AddForce(playerCamera.forward * force, ForceMode.Impulse);
            PlayThrowSound();
            OnThrowCallback?.Invoke();
            storedGrabJoint = null;
            isGrabbing = false;
        }

        /// <summary>
        /// Play throw sound
        /// </summary>
        private void PlayThrowSound()
        {
            if (throwSound != null)
            {
                audioSource.PlayOneShot(throwSound);
            }
        }

        public void RegisterInputActions()
        {
            InputReceiver.GrabObjectAction.performed += OnGrabAction;
            InputReceiver.ThrowObjectAction.performed += OnThrowAction;
        }

        public void RemoveInputActions()
        {
            InputReceiver.GrabObjectAction.performed -= OnGrabAction;
            InputReceiver.ThrowObjectAction.performed -= OnThrowAction;
        }

        #region [Input Action Wrapper]
        private void OnGrabAction(InputAction.CallbackContext context)
        {
            if (!isGrabbing)
            {
                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, grabRange, cullingLayer, QueryTriggerInteraction.Ignore))
                {
                    GrabJoint grabJoint = hitInfo.transform.GetComponent<GrabJoint>();
                    if (grabJoint != null)
                    {
                        Grab(grabJoint);
                    }
                }
            }
            else if (isGrabbing && storedGrabJoint != null)
            {
                Drop();
            }
        }

        private void OnThrowAction(InputAction.CallbackContext context)
        {
            if (isGrabbing && storedGrabJoint != null)
            {
                Throw(throwForce);
            }
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// OnGrabCallback called once when controller grabbed object.
        /// </summary>
        /// <param name="GameObject">Grabbed gameobject.</param>
        public event Action<GameObject> OnGrabCallback;

        /// <summary>
        /// OnDropCallback called when grabbed object is dropped. 
        /// </summary>
        public event Action OnDropCallback;

        /// <summary>
        /// OnThrowCallback called when object throwed.
        /// </summary>
        public event Action OnThrowCallback;
        #endregion
       
        #region [Getter / Setter]
        public Transform GetPlayerCamera()
        {
            return playerCamera;
        }
        public void SetPlayerCamera(Transform value)
        {
            playerCamera = value;
        }

        public Transform GetAttachBody()
        {
            return attachBody;
        }

        public void SetAttachBody(Transform value)
        {
            attachBody = value;
        }

        public float GetGrabRange()
        {
            return grabRange;
        }

        public void SetGrabRange(float value)
        {
            grabRange = value;
        }

        public float GetThrowForce()
        {
            return throwForce;
        }

        public void SetThrowForce(float value)
        {
            throwForce = value;
        }

        public AudioClip GetThrowSound()
        {
            return throwSound;
        }

        public void SetThrowSound(AudioClip value)
        {
            throwSound = value;
        }

        public LayerMask GetGrabLayer()
        {
            return cullingLayer;
        }

        public void SetGrabLayer(LayerMask value)
        {
            cullingLayer = value;
        }

        public bool IsGrabbing()
        {
            return isGrabbing;
        }

        private void IsGrabbing(bool value)
        {
            isGrabbing = value;
        }

        public AudioSource GetAudioSource()
        {
            return audioSource;
        }

        public void SetAudioSource(AudioSource value)
        {
            audioSource = value;
        }
        #endregion
    }
}