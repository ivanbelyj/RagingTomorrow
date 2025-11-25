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

namespace AuroraFPSRuntime.WeaponModules
{
    [System.Serializable]
    public sealed class ShootSounds
    {
        public enum FetchType
        {
            Sequential,
            Random
        }

        [SerializeField]
        private FetchType fetchType = FetchType.Sequential;

        [SerializeField]
        [ReorderableList(DisplayHeader = false, ElementLabel = null)]
        private AudioClip[] clips;

        // Stored required properties.
        private int clipIndex;

        public AudioClip FetchClip()
        {
            switch (fetchType)
            {
                case FetchType.Sequential:
                    return GetNextClip();
                case FetchType.Random:
                    return GetRandomClip();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get next clip from step clips array.
        /// </summary>
        public AudioClip GetNextClip()
        {
            if (clips != null && clips.Length > 0)
            {
                return clips[Mathf.Clamp(clipIndex++, 0, clips.Length -1)];
            }
            return null;
        }

        /// <summary>
        /// Get random clip form steps clips array.
        /// </summary>
        public AudioClip GetRandomClip()
        {
            if (clips != null && clips.Length > 0)
            {
                if (clips.Length > 1)
                {
                    int index = clipIndex;
                    clipIndex = Random.Range(0, clips.Length);
                    if (clipIndex == index)
                        clipIndex = Mathf.Clamp(++clipIndex, 0, clips.Length - 1);
                }
                return clips[clipIndex];
            }
            return null;
        }

        #region [Getter / Setter]
        public FetchType GetFetchType()
        {
            return fetchType;
        }

        public void SetFetchType(FetchType value)
        {
            fetchType = value;
        }

        public AudioClip[] GetClips()
        {
            return clips;
        }

        public void SetClips(AudioClip[] value)
        {
            clips = value;
        }
        #endregion
    }
}