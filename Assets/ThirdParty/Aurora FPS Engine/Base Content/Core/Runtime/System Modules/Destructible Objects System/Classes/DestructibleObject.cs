/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Alexandra Averyanova
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{ 
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Interactive/Destructible Object")]
    [DisallowMultipleComponent]
    public sealed class DestructibleObject : ObjectHealth
    {
        private Rigidbody[] components;

        /// <summary>
        /// Called when the script is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            components = GetComponentsInChildren<Rigidbody>();
            for(int i = 0; i < components.Length; i++)
            {
                Rigidbody component = components[i];
                component.isKinematic = true;
            }
        }

        /// <summary>
        /// Called when the object health becomes zero.
        /// </summary>
        protected override void OnDead()
        {
            for (int i = 0; i < components.Length; i++)
            {
                Rigidbody component = components[i];
                component.isKinematic = false;
            }
        }

        /// <summary>
        /// Draws gizmos on components when the object is selected
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if(components == null)
            {
                components = GetComponentsInChildren<Rigidbody>();
            }
            for (int i = 0; i < components.Length; i++)
            {
                Transform componentTransform = components[i].transform;
                if (componentTransform.TryGetComponent(out MeshFilter filter))
                {
                    Gizmos.DrawWireMesh(filter.sharedMesh, componentTransform.position, 
                        componentTransform.rotation, componentTransform.lossyScale);
                }
            }
        }
    }
}
