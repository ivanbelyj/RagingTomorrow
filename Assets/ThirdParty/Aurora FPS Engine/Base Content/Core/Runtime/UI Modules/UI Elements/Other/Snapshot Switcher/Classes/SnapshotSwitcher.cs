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
using UnityEngine.Audio;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Event Wrappers/Snapshot Switcher")]
    public sealed class SnapshotSwitcher : MonoBehaviour
    {
        [SerializeField]
        private AudioMixerSnapshot game;

        [SerializeField]
        private AudioMixerSnapshot pause;

        [SerializeField]
        private float duration = 1.0f;

        public void Paused(bool value)
        {
            if (value)
                pause.TransitionTo(duration);
            else
                game.TransitionTo(duration);
        }

        public AudioMixerSnapshot GetGameSnapshot()
        {
            return game;
        }

        public void SetGameSnapshot(AudioMixerSnapshot value)
        {
            game = value;
        }

        public AudioMixerSnapshot GetPauseSnapshot()
        {
            return pause;
        }

        public void SetPauseSnapshot(AudioMixerSnapshot value)
        {
            pause = value;
        }

        public float GetTransitionDuration()
        {
            return duration;
        }

        public void SetTransitionDuration(float value)
        {
            duration = value;
        }
    }
}
