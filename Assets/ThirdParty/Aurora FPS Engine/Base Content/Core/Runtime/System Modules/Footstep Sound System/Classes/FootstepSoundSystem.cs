/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Footstep/Footstep Sound System")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public partial class FootstepSoundSystem : MonoBehaviour
    {
        internal const float ANIMATION_WEIGHT_RATIO = 0.5f;

        [SerializeField]
        [Tooltip("The algorithm by which the sounds will be played.")]
        [Order(-998)]
        private ProcessingType processingType = ProcessingType.AnimationEvent;

        [SerializeField]
        [Tooltip("The algorithm by which the sounds will be played.")]
        [Order(-997)]
        private FetchType fetchType = FetchType.Random;

        [SerializeField]
        [NotNull]
        [Tooltip("Footstep sound mapping.")]
        [Order(-996)]
        private FootstepSoundMapping mapping;

        [SerializeField]
        [ReorderableList(DisplayHeader = false)]
        [Foldout("Interval Settings", Style = "Header")]
        [VisibleIf("processingType", "Interval")]
        [Order(990)]
        private Interval[] intervals;

        [SerializeField]
        [NotNull]
        [Foldout("Foot Settings", Style = "Header")]
        [VisibleIf("processingType", "Procedural")]
        [Order(991)]
        private Transform leftFoot;

        [SerializeField]
        [NotNull]
        [Foldout("Foot Settings", Style = "Header")]
        [VisibleIf("processingType", "Procedural")]
        [Order(992)]
        private Transform rightFoot;

        [SerializeField]
        [MinValue(0.01f)]
        [Foldout("Advanced Settings", Style = "Header")]
        [Tooltip("Vertical check ground range distance.")]
        [Order(993)]
        private float range = 0.1f;

        [SerializeField]
        [MinValue(0.0f)]
        [Foldout("Advanced Settings", Style = "Header")]
        [Tooltip("Vertical height offset distance.")]
        [Order(994)]
        private float heightOffset = 0.1f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Tooltip("Ground culling layer.")]
        [Order(995)]
        private LayerMask cullingLayer = 1 << 0;

        // Stored required components.
        private AudioSource audioSource;
        private TerrainManager terrainManager;

        // Stored required properties.
        private CoroutineObject processnigCoroutine;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            terrainManager = TerrainManager.GetRuntimeInstance();
            processnigCoroutine = new CoroutineObject(this);
        }

        protected virtual void Start()
        {
            RegisterControllerEvents();
        }

        protected virtual void OnEnable()
        {
            switch (processingType)
            {
                case ProcessingType.Interval:
                    processnigCoroutine.Start(IntervalStepProcessing, true);
                    break;
                case ProcessingType.Procedural:
                    processnigCoroutine.Start(ProceduralStepProcessing, true);
                    break;
            }
        }

        protected virtual void OnDisable()
        {
            processnigCoroutine.Stop(); 
        }

        /// <summary>
        /// Play footstep clip called from Animation event.
        /// Implement this method to make custom Animation event.
        /// </summary>
        protected virtual void PlayFootstepSound(AnimationEvent evt)
        {
            if (evt.animatorClipInfo.weight > ANIMATION_WEIGHT_RATIO && TryGetSoundsRelativeSurface(transform.position, out FootstepSoundStorage sounds) && TryGetStepClip(sounds, out AudioClip clip))
            {
                PlayClip(clip);
            }
        }

        /// <summary>
        /// Play footstep clip after reach every interval.
        /// Implement this method to make custom interval processing.
        /// </summary>
        protected IEnumerator IntervalStepProcessing()
        {
            yield return null;
            PlayerController controller = GetComponent<PlayerController>();
            float rateTime = 0.01f;
            WaitForSeconds rate = new WaitForSeconds(rateTime);
            while (true)
            {
                Vector3 velocity = controller.GetVelocity();
                velocity.y = 0;
                float velocityMagnitude = velocity.sqrMagnitude;
                if (velocityMagnitude == 0)
                {
                    yield return null;
                    continue;
                }

                if (rateTime > 0 && TryGetSoundsRelativeSurface(transform.position, out FootstepSoundStorage sounds))
                {
                    if (sounds != null)
                    {
                        AudioClip clip = null;
                        switch (fetchType)
                        {
                            case FetchType.Sequential:
                                clip = sounds.GetNextStepClip();
                                if (clip != null)
                                {
                                    PlayClip(clip);
                                }
                                break;
                            case FetchType.Random:
                                clip = sounds.GetRandomStepClip();
                                if (clip != null)
                                {
                                    PlayClip(clip);
                                }
                                break;
                        }
                    }
                }

                for (int i = 0; i < intervals.Length; i++)
                {
                    rateTime = 0;
                    Interval interval = intervals[i];
                    if (CoreModules.Mathematics.Math.InRange(velocityMagnitude, interval.GetVelocity()) && rateTime != interval.GetRate())
                    {
                        rate = new WaitForSeconds(interval.GetRate());
                        rateTime = interval.GetRate();
                        break;
                    }
                }

                if(rateTime == 0)
                {
                    yield return null;
                }
                else
                {
                    yield return rate;
                }
            }
        }

        /// <summary>
        /// Procedural play footstep sound, when foot's touch surface.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator ProceduralStepProcessing()
        {
            yield return null;
            FootstepSoundStorage sounds = null;
            AudioClip clip = null;
            while (true)
            {
                if (TryGetSoundsRelativeSurface(leftFoot.position, out sounds) && TryGetStepClip(sounds, out clip))
                {
                    PlayClip(clip);
                }

                if (TryGetSoundsRelativeSurface(rightFoot.position, out sounds) && TryGetStepClip(sounds, out clip))
                {
                    PlayClip(clip);
                }
                yield return null;
            }
        }

        /// <summary>
        /// Try to get audio clip by fetch type from Sounds.
        /// </summary>
        protected bool TryGetStepClip(FootstepSoundStorage sounds, out AudioClip clip)
        {
            clip = null;
            if (sounds != null)
            {
                switch (fetchType)
                {
                    case FetchType.Sequential:
                        clip = sounds.GetNextStepClip();
                        break;
                    case FetchType.Random:
                        clip = sounds.GetRandomStepClip();
                        break;
                }

                if (clip != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Try to get audio clip by fetch type from Sounds.
        /// </summary>
        protected bool TryGetJumpClip(FootstepSoundStorage sounds, out AudioClip clip)
        {
            clip = null;
            if (sounds != null)
            {
                switch (fetchType)
                {
                    case FetchType.Sequential:
                        clip = sounds.GetNextJumpClip();
                        break;
                    case FetchType.Random:
                        clip = sounds.GetRandomJumpClip();
                        break;
                }

                if (clip != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Try to get audio clip by fetch type from Sounds.
        /// </summary>
        protected bool TryGetLandClip(FootstepSoundStorage sounds, out AudioClip clip)
        {
            clip = null;
            if (sounds != null)
            {
                switch (fetchType)
                {
                    case FetchType.Sequential:
                        clip = sounds.GetNextLandClip();
                        break;
                    case FetchType.Random:
                        clip = sounds.GetRandomLandClip();
                        break;
                }

                if (clip != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get footstep property by current position surface.
        /// </summary>
        protected bool TryGetSoundsRelativeSurface(Vector3 position, out FootstepSoundStorage sounds)
        {
            Vector3 start = new Vector3(position.x, position.y + heightOffset, position.z);
            Vector3 end = new Vector3(position.x, position.y - range, position.z);
            if (Physics.Linecast(start, end, out RaycastHit hitInfo, cullingLayer, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.transform.CompareTag(TNC.Terrain) && terrainManager.GetTerrainTextureDetectors() != null)
                {
                    for (int i = 0, length = terrainManager.GetTerrainTextureDetectors().Length; i < length; i++)
                    {
                        TerrainTextureDetector terrainTextureDetector = terrainManager.GetTerrainTextureDetector(i);
                        if (terrainTextureDetector.GetTerrain().transform == hitInfo.transform)
                        {
                            Texture2D terrainTexture = terrainTextureDetector.GetActiveTexture(hitInfo.point);
                            if (terrainTexture != null && mapping.ContainsKey(terrainTexture))
                            {
                                sounds = mapping.GetValue(terrainTexture);
                                return true;
                            }
                        }
                    }
                }
                else
                {
#if UNITY_6000_0_OR_NEWER
                    PhysicsMaterial physicMaterial = hitInfo.collider.sharedMaterial;
#else
                    PhysicMaterial physicMaterial = hitInfo.collider.sharedMaterial;
#endif
                    if (physicMaterial != null && mapping.ContainsKey(physicMaterial))
                    {
                        sounds = mapping.GetValue(physicMaterial);
                        return true;
                    }
                }
            }
            sounds = null;
            return false;
        }

        /// <summary>
        /// Register controller jump and land sound callbacks.
        /// </summary>
        protected void RegisterControllerEvents()
        {
            PlayerController controller = GetComponentInParent<PlayerController>();
            if (controller != null)
            {
                controller.OnJumpCallback += OnJumpSoundCallback;
                controller.OnGroundedCallback += OnLandSoundCallback;
            }
        }

        /// <summary>
        /// On controller jump sound callback event.
        /// </summary>
        private void OnJumpSoundCallback()
        {
            if (TryGetSoundsRelativeSurface(transform.position, out FootstepSoundStorage sounds) && TryGetJumpClip(sounds, out AudioClip clip))
            {
                PlayClip(clip);
            }
        }

        /// <summary>
        /// On controller land sound callback event.
        /// </summary>
        private void OnLandSoundCallback()
        {
            if (TryGetSoundsRelativeSurface(transform.position, out FootstepSoundStorage sounds) && TryGetLandClip(sounds, out AudioClip clip))
            {
                PlayClip(clip);
            }
        }

        protected void PlayClip(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
            OnPlayClipCallback?.Invoke(clip);
        }

        #region [Aurora Engine Debug Directive]
#if AURORA_ENGINE_DEBUG && UNITY_EDITOR
        [SerializeField]
        [Slider(0, 1)]
        [Foldout("Debug Settings", Style = "Header")]
        [Order(9991)]
        private float hitRadius = 0.05f;

        [SerializeField]
        [Foldout("Debug Settings", Style = "Header")]
        [Order(9992)]
        private Color hitColor = Color.red;

        [SerializeField]
        [Foldout("Debug Settings", Style = "Header")]
        [Order(9993)]
        private Color lineColor = Color.red;

        private void OnDrawGizmos()
        {
            Gizmos.color = lineColor;

            Vector3 start = new Vector3(transform.position.x, transform.position.y + heightOffset, transform.position.z);
            Vector3 end = new Vector3(transform.position.x, transform.position.y - range, transform.position.z);
            Gizmos.DrawLine(start, end);

            if (Physics.Linecast(start, end, out RaycastHit hitInfo, cullingLayer, QueryTriggerInteraction.Ignore))
            {
                Gizmos.color = hitColor;
                Gizmos.DrawSphere(hitInfo.point, hitRadius);
            }
        }
#endif
        #endregion

        #region [Event Callback Functions]
        public event Action<AudioClip> OnPlayClipCallback;
        #endregion

        #region [Getter / Setter]
        public FootstepSoundMapping GetMapping()
        {
            return mapping;
        }

        public void SetMapping(FootstepSoundMapping value)
        {
            mapping = value;
        }

        public ProcessingType GetProcessingType()
        {
            return processingType;
        }

        public void SetProcessingType(ProcessingType value)
        {
            processingType = value;
        }

        public FetchType GetFetchType()
        {
            return fetchType;
        }

        public void SetFetchType(FetchType value)
        {
            fetchType = value;
        }

        public float GetRange()
        {
            return range;
        }

        public void SetRange(float value)
        {
            range = value;
        }

        public LayerMask GetCullingLayer()
        {
            return cullingLayer;
        }

        public void SetCullingLayer(LayerMask value)
        {
            cullingLayer = value;
        }

        public AudioSource GetAudioSource()
        {
            return audioSource;
        }

        public TerrainManager GetTerrainManager()
        {
            return terrainManager;
        }
        #endregion
    }
}