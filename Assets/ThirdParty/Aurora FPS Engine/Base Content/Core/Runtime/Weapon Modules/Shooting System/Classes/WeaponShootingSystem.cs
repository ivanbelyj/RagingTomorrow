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
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using AuroraFPSRuntime.WeaponModules.RecoilSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WeaponReloadSystem))]
    public abstract class WeaponShootingSystem : MonoBehaviour, IInputListener
    {
        public enum Direction
        {
            ViewportCenter,
            TransformForward
        }

        protected readonly Vector3 ViewportCenter = new Vector3(0.5f, 0.5f, 0.0f);

        [SerializeField]
        [Order(-100)]
        private FireMode fireMode = FireMode.Free | FireMode.Single;

        [SerializeField]
        [ValueDropdown("FireModes")]
        [Order(-99)]
        private string startFireMode = FireMode.Free.ToString();

        [SerializeField]
        [NotNull]
        [Order(-98)]
        private Transform shootPoint;

        [SerializeField]
        [Indent(1)]
        [Order(-97)]
        private Direction direction = Direction.ViewportCenter;

        [SerializeField]
        [NotNull]
        [VisibleIf("direction", "TransformForward")]
        [Indent(1)]
        [Order(-96)]
        private Transform shootDirection;

        [SerializeField]
        [Label("Single")]
        [Foldout("Fire Mode Settings", Style = "Header")]
        [Suffix("rpm", true)]
        [MinValue(0.0f)]
        [Order(99)]
        private float singleRPM = 700f;

        [SerializeField]
        [Label("Free")]
        [Foldout("Fire Mode Settings", Style = "Header")]
        [Suffix("rpm", true)]
        [MinValue(0.0f)]
        [Order(100)]
        private float freeRPM = 700f;

        [SerializeField]
        [Label("Queue")]
        [Foldout("Fire Mode Settings", Style = "Header")]
        [Suffix("rpm", true)]
        [MinValue(0.0f)]
        [Order(101)]
        private float queueRPM = 700f;

        [SerializeField]
        [Label("Count")]
        [Foldout("Fire Mode Settings", Style = "Header")]
        [MinValue(2)]
        [Indent(1)]
        [Order(102)]
        private int queueCount = 3;

        [SerializeField]
        [Label("Config")]
        [Foldout("Recoil Settings", Style = "Header")]
        [Order(103)]
        private RecoilConfig recoilConfig;

        [SerializeField]
        [HideExpandButton]
        [Foldout("Sound Settings", Style = "Header")]
        [Order(104)]
        private ShootSounds fireSounds;

        [SerializeField]
        [HideExpandButton]
        [Foldout("Sound Settings", Style = "Header")]
        [Order(105)]
        private ShootSounds emptySounds;

        [SerializeField]
        [ReorderableList(DisplayHeader = false, ElementLabel = null)]
        [Foldout("Effect Settings", Style = "Header")]
        [Order(106)]
        private ParticleSystem[] particleEffects;

        [SerializeField]
        [Foldout("Events", Style = "Header")]
        [Order(300)]
        private UnityEvent onShootEvent;

        [SerializeField]
        [Foldout("Events", Style = "Header")]
        [Order(301)]
        private UnityEvent onEmptyEvent;

        [SerializeField]
        [Foldout("Events", Style = "Header")]
        [Order(302)]
        private UnityEvent onStartShootEvent;

        [SerializeField]
        [Foldout("Events", Style = "Header")]
        [Order(303)]
        private UnityEvent onStopShootEvent;

        // Stored required components.
        private PlayerCamera playerCamera;
        private PlayerController controller;
        private WeaponReloadSystem reloadSystem;
        private AudioSource audioSource;

        // Stored required properties.
        private bool isShooted;
        private bool isShootPerformed;
        private bool isShootStarted;
        private int indexCameraRecoil;
        private int indexBulletRecoil;
        private float recoilSnappinessUp;
        private float recoilSnappinessBack;
        private Vector3 targetRecoilRotation;
        private Vector3 currentRecoilRotation;
        private FireMode activeFireMode;
        private CoroutineObject coroutineObject;


        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            reloadSystem = GetComponent<WeaponReloadSystem>();
            controller = GetComponentInParent<PlayerController>();
            playerCamera = controller.GetPlayerCamera();
            coroutineObject = new CoroutineObject(this);

            Debug.Assert(shootPoint != null, $"<b><color=#FF0000>.Fire point reference is required!\nAttach reference of fire point to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name} <i>(component)</i> -> Fire Point<i>(field)</i>.</color></b>");
            Debug.Assert(controller != null, $"<b><color=#FF0000>Weapon Shooting System can used only as child gameobject of Player controller.\nAttach reference of the player controller to {transform.root.name}<i>(gameobject).</b>");

            CopyAudioSource(out audioSource);
            InstantiateRecoilHinge();

            OnShootCallback += onShootEvent.Invoke;
            OnEmptyCallback += onEmptyEvent.Invoke;
            OnStartShootCallback += onStartShootEvent.Invoke;
            OnStopShootCallback += onStopShootEvent.Invoke;
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            indexCameraRecoil = -1;
            indexBulletRecoil = -1;
            isShootPerformed = false;
            RegisterInputActions();
            if (Enum.TryParse<FireMode>(startFireMode, out FireMode fireMode))
            {
                SwitchFireMode(fireMode);
            }

            OnShootCallback += controller.BreakSprint;
            OnEmptyCallback += controller.BreakSprint;
            OnStopShootCallback += ResetRecoilIndex;
        }

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            CalculateRecoilRotation();
        }

        /// <summary>
        /// Called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            isShootPerformed = false;
            coroutineObject.Stop();
            OnShootCallback -= controller.BreakSprint;
            OnEmptyCallback -= controller.BreakSprint;
            OnStopShootCallback -= ResetRecoilIndex;
            RemoveInputActions();
        }

        /// <summary>
        /// Implement this method to make logic of shooting. 
        /// </summary>
        /// <param name="origin">Origin vection of shoot.</param>
        /// <param name="direction">Direction vector of shoot.</param>
        protected abstract void MakeShoot(Vector3 origin, Vector3 direction);

        /// <summary>
        /// Implement this method to change origin and direction of shoot. 
        /// </summary>
        public virtual Ray CalculateRay()
        {
            Ray ray = new Ray(shootPoint.position, Vector3.zero);
            switch (direction)
            {
                case Direction.ViewportCenter:
                    {
                        Ray viewportRay = playerCamera.GetCamera().ViewportPointToRay(ViewportCenter);
                        ray.direction = viewportRay.direction;
                        if (Physics.Raycast(viewportRay, out RaycastHit hitInfo, GetDirectionRange(), GetDirectionLayer(), QueryTriggerInteraction.Ignore))
                        {
                            Vector3 direction = hitInfo.point - shootPoint.position;
                            float dot = Vector3.Dot(viewportRay.direction, direction.normalized);
                            if (dot > 0)
                            {
                                ray.direction = direction;
                            }
                            else
                            {
                                ray = viewportRay;
                            }
                        }
                        return ray;
                    }

                default:
                case Direction.TransformForward:
                    {
                        ray.direction = shootDirection.forward;
                        if (Physics.Raycast(shootDirection.position, shootDirection.forward, out RaycastHit hitInfo, GetDirectionRange(), GetDirectionLayer(), QueryTriggerInteraction.Ignore))
                        {
                            Vector3 direction = hitInfo.point - shootPoint.position;
                            float dot = Vector3.Dot(shootDirection.forward, direction.normalized);
                            if (dot > 0)
                            {
                                ray.direction = direction;
                            }
                        }
                        return ray;
                    }
            }
        }

        /// <summary>
        /// Calculate origin of shoot.
        /// </summary>
        [Obsolete("Use CalculateRay instead.")]
        public virtual Vector3 CalculateOrigin()
        {
            return shootPoint.position;
        }

        /// <summary>
        /// Calculate direction of shoot.
        /// </summary>
        /// <param name="camera">Current player camera component.</param>
        /// <param name="firePoint">Reference of shoot point.</param>
        [Obsolete("Use CalculateRay instead.")]
        public virtual Vector3 CalculateDirection()
        {
            switch (direction)
            {
                case Direction.ViewportCenter:
                    Ray ray = playerCamera.GetCamera().ViewportPointToRay(ViewportCenter);
                    ray.origin = new Vector3(ray.origin.x, ray.origin.y, shootPoint.position.z);
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, GetDirectionRange(), GetDirectionLayer(), QueryTriggerInteraction.Ignore))
                        return hitInfo.point - shootPoint.position;
                    return ray.direction;
                default:
                case Direction.TransformForward:
                    if (Physics.Raycast(shootDirection.position, shootDirection.forward, out RaycastHit _hitInfo, GetDirectionRange(), GetDirectionLayer(), QueryTriggerInteraction.Ignore))
                        return _hitInfo.point - shootPoint.position;
                    return shootPoint.forward;
            }
        }

        /// <summary>
        /// Culling layer of calculation shoot direction.
        /// </summary>
        public virtual LayerMask GetDirectionLayer()
        {
            return Physics.AllLayers;
        }

        /// <summary>
        /// Maximum range of calculation shoot direction.
        /// </summary>
        public virtual float GetDirectionRange()
        {
            return Mathf.Infinity;
        }

        /// <summary>
        /// Reset the index of the current recoil to starting position.
        /// <br><i>By default, it is always reset after stop shooting.</i></br>
        /// </summary>
        public void ResetRecoilIndex()
        {
            indexCameraRecoil = -1;
            indexBulletRecoil = -1;
        }

        /// <summary>
        /// Register required callback to input actions.
        /// </summary>
        public virtual void RegisterInputActions()
        {
            InputReceiver.AttackAction.performed += OnShootAction;
            InputReceiver.AttackAction.canceled += OnShootAction;
            InputReceiver.SwitchFireModeAction.performed += OnFireModeChangedAction;
        }

        /// <summary>
        /// Remove registered callbacks from input actions.
        /// </summary>
        public virtual void RemoveInputActions()
        {
            InputReceiver.AttackAction.performed -= OnShootAction;
            InputReceiver.AttackAction.canceled -= OnShootAction;
            InputReceiver.SwitchFireModeAction.performed -= OnFireModeChangedAction;
        }

        /// <summary>
        /// Execute shooting logic.
        /// </summary>
        public void ExecuteShoot()
        {
            Ray ray = CalculateRay();
            Vector3 direction = ray.direction;
            ApplyRecoil(ref direction);
            MakeShoot(ray.origin, direction);
            PlayFireSound();
            PlayAllParticleEffects();
        }

        /// <summary>
        /// Execute shooting logic.
        /// </summary>
        public void ExecuteShoot(Vector3 origin, Vector3 direction)
        {
            ApplyRecoil(ref direction);
            MakeShoot(origin, direction.normalized);
            PlayFireSound();
            PlayAllParticleEffects();
        }

        /// <summary>
        /// Execute empty shooting logic.
        /// </summary>
        public void ExecuteEmptyShoot()
        {
            PlayEmptySound();
        }

        /// <summary>
        /// Copy audio source component of this weapon shooting system.
        /// </summary>
        /// <param name="audioSource">Refernce of audio source.</param>
        public virtual void CopyAudioSource(out AudioSource audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Set shoot direction to viewport center.
        /// </summary>
        public void SetViewportCenterDirection()
        {
            direction = Direction.ViewportCenter;
        }

        /// <summary>
        /// Set shoot direction to transform forward.
        /// </summary>
        public void SetTransformForwardDirection(Transform shootDirection)
        {
            if (shootDirection != null)
            {
                this.shootDirection = shootDirection;
                direction = Direction.TransformForward;
            }
        }

        /// <summary>
        /// Switch current fire mode to the new one.
        /// </summary>
        /// <param name="fireMode">New fire mode.</param>
        public void SwitchFireMode(FireMode fireMode)
        {
            switch (fireMode)
            {
                case FireMode.Single:
                    coroutineObject.Start(SingleFireModeProcessing, true);
                    break;
                case FireMode.Queue:
                    coroutineObject.Start(QueueFireModeProcessing, true);
                    break;
                case FireMode.Free:
                    coroutineObject.Start(FreeFireModeProcessing, true);
                    break;
                case FireMode.Mute:
                default:
                    coroutineObject.Stop();
                    break;
            }
            activeFireMode = fireMode;
        }

        /// <summary>
        /// Switch current fire mode to the next.
        /// </summary>
        public void MoveNextFireMode()
        {
            foreach (string mode in FireModes)
            {
                FireMode enum_mode = (FireMode)Enum.Parse(typeof(FireMode), mode);
                if (activeFireMode != enum_mode && (fireMode & enum_mode) != 0)
                {
                    switch (enum_mode)
                    {
                        case FireMode.Mute:
                            coroutineObject.Stop();
                            activeFireMode = FireMode.Mute;
                            return;
                        case FireMode.Single:
                            coroutineObject.Start(SingleFireModeProcessing, true);
                            activeFireMode = FireMode.Single;
                            return;
                        case FireMode.Queue:
                            coroutineObject.Start(QueueFireModeProcessing, true);
                            activeFireMode = FireMode.Queue;
                            return;
                        case FireMode.Free:
                            coroutineObject.Start(FreeFireModeProcessing, true);
                            activeFireMode = FireMode.Free;
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// Weapon single fire mode processing.
        /// </summary>
        private IEnumerator SingleFireModeProcessing()
        {
            WaitForSeconds fireRate = new WaitForSeconds(RPMToDelay(singleRPM));
            while (true)
            {
                if (!(reloadSystem?.IsReloading() ?? false) && isShootPerformed)
                {
                    if (reloadSystem.RemoveAmmo(1))
                    {
                        if (!isShootStarted)
                        {
                            OnStartShootCallback?.Invoke();
                            isShootStarted = true;
                        }

                        ExecuteShoot();
                        isShooted = true;
                        OnShootCallback?.Invoke();
                    }
                    else
                    {
                        ExecuteEmptyShoot();
                        OnEmptyCallback?.Invoke();
                    }
                    isShootPerformed = false;
                    yield return fireRate;
                }
                if (!isShooted && isShootStarted)
                {
                    OnStopShootCallback?.Invoke();
                    isShootStarted = false;
                }

                isShooted = false;
                yield return null;
            }
        }

        /// <summary>
        /// Weapon queue fire mode processing.
        /// </summary>
        private IEnumerator QueueFireModeProcessing()
        {
            WaitForSeconds fireRate = new WaitForSeconds(RPMToDelay(queueRPM));
            while (true)
            {
                if (!(reloadSystem?.IsReloading() ?? false) && isShootPerformed)
                {
                    int storedQueueCount = queueCount;
                    while (storedQueueCount > 0)
                    {
                        if (reloadSystem.RemoveAmmo(1))
                        {
                            if (!isShootStarted)
                            {
                                OnStartShootCallback?.Invoke();
                                isShootStarted = true;
                            }

                            ExecuteShoot();
                            storedQueueCount--;
                            isShooted = true;
                            OnShootCallback?.Invoke();
                        }
                        else
                        {
                            ExecuteEmptyShoot();
                            storedQueueCount = 0;
                            OnEmptyCallback?.Invoke();
                        }
                        isShootPerformed = false;
                        yield return fireRate;
                    }
                }
                if (!isShooted && isShootStarted)
                {
                    OnStopShootCallback?.Invoke();
                    isShootStarted = false;
                }
                isShooted = false;
                yield return null;
            }
        }

        /// <summary>
        /// Weapon free fire mode processing.
        /// </summary>
        private IEnumerator FreeFireModeProcessing()
        {
            WaitForSeconds fireRate = new WaitForSeconds(RPMToDelay(freeRPM));
            while (true)
            {
                if (!(reloadSystem?.IsReloading() ?? false))
                {
                    if (isShootPerformed && reloadSystem.RemoveAmmo(1))
                    {
                        if (!isShootStarted)
                        {
                            OnStartShootCallback?.Invoke();
                            isShootStarted = true;
                        }

                        ExecuteShoot();
                        isShooted = true;
                        OnShootCallback?.Invoke();
                        yield return fireRate;
                    }
                    else if (reloadSystem.GetAmmoCount() == 0 && isShootPerformed)
                    {
                        ExecuteEmptyShoot();
                        isShootPerformed = false;
                        OnEmptyCallback?.Invoke();
                        yield return null;
                    }
                }
                if (!isShooted && isShootStarted)
                {
                    OnStopShootCallback?.Invoke();
                    isShootStarted = false;
                }
                isShooted = false;
                yield return null;
            }
        }

        /// <summary>
        /// Instantiating of the camera recoil hinge and configuring it.
        /// </summary>
        private void InstantiateRecoilHinge()
        {
            if (RecoilHinge == null)
            {
                GameObject recoilHingeObject = new GameObject("Recoil Hinge");
                RecoilHinge = recoilHingeObject.transform;
                Transform[] children = new Transform[playerCamera.GetHinge().childCount];
                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = playerCamera.GetHinge().GetChild(i);
                }

                RecoilHinge.SetParent(playerCamera.GetHinge());
                RecoilHinge.localPosition = Vector3.zero;
                RecoilHinge.localRotation = Quaternion.identity;
                RecoilHinge.localScale = Vector3.one;

                for (int i = 0; i < children.Length; i++)
                {
                    children[i].transform.SetParent(RecoilHinge);
                }
            }
        }

        /// <summary>
        /// Calculate recoil hinge transform local rotation.
        /// </summary>
        private void CalculateRecoilRotation()
        {
            float snappiness = recoilSnappinessUp;
            if (!isShootStarted)
            {
                targetRecoilRotation = Vector3.zero;
                snappiness = recoilSnappinessBack;
            }
            currentRecoilRotation = Vector3.Slerp(currentRecoilRotation, targetRecoilRotation, snappiness * Time.deltaTime);
            RecoilHinge.localRotation = Quaternion.Euler(currentRecoilRotation);
        }

        /// <summary>
        /// Find actual recoil preset and apply it.
        /// </summary>
        private void ApplyRecoil(ref Vector3 direction)
        {
            recoilConfig.TryFindOverride(controller, out CameraRecoil cameraRecoil, out BulletRecoil bulletRecoil);
            recoilSnappinessUp = cameraRecoil.GetSnappinessUp();
            recoilSnappinessBack = cameraRecoil.GetSnappinessBack();
            cameraRecoil.MoveNext(ref indexCameraRecoil, ref targetRecoilRotation);
            bulletRecoil.MoveNext(ref indexBulletRecoil, ref direction);
        }

        /// <summary>
        /// Play fire sound from array.
        /// </summary>
        protected void PlayFireSound()
        {
            if (audioSource == null)
                return;

            AudioClip clip = fireSounds.FetchClip();
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Play dry fire sound from array.
        /// </summary>
        protected void PlayEmptySound()
        {
            if (audioSource == null)
                return;

            AudioClip clip = emptySounds.FetchClip();
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Play all assigned particle effects.
        /// </summary>
        protected void PlayAllParticleEffects()
        {
            if (particleEffects == null)
                return;

            for (int i = 0; i < particleEffects.Length; i++)
            {
                ParticleSystem particle = particleEffects[i];
                if (particle != null)
                {
                    particle.Play();
                }
            }
        }

        /// <summary>
        /// All available fire modes.
        /// </summary>
        public IEnumerable<string> FireModes
        {
            get
            {
                yield return FireMode.Mute.ToString();
                yield return FireMode.Single.ToString();
                yield return FireMode.Queue.ToString();
                yield return FireMode.Free.ToString();
            }
        }

        #region [Input Actions]
        private void OnShootAction(InputAction.CallbackContext context)
        {
            if (context.performed)
                isShootPerformed = true;
            else if (context.canceled)
                isShootPerformed = false;
        }

        private void OnFireModeChangedAction(InputAction.CallbackContext context)
        {
            MoveNextFireMode();
        }
        #endregion

        #region [Static Members]
        private static Transform RecoilHinge = null;

        /// <summary>
        /// Convert the common RPM unit of measurement to a delay in seconds.
        /// </summary>
        /// <param name="rpm">Value of round per minute.</param>
        /// <returns>Delay in seconds.</returns>
        public static float RPMToDelay(float rpm)
        {
            return 1 / (rpm / 60);
        }

        /// <summary>
        /// Convert the common RPM unit of measurement to a yield delay instruction.
        /// </summary>
        /// <param name="rpm">Value of round per minute.</param>
        /// <returns>Yield delay instruction (WaitForSeconds).</returns>
        public static WaitForSeconds RPMToInstruction(float rpm)
        {
            return new WaitForSeconds(RPMToDelay(rpm));
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when weapon is shoot.
        /// </summary>
        public event Action OnShootCallback;

        /// <summary>
        /// Called when weapon is dry shoot.
        /// </summary>
        public event Action OnEmptyCallback;

        /// <summary>
        /// Called when weapon start shooting.
        /// </summary>
        public event Action OnStartShootCallback;

        /// <summary>
        /// Called when weapon stop shooting.
        /// </summary>
        public event Action OnStopShootCallback;
        #endregion

        #region [Getter / Setter]
        public FireMode GetFireMode()
        {
            return fireMode;
        }

        public void SetFireMode(FireMode value)
        {
            fireMode = value;
        }

        public Transform GetShootPoint()
        {
            return shootPoint;
        }

        public void SetShootPoint(Transform value)
        {
            shootPoint = value;
        }

        public float GetSingleModeRPM()
        {
            return singleRPM;
        }

        public void SetSingleModeRPM(float value)
        {
            singleRPM = RPMToDelay(value);
        }

        public float GetFreeModeRPM()
        {
            return freeRPM;
        }

        public void SetFreeModeRPM(float value)
        {
            freeRPM = RPMToDelay(value);
        }

        public float GetQueueModeRPM()
        {
            return queueRPM;
        }

        public void SetQueueRPM(float value)
        {
            queueRPM = RPMToDelay(value);
        }

        public int GetQueueCount()
        {
            return queueCount;
        }

        public void SetQueueCount(int value)
        {
            queueCount = value;
        }

        public ShootSounds GetShootSounds()
        {
            return fireSounds;
        }

        public void SetShootSounds(ShootSounds value)
        {
            fireSounds = value;
        }

        public ShootSounds GetEmptySounds()
        {
            return emptySounds;
        }

        public void SetEmptySounds(ShootSounds value)
        {
            emptySounds = value;
        }

        public ParticleSystem[] GetParticleEffects()
        {
            return particleEffects;
        }

        public void SetParticleEffects(ParticleSystem[] value)
        {
            particleEffects = value;
        }
        #endregion
    }
}