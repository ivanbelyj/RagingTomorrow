/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [System.Serializable]
    public class FootstepSoundDictionary : SerializableDictionary<Object, FootstepSoundStorage>
    {
        [SerializeField]
        private Object[] keys;

        [SerializeField]
        private FootstepSoundStorage[] values;

        protected override Object[] GetKeys()
        {
            return keys;
        }

        protected override FootstepSoundStorage[] GetValues()
        {
            return values;
        }

        protected override void SetKeys(Object[] keys)
        {
            this.keys = keys;
        }

        protected override void SetValues(FootstepSoundStorage[] values)
        {
            this.values = values;
        }
    }

    [CreateAssetMenu(fileName = "Footstep Mapping", menuName = "Aurora FPS Engine/Controller/Footstep Mapping", order = 55)]
    public sealed class FootstepSoundMapping : ScriptableMappingDictionary<FootstepSoundDictionary, Object, FootstepSoundStorage> { }
}