/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using AuroraFPSRuntime.CoreModules.Coroutines;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Camera/Camera Shake/Camera Shake Builder")]
    [DisallowMultipleComponent]
    public sealed class CameraShakeBuilder : MonoBehaviour
    {
        private readonly List<ICameraShake> activeShakes = new List<ICameraShake>();

        public enum FetchType
        {
            Sequental,
            Random
        }

        [System.Serializable]
        private struct Build
        {
            public enum Algorithm
            {
                Bounce,
                Kick,
                PerlinNoise
            }

            [SerializeField]
            private Algorithm algorithm;

            [SerializeField]
            private float delay;

            [SerializeField]
            [Label("Settings")]
            [VisibleIf("algorithm", "Bounce")]
            private BounceShake.Settings bounceSettings;

            [SerializeField]
            [Label("Settings")]
            [VisibleIf("algorithm", "Kick")]
            private KickShake.Settings kickSettings;

            [SerializeField]
            [VisibleIf("algorithm", "Kick")]
            [Indent(1)]
            private bool attenuateStrength;

            [SerializeField]
            [Label("Settings")]
            [VisibleIf("algorithm", "PerlinNoise")]
            private PerlinShake.Settings perlinSettings;

            #region [Getter / Setter]
            public Algorithm GetAlgorithm()
            {
                return algorithm;
            }

            public void SetAlgorithm(Algorithm value)
            {
                algorithm = value;
            }

            public float GetDelay()
            {
                return delay;
            }

            public void SetDelay(float value)
            {
                delay = value;
            }

            public BounceShake.Settings GetBounceSettings()
            {
                return bounceSettings;
            }

            public void SetBounceSettings(BounceShake.Settings value)
            {
                bounceSettings = value;
            }

            public KickShake.Settings GetKickSettings()
            {
                return kickSettings;
            }

            public void SetKickSettings(KickShake.Settings value)
            {
                kickSettings = value;
            }

            public bool AttenuateStrength()
            {
                return attenuateStrength;
            }

            public void AttenuateStrength(bool value)
            {
                attenuateStrength = value;
            }

            public PerlinShake.Settings GetPerlinSettings()
            {
                return perlinSettings;
            }

            public void SetPerlinSettings(PerlinShake.Settings value)
            {
                perlinSettings = value;
            }
            #endregion
        }

        [SerializeField]
        [NotNull]
        private new Camera camera;

        [SerializeField]
        private bool playOnAwake = true;

        [SerializeField]
        private bool loop = true;

        [SerializeField]
        [VisibleIf("loop", true)]
        [Indent(1)]
        private FetchType fetchType = FetchType.Sequental;

        [SerializeField]
        [Slider(0, 1)]
        private float strengthMultiplier = 1.0f;

        [SerializeField]
        [ReorderableList(ElementLabel = "Build {niceIndex}")]
        private Build[] builds;


        // Stored required properties.
        private CoroutineObject coroutineObject;
        private Displacement displacement;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            coroutineObject = new CoroutineObject(this);
            displacement = new Displacement(transform.position, transform.eulerAngles);
            if (playOnAwake)
            {
                if (!loop)
                {
                    coroutineObject.Start(ForeachShakes, true);
                }
                else
                {
                    coroutineObject.Start(LoopShakes, true);
                }
            }
        }

        private IEnumerator ExecuteShake(int index)
        {
            if(builds != null && index < builds.Length)
            {
                Build build = builds[index];
                ICameraShake shake = null;
                switch (build.GetAlgorithm())
                {
                    case Build.Algorithm.Bounce:
                        shake = new BounceShake(build.GetBounceSettings());
                        break;
                    case Build.Algorithm.Kick:
                        shake = new KickShake(build.GetKickSettings(), transform.position, build.AttenuateStrength());
                        break;
                    case Build.Algorithm.PerlinNoise:
                        shake = new PerlinShake(build.GetPerlinSettings());
                        break;
                }
                shake.Initialize(transform.position, transform.rotation);
                activeShakes.Add(shake);
                if (build.GetDelay() == 0)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(build.GetDelay());
                }
            }
            yield break;
        }

        private IEnumerator LoopShakes()
        {
            while (builds != null && builds.Length > 0)
            {
                if(builds.Length == 1)
                {
                    yield return ExecuteShake(0);
                }
                else if (fetchType == FetchType.Sequental)
                {
                    yield return ForeachShakes();
                }
                else if (fetchType == FetchType.Random)
                {
                    int previousIndex = 0;
                    int index = 0;
                    do
                    {
                        index = Random.Range(0, builds.Length);
                        yield return null;
                    }
                    while (index == previousIndex);
                    previousIndex = index;
                    yield return ExecuteShake(index);
                }
            }
        }


        private IEnumerator ForeachShakes()
        {
            for (int i = 0; i < builds.Length; i++)
            {
                yield return ExecuteShake(i);
            }
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            Displacement cameraDisplacement = displacement;
            for (int i = activeShakes.Count - 1; i >= 0; i--)
            {
                if (activeShakes[i].IsFinished())
                {
                    activeShakes.RemoveAt(i);
                }
                else
                {
                    activeShakes[i].Update(Time.deltaTime, transform.position, transform.rotation);
                    cameraDisplacement += activeShakes[i].GetCurrentDisplacement();
                }
            }
            transform.localPosition = strengthMultiplier * cameraDisplacement.GetPosition();
            transform.localRotation = Quaternion.Euler(strengthMultiplier * cameraDisplacement.GetEulerAngles());
        }

        public void Execute()
        {
            if (!loop)
            {
                coroutineObject.Start(ForeachShakes, true);
            }
            else
            {
                coroutineObject.Start(LoopShakes, true);
            }
        }

        public void AddShakeWithoutDelay(int index)
        {
            if (index < builds.Length)
            {
                Build build = builds[index];
                ICameraShake shake = null;
                switch (build.GetAlgorithm())
                {
                    case Build.Algorithm.Bounce:
                        shake = new BounceShake(build.GetBounceSettings());
                        break;
                    case Build.Algorithm.Kick:
                        shake = new KickShake(build.GetKickSettings(), transform.position, build.AttenuateStrength());
                        break;
                    case Build.Algorithm.PerlinNoise:
                        shake = new PerlinShake(build.GetPerlinSettings());
                        break;
                }
                shake.Initialize(transform.position, transform.rotation);
                activeShakes.Add(shake);
            }
        }
    }
}
