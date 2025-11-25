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

namespace AuroraFPSRuntime.SystemModules
{
    [System.Serializable]
    public sealed class FootstepSoundStorage
    {
        // Clip type arrays
        [SerializeField]
        [ReorderableList(ElementLabel = null, Draggable = true, DrawClearButton = true)]
        private AudioClip[] stepClips;

        [SerializeField]
        [ReorderableList(ElementLabel = null, Draggable = true, DrawClearButton = true)]
        private AudioClip[] jumpClips;

        [SerializeField]
        [ReorderableList(ElementLabel = null, Draggable = true, DrawClearButton = true)]
        private AudioClip[] landClips;

        // Stored required properties.
        private int stepIndex;
        private int jumpIndex;
        private int landIndex;

        /// <summary>
        /// StepProperty constructor.
        /// </summary>
        public FootstepSoundStorage(AudioClip[] stepClips, AudioClip[] jumpClips, AudioClip[] landClips)
        {
            this.stepClips = stepClips;
            this.jumpClips = jumpClips;
            this.landClips = landClips;
        }

        /// <summary>
        /// Get next clip from step clips array.
        /// </summary>
        public AudioClip GetNextStepClip()
        {
            if (stepClips != null && stepClips.Length > 0)
            {
                int nextIndex = ++stepIndex;
                stepIndex = nextIndex >= stepClips.Length ? 0 : nextIndex;
                return stepClips[stepIndex];
            }
            return null;
        }

        /// <summary>
        /// Get next clip from jump clips array.
        /// </summary>
        public AudioClip GetNextJumpClip()
        {
            if (jumpClips != null && jumpClips.Length > 0)
            {
                int nextIndex = ++jumpIndex;
                jumpIndex = nextIndex >= jumpClips.Length ? 0 : nextIndex;
                return jumpClips[jumpIndex];
            }
            return null;
        }

        /// <summary>
        /// Get next clip from land clips array.
        /// </summary>
        public AudioClip GetNextLandClip()
        {
            if (landClips != null && landClips.Length > 0)
            {
                int nextIndex = ++landIndex;
                landIndex = nextIndex >= landClips.Length ? 0 : nextIndex;
                return landClips[landIndex];
            }
            return null;
        }

        /// <summary>
        /// Get new random clip form steps clips array.
        /// </summary>
        public AudioClip GetRandomStepClip()
        {
            if (stepClips != null && stepClips.Length > 0)
            {
                if (stepClips.Length > 1)
                {
                    int randomIndex = stepIndex;
                    while (randomIndex == stepIndex)
                    {
                        randomIndex = Random.Range(0, stepClips.Length);
                    }

                    stepIndex = randomIndex;
                }
                return stepClips[stepIndex];
            }
            return null;
        }

        /// <summary>
        /// Get new random clip form jump clips array.
        /// </summary>
        public AudioClip GetRandomJumpClip()
        {
            if (jumpClips != null && jumpClips.Length > 0)
            {
                if (jumpClips.Length > 1)
                {
                    int randomIndex = jumpIndex;
                    while (randomIndex == jumpIndex)
                    {
                        randomIndex = Random.Range(0, jumpClips.Length);
                    }

                    jumpIndex = randomIndex;
                }
                return jumpClips[jumpIndex];
            }
            return null;
        }

        /// <summary>
        /// Get new random clip form land clips array.
        /// </summary>
        public AudioClip GetRandomLandClip()
        {
            if (landClips != null && landClips.Length > 0)
            {
                if (landClips.Length > 1)
                {
                    int randomIndex = landIndex;
                    while (randomIndex == landIndex)
                    {
                        randomIndex = Random.Range(0, landClips.Length);
                    }

                    landIndex = randomIndex;
                }
                return landClips[landIndex];
            }
            return null;
        }

        #region [Getter / Setter]
        /// <summary>
        /// Get step clips.
        /// </summary>
        public AudioClip[] GetStepClips()
        {
            return stepClips;
        }

        /// <summary>
        /// Set range step clips.
        /// </summary>
        /// <param name="stepClips"></param>
        public void SetStepClips(AudioClip[] stepClips)
        {
            this.stepClips = stepClips;
        }

        /// <summary>
        /// Get step clip.
        /// </summary>
        /// <param name="index">Step clip index.</param>
        public AudioClip GetStepClip(int index)
        {
            return stepClips[index];
        }

        /// <summary>
        /// Set step clip.
        /// </summary>
        /// <param name="index">Step clip index.</param>
        /// <param name="stepClip">Step clip.</param>
        public void SetStepClip(int index, AudioClip stepClip)
        {
            stepClips[index] = stepClip;
        }

        /// <summary>
        /// Get jump clips.
        /// </summary>
        public AudioClip[] GetJumpClips()
        {
            return jumpClips;
        }

        /// <summary>
        /// Set range jump clips.
        /// </summary>
        /// <param name="jumpClips"></param>
        public void SetJumpClips(AudioClip[] jumpClips)
        {
            this.jumpClips = jumpClips;
        }

        /// <summary>
        /// Get jump clip.
        /// </summary>
        /// <param name="index">Jump clip index.</param>
        public AudioClip GetJumpClip(int index)
        {
            return jumpClips[index];
        }

        /// <summary>
        /// Set jump clip.
        /// </summary>
        /// <param name="index">Jump clip index.</param>
        /// <param name="jumpClip">Jump clip.</param>
        public void SetJumpClip(int index, AudioClip jumpClip)
        {
            jumpClips[index] = jumpClip;
        }

        /// <summary>
        /// Get land clips.
        /// </summary>
        public AudioClip[] GetLandClips()
        {
            return landClips;
        }

        /// <summary>
        /// Set range land clips.
        /// </summary>
        public void SetLandClips(AudioClip[] landClips)
        {
            this.landClips = landClips;
        }

        /// <summary>
        /// Get land clip.
        /// </summary>
        /// <param name="index">Land clip index.</param>
        public AudioClip GetLandClip(int index)
        {
            return landClips[index];
        }

        /// <summary>
        /// Set land clip.
        /// </summary>
        /// <param name="index">Land clip index.</param>
        /// <param name="landClip">Land clip.</param>
        public void SetLandClip(int index, AudioClip landClip)
        {
            landClips[index] = landClip;
        }

        /// <summary>
        /// Get step clips array length.
        /// </summary>
        public int GetStepClipsLength()
        {
            return stepClips.Length;
        }

        /// <summary>
        /// Get jump clips array length.
        /// </summary>
        public int GetJumpClipsLength()
        {
            return jumpClips.Length;
        }

        /// <summary>
        /// Get land clips array length.
        /// </summary>
        public int GetLandClipsLength()
        {
            return landClips.Length;
        }
        #endregion
    }
}