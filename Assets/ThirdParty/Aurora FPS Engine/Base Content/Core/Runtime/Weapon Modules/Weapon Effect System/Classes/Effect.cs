/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules.EffectSystem
{
    [System.Serializable]
    public abstract class Effect
    {
        /// <summary>
        /// Called when the effect instance is being loaded.
        /// </summary>
        /// <param name="weapon">Weapon owner transform of this effect instance.</param>
        public virtual void Initialize(Transform weapon)
        {

        }

        /// <summary>
        /// Called when the effect becomes enabled and active.
        /// </summary>
        public virtual void OnEnable()
        {

        }

        /// <summary>
        /// Called on the frame when a Behaviour is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        public virtual void Start()
        {

        }

        /// <summary>
        /// Called every frame, if the effect is enabled.
        /// </summary>
        public abstract void OnAnimationUpdate();

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        public virtual void OnDisable()
        {

        }

        /// <summary>
        /// Called when the effect becomes removed. 
        /// </summary>
        public virtual void OnRemove()
        {

        }

        /// <summary>
        /// Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy.
        /// </summary>
        public virtual void OnDestroy()
        {

        }

        /// <summary>
        /// Instantiate hinge for processing independent effect relative parent transform.
        /// </summary>
        /// <param name="name">Name for creating hinge.</param>
        /// <param name="parent">Parent transform relative creating hinge.</param>
        /// <returns>Hinge transform reference.</returns>
        protected Transform InstantiateHinge(string name, Transform parent)
        {
            GameObject gameObject = new GameObject(name);
            Transform hinge = gameObject.transform;
            Transform[] children = new Transform[parent.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = parent.GetChild(i);
            }

            hinge.SetParent(parent);
            hinge.localPosition = Vector3.zero;
            hinge.localRotation = Quaternion.identity;
            hinge.localScale = Vector3.one;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].transform.SetParent(hinge);
            }
            return hinge;
        }
    }
}