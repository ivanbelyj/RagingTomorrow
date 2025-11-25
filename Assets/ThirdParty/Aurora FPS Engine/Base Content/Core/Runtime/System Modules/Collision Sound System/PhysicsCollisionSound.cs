/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Physics/Physics Collision Sound")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public sealed class PhysicsCollisionSound : MonoBehaviour
    {
        [System.Serializable]
        public struct CollisionEvent
        {
            [NotNull]
            [Tooltip("An audio clip that will be played when colliding with other objects in this velocity range.")]
            public AudioClip clip;

            [Slider(0, 1)]
            [Tooltip("Audio clip volume.")]
            public float volume;

            [MinMaxSlider(0.01f, 500.0f)]
            [Tooltip("Velocity range to play clip with this volume.")]
            public Vector2 velocity;

            public CollisionEvent(AudioClip clip, float volume, Vector2 velocity)
            {
                this.clip = clip;
                this.volume = volume;
                this.velocity = velocity;
            }
        }

        // Serialized properties.
        [SerializeField]
        [ReorderableList(
            Draggable = true, 
            ElementLabel = "Event {niceIndex}")]
        private CollisionEvent[] collisionEvents;

        // Stored required components.
        private AudioSource audioSource;


        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
        /// </summary>
        /// <param name="other">The Collision data associated with this collision.</param>
        private void OnCollisionEnter(Collision other)
        {
            if (collisionEvents != null && collisionEvents.Length > 0)
            {
                float relativeVelocity = other.relativeVelocity.sqrMagnitude;
                for (int i = 0; i < collisionEvents.Length; i++)
                {
                    CollisionEvent kickSound = collisionEvents[i];
                    if (kickSound.clip != null)
                    {
                        if (Math.InRange(relativeVelocity, kickSound.velocity))
                        {
                            audioSource.PlayOneShot(kickSound.clip, kickSound.volume);
                            break;
                        }
                    }
                    
                }
            }
        }

        #region [Getter / Setter]
        public CollisionEvent[] GetCollisionEvents()
        {
            return collisionEvents;
        }

        public void SetCollisionEvents(CollisionEvent[] value)
        {
            collisionEvents = value;
        }

        public AudioSource GetAudioSource()
        {
            return audioSource;
        }
        #endregion
    }
}