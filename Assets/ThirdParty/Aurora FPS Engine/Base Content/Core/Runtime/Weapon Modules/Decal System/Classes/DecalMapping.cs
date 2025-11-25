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
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime
{
    /// <summary>
    /// Represents base class for serialized Dictionary(Object, PoolObject[]).
    /// </summary>
    [System.Serializable]
    public class DecalDictionary : SerializableDictionary<Object, PoolObjectStorage>
    {
        [SerializeField]
        private Object[] keys;

        [SerializeField]
        private PoolObjectStorage[] values;

        protected override Object[] GetKeys()
        {
            return keys;
        }

        protected override PoolObjectStorage[] GetValues()
        {
            return values;
        }

        protected override void SetKeys(Object[] keys)
        {
            this.keys = keys;
        }

        protected override void SetValues(PoolObjectStorage[] values)
        {
            this.values = values;
        }
    }

    [System.Serializable]
    public class PoolObjectStorage : SerializationStorage<PoolObject[]> { }

    [CreateAssetMenu(fileName = "Decal Mapping", menuName = "Aurora FPS Engine/Weapon/Bullet/Decal Mapping", order = 120)]
    public class DecalMapping : ScriptableMappingDictionary<DecalDictionary, Object, PoolObjectStorage> { }
}