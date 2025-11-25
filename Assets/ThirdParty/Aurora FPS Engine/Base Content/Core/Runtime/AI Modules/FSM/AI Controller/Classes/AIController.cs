/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.Behaviour;
using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class AIController : MonoBehaviour, IControllerVelocity, IControllerOrientation, IControllerDestination, IControllerMovement, IBehaviourCollections
    {
        [Serializable]
        public class Behaviours : SerializableDictionary<string, AIBehaviour>
        {
            [SerializeField]
            private string[] keys;

            [SerializeReference]
            private AIBehaviour[] values;

            protected override string[] GetKeys()
            {
                return keys;
            }

            protected override AIBehaviour[] GetValues()
            {
                return values;
            }

            protected override void SetKeys(string[] keys)
            {
                this.keys = keys;
            }

            protected override void SetValues(AIBehaviour[] values)
            {
                this.values = values;
            }
        }

        [SerializeField]
        [ValueDropdown("GetBehaviourNames")]
        private string startBehaviour = "Idle";

        [SerializeField]
        private Behaviours behaviours = new Behaviours()
        {
            ["Idle"] = new AIIdleBehaviour()
        };

        // Stored required properties.
        private string activeBehaviourName;
        private AIBehaviour activeBehaviour;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            RegisterBehaviours();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled 
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            SwitchToStartBehaviour();
        }

        public void SwitchToStartBehaviour()
        {
            if (TryGetBehaviour(startBehaviour, out AIBehaviour behaviour))
            {
                activeBehaviourName = startBehaviour;
                activeBehaviour = behaviour;
                activeBehaviour.Internal_EnableBehaviour();
            }
        }

        /// <summary>
        /// Called every frame, while the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (enabled)
            {
                activeBehaviour.Internal_UpdateBehaviour();
            }
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (enabled)
            {
                activeBehaviour.Internal_CheckTrasition();
            }
        }

        /// <summary>
        /// Called when the AIController instance is being loaded. 
        /// </summary>
        protected void RegisterBehaviours()
        {
            foreach (string name in GetBehaviourNames())
            {
                behaviours[name].RegisterBehaviour(this);
            }
        }

        /// <summary>
        /// Switch current behaviour name to other, without considering transitions.
        /// </summary>
        /// <param name="name">Name of the behaviour in AIController.</param>
        public void SwitchBehaviour(string name)
        {
            if (TryGetBehaviour(name, out AIBehaviour behaviour))
            {
                activeBehaviour.Internal_DisableBehaviour();
                activeBehaviourName = name;
                activeBehaviour = behaviour;
                activeBehaviour.Internal_EnableBehaviour();
            }
        }

        /// <summary>
        /// Gets the behaviour instance associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the behaviour instance to get.</param>
        /// <param name="behaviour">
        /// When this method returns, contains the behaviour instance associated with the specified name, if the name is found.
        /// Otherwise, the default behaviour instance for the type of the behaviour instance parameter. 
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if contains an behaviour instance with the specified name.
        /// Otherwise, false.
        /// </returns>
        public bool TryGetBehaviour(string name, out AIBehaviour behaviour)
        {
            return behaviours.TryGetValue(name, out behaviour);
        }

        public virtual void Sleep(bool value)
        {
            if (value)
                activeBehaviour.Internal_DisableBehaviour();
            else
                activeBehaviour.Internal_EnableBehaviour();
            enabled = !value;
        }

        #region [IControllerVelocity Implementation]
        /// <summary>
        /// Controller velocity in Vector3 representation.
        /// </summary>
        public abstract Vector3 GetVelocity();
        #endregion

        #region [IControllerOrientation Implementation]
        public abstract void UpdateOrientation(bool value);
        #endregion

        #region [IControllerDestination Implementation]
        /// <summary>
        /// Set controller destination.
        /// </summary>
        /// <param name="position">Position in wolrd space.</param>
        public abstract void SetDestination(Vector3 position);

        /// <summary>
        /// Return true if controller reach current destination. Otherwise false.
        /// </summary>
        public abstract bool IsReachDestination();
        #endregion

        #region [IControllerMovement Implementation]
        /// <summary>
        /// Resume or stop controller movement.
        /// </summary>
        /// <param name="value">Set true to resume moving or false to stop.</param>
        public abstract void IsMoving(bool value);
        #endregion

        #region [IBehaviourCollections Implementation]
        /// <summary>
        /// Iterate all initialized behaviour names.
        /// </summary>
        public IEnumerable<string> GetBehaviourNames()
        {
            foreach (KeyValuePair<string, AIBehaviour> item in behaviours)
            {
                yield return item.Key;
            }
        }

        /// <summary>
        /// Iterate all initialized behaviour instances.
        /// </summary>
        public IEnumerable<AIBehaviour> GetBehaviours()
        {
            foreach (KeyValuePair<string, AIBehaviour> item in behaviours)
            {
                yield return item.Value;
            }
        }
        #endregion

        #region [Getter / Setter]
        public string GetStartBehaviour()
        {
            return startBehaviour;
        }

        public void SetStartBehaviour(string behaviour)
        {
            startBehaviour = behaviour;
        }

        public string GetActiveBehaviour()
        {
            return activeBehaviourName;
        }
        #endregion
    }
}