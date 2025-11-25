/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Linq;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/Common/Weapon/AI Weapon Helper")]
    public sealed class AIWeaponHelper : MonoBehaviour
    {
        [SerializeField]
        [Array]
        private Transform[] weaponObjects;

        // Stored required properties.
        private Transform[] parents;
        private Vector3[] startPositions;
        private Quaternion[] startRotations;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            parents = weaponObjects.Select(t => t.parent).ToArray();
            startPositions = weaponObjects.Select(t => t.localPosition).ToArray();
            startRotations = weaponObjects.Select(t => t.localRotation).ToArray();
        }

        /// <summary>
        /// Releases the weapon from his hands.
        /// </summary>
        public void ReleaseWeapon()
        {
            for (int i = 0; i < weaponObjects.Length; i++)
            {
                Transform t = weaponObjects[i];

                if (t.gameObject.activeSelf)
                {
                    t.SetParent(null);

                    Collider collider = t.GetComponent<Collider>();
                    collider.enabled = true;

                    Rigidbody rigidbody = t.GetComponent<Rigidbody>();
                    rigidbody.useGravity = true;
                    rigidbody.isKinematic = false;
                }
            }
        }

        /// <summary>
        /// Reset weapon to start position.
        /// </summary>
        public void ResetPosition()
        {
            for (int i = 0; i < weaponObjects.Length; i++)
            {
                Transform t = weaponObjects[i];

                if (t.gameObject.activeSelf)
                {
                    Rigidbody rigidbody = t.GetComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = true;

                    Collider collider = t.GetComponent<Collider>();
                    collider.enabled = false;

                    t.SetParent(parents[i]);
                    t.localPosition = startPositions[i];
                    t.localRotation = startRotations[i];
                }
            }
        }
    }
}