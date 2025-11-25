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
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class EquippableObjectAnimationSystem : MonoBehaviour, IEquippableAnimation
    {
        [SerializeField]
        [ReorderableList(ElementLabel = null)]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(999)]
        private Behaviour[] behaviours;

        /// <summary>
        /// Play pull weapon in inventory animation clip.
        /// </summary>
        public abstract void PlayPullAnimation();

        /// <summary>
        /// Play push weapon in inventory animation clip.
        /// </summary>
        public abstract void PlayPushAnimation();

        public Behaviour[] GetAllBehaviours()
        {
            return behaviours;
        }

        public void SetBehavioursRange(Behaviour[] behaviours)
        {
            this.behaviours = behaviours;
        }

        public void EnableBehaviours()
        {
            for (int i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].enabled = true;
            }
        }

        public void DisableBehaviours()
        {
            for (int i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].enabled = false;
            }
        }
    }
}