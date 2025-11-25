/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Pattern;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using UnityEngine;
using Allocator = AuroraFPSRuntime.SystemModules.PoolContainerBase.Allocator;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Pool System/Pool Manager", 0)]
    [DisallowMultipleComponent]
    public sealed class PoolManager : Singleton<PoolManager>
    {
        [System.Serializable]
        internal class ContainerDictionary : SerializableDictionary<string, PoolContainer>
        {
            [SerializeField]
            private string[] keys;

            [SerializeField]
            private PoolContainer[] values;

            protected override string[] GetKeys()
            {
                return keys;
            }

            protected override PoolContainer[] GetValues()
            {
                return values;
            }

            protected override void SetKeys(string[] keys)
            {
                this.keys = keys;
            }

            protected override void SetValues(PoolContainer[] values)
            {
                this.values = values;
            }
        }

        [SerializeField] 
        private ContainerDictionary pool = new ContainerDictionary();

        /// <summary>
        /// Instantiate new pool container with reserved pool objects.
        /// </summary>
        /// <param name="original">PoolObject instance.</param>
        /// <param name="allocator">Container allocator type.</param>
        /// <param name="capacity">Reserved container capacity.</param>
        [System.Obsolete("Use CreateContainer instead.")]
        public void InstantiateContainer(PoolObject original, Allocator allocator, int capacity)
        {
            string id = original.GetPoolObjectID();

            GameObject root = new GameObject();
            root.name = string.Format("{0} [Container]", id);
            root.transform.SetParent(transform);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;

            PoolContainer container = new PoolContainer(original, allocator, capacity);
            for (int i = 0; i < capacity; i++)
            {
                PoolObject poolObject = Instantiate(original);
                poolObject.transform.SetParent(root.transform);
                poolObject.transform.localPosition = Vector3.zero;
                poolObject.transform.localRotation = Quaternion.identity;
                poolObject.gameObject.SetActive(false);
                container.Push(poolObject);
            }
            pool.Add(id, container);
        }

        /// <summary>
        /// Instantiate new pool container with reserved pool objects.
        /// </summary>
        /// <param name="original">PoolObject instance.</param>
        /// <param name="allocator">Container allocator type.</param>
        /// <param name="capacity">Reserved container capacity.</param>
        public void CreateContainer(PoolObject original, Allocator allocator, int capacity, Transform root)
        {
            string id = original.GetPoolObjectID();
            PoolContainer container = new PoolContainer(original, allocator, capacity, root);
            for (int i = 0; i < capacity; i++)
            {
                PoolObject poolObject = Instantiate(original, root);
                poolObject.transform.localPosition = Vector3.zero;
                poolObject.transform.localRotation = Quaternion.identity;
                poolObject.gameObject.SetActive(false);
                container.Push(poolObject);
            }
            pool.Add(id, container);
        }

        /// <summary>
        /// Instantiate new pool container with reserved pool objects.
        /// </summary>
        /// <param name="original">PoolObject instance.</param>
        /// <param name="allocator">Container allocator type.</param>
        /// <param name="capacity">Reserved container capacity.</param>
        public void CreateContainer(PoolObject original, Allocator allocator, int capacity)
        {
            string id = original.GetPoolObjectID();

            GameObject containerObject = new GameObject(string.Format("{0} [Container]", id));
            containerObject.transform.SetParent(transform);
            containerObject.transform.localPosition = Vector3.zero;
            containerObject.transform.localRotation = Quaternion.identity;
            CreateContainer(original, allocator, capacity, containerObject.transform);
        }

        /// <summary>
        /// Create or pop pool object.
        /// 
        /// If original pool object contains in pool return free pool object depending on the type of pool container allocator.
        /// 
        /// Else create new pool container by original pool object and return it.
        /// New pool container settings: [Allocator - Free], [Default size - 1]
        /// </summary>
        /// <param name="original">Original PoolObject instance.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <returns>The instantiated or poped pool object.</returns>
        public PoolObject CreateOrPop(PoolObject original, Vector3 position, Quaternion rotation, Transform parent)
        {
            PoolObject poolObject = CreateOrPop(original, position, rotation);
            if (poolObject != null)
            {
                poolObject.transform.SetParent(parent);
            }
            return poolObject;
        }

        /// <summary>
        /// Create or pop pool object.
        /// 
        /// If original pool objects contains in pool return free pool object depending on the type of pool container allocator.
        /// 
        /// Else create new pool container by original pool object and return it.
        /// New pool container settings: [Allocator - Free], [Default size - 1]
        /// </summary>
        /// <param name="original">Original PoolObject instance.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <returns>The instantiated or poped pool object.</returns>
        public PoolObject CreateOrPop(PoolObject original, Vector3 position, Quaternion rotation)
        {
            PoolObject poolObject = CreateOrPop(original);
            if (poolObject != null)
            {
                poolObject.transform.position = position;
                poolObject.transform.rotation = rotation;
            }
            return poolObject;
        }

        /// <summary>
        /// Create or pop pool object.
        /// 
        /// If original pool objects contains in pool manager return free pool object depending on the type of pool container allocator.
        /// 
        /// Else create new pool container by original pool object and return it.
        /// New pool container settings: [Allocator - Free], [Default size - 1]
        /// </summary>
        /// <param name="original">Original PoolObject instance.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <returns>The instantiated or poped pool object.</returns>
        public PoolObject CreateOrPop(PoolObject original)
        {
            if (original != null)
            {
                string id = original.GetPoolObjectID();
                if (!pool.ContainsKey(id))
                {
                    CreateContainer(original, Allocator.Free, 1);
                }

                PoolObject value = pool[id].Pop();
                return value;
            }
            return null;
        }

        /// <summary>
        /// Create or pop pool object.
        /// 
        /// If original pool object contains in pool return free pool object depending on the type of pool container allocator.
        /// 
        /// Else create new pool container by original pool object and return it.
        /// New pool container settings: [Allocator - Free], [Default size - 1]
        /// </summary>
        /// <param name="original">Original PoolObject instance.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <returns>The instantiated or poped pool object.</returns>
        public T CreateOrPop<T>(PoolObject original, Vector3 position, Quaternion rotation, Transform parent) where T : PoolObject
        {
            PoolObject poolObject = CreateOrPop(original, position, rotation);
            if (poolObject != null)
            {
                poolObject.transform.SetParent(parent);
            }
            return poolObject as T;
        }

        /// <summary>
        /// Create or pop pool object.
        /// 
        /// If original pool objects contains in pool return free pool object depending on the type of pool container allocator.
        /// 
        /// Else create new pool container by original pool object and return it.
        /// New pool container settings: [Allocator - Free], [Default size - 1]
        /// </summary>
        /// <param name="original">Original PoolObject instance.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <returns>The instantiated or poped pool object.</returns>
        public T CreateOrPop<T>(PoolObject original, Vector3 position, Quaternion rotation) where T : PoolObject
        {
            PoolObject poolObject = CreateOrPop(original);
            if (poolObject != null)
            {
                poolObject.transform.position = position;
                poolObject.transform.rotation = rotation;
            }
            return poolObject as T;
        }

        /// <summary>
        /// Create or pop pool object.
        /// 
        /// If original pool objects contains in pool manager return free pool object depending on the type of pool container allocator.
        /// 
        /// Else create new pool container by original pool object and return it.
        /// New pool container settings: [Allocator - Free], [Default size - 1]
        /// </summary>
        /// <param name="original">Original PoolObject instance.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <returns>The instantiated or poped pool object.</returns>
        public T CreateOrPop<T>(PoolObject original) where T : PoolObject
        {
            if (original != null)
            {
                string id = original.GetPoolObjectID();
                if (!pool.ContainsKey(id))
                {
                    CreateContainer(original, Allocator.Free, 1);
                }

                PoolObject value = pool[id].Pop();
                return value as T;
            }
            return null;
        }

        /// <summary>
        /// Pop pool object form pool container by pool id.
        /// </summary>
        /// <param name="id">Pool container ID (By default used name of original pool object).</param>
        /// <returns>
        /// If ID contains in pool manager: Poped pool object.
        /// Else: null.
        /// </returns>
        public PoolObject Pop(string id)
        {
            if (pool.TryGetValue(id, out PoolContainer container))
            {
                PoolObject value = container.Pop();
                return value;
            }

            return null;
        }

        /// <summary>
        /// Pop pool object form pool container by pool id.
        /// </summary>
        /// <param name="original">Original pool object.</param>
        /// <returns>
        /// If ID contains in pool manager: Poped pool object.
        /// Else: null.
        /// </returns>
        public PoolObject Pop(PoolObject original)
        {
            if (original != null)
            {
                if (pool.TryGetValue(original.GetPoolObjectID(), out PoolContainer container))
                {
                    PoolObject value = container.Pop();
                    return value;
                }
            }

            return null;
        }

        /// <summary>
        /// Push pool object to pool container.
        /// </summary>
        /// <param name="value">Gameobject to push.</param>
        public void Push(PoolObject value)
        {
            if (value != null)
            {
                if (!pool.TryGetValue(value.GetPoolObjectID(), out PoolContainer container))
                {
                    container = new PoolContainer(value, Allocator.Free, 1);
                    pool.Add(value.GetPoolObjectID(), container);
                }
                container.Push(value);
            }
        }

        /// <summary>
        /// Try to peek value from pool container by id without remove it or creating new if is container is empty.
        /// 
        /// Note: value won't be activated after peek.
        /// </summary>
        /// <param name="value">
        /// First available pool value.
        /// If the pool container is empty, the output value will be null.
        /// </param>
        /// <returns>True if pool manager contains container with this id and container contains available pool object for peek, else false.</returns>
        public bool TryPeek(string id, out PoolObject value)
        {
            if (pool.TryGetValue(id, out PoolContainer container))
            {
                return container.TryPeek(out value);
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Try to peek value from pool container by id without remove it or creating new if is container is empty.
        /// 
        /// Note: value won't be activated after peek.
        /// </summary>
        /// <param name="value">
        /// First available pool value.
        /// If the pool container is empty, the output value will be null.
        /// </param>
        /// <returns>True if pool manager contains container with this id and container contains available pool object for peek, else false.</returns>
        public bool TryPeek(PoolObject original, out PoolObject value)
        {
            if (original != null && pool.TryGetValue(original.GetPoolObjectID(), out PoolContainer container))
            {
                return container.TryPeek(out value);
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Reallocate container by id.
        /// </summary>
        /// <param name="id">Pool container id.</param>
        /// <param name="allocator">New container allocator type.</param>
        /// <param name="capacity">New container capacity.</param>
        public void ReallocateContainer(string id, Allocator allocator, int capacity)
        {
            if (pool.TryGetValue(id, out PoolContainer container))
            {
                container.Reallocate(allocator, capacity);
            }
        }

        /// <summary>
        /// Reallocate container by original PoolObject id.
        /// </summary>
        /// <param name="original">PoolObject original instance.</param>
        /// <param name="allocator">New container allocator type.</param>
        /// <param name="capacity">New container capacity.</param>
        public void ReallocateContainer(PoolObject original, Allocator allocator, int capacity)
        {
            if (original != null)
            {
                string id = original.GetPoolObjectID();
                if (pool.TryGetValue(id, out PoolContainer container))
                {
                    container.Reallocate(allocator, capacity);
                }
            }
        }

        /// <summary>
        /// Trying to remove a pool container with all the objects it contains from pool.
        /// </summary>
        /// <param name="id">Pool container id.</param>
        /// <returns>
        /// True if container contains in pool.
        /// Else false.
        /// </returns>
        public bool RemoveContainer(string id)
        {
            if (pool.TryGetValue(id, out PoolContainer container))
            {
                foreach (PoolObject poolObject in container.GetObjectsStack())
                {
#if UNITY_EDITOR
                    DestroyImmediate(poolObject);
#else
                    Destroy(poolObject);
#endif
                }
                container = null;
                pool.Remove(id);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Trying to remove a pool container with all the objects it contains from pool.
        /// </summary>
        /// <param name="original">Original PoolObject instance.</param>
        /// <returns>
        /// True if original pool object not null & container contains in pool.
        /// Else false.
        /// </returns>
        public bool RemoveContainer(PoolObject original)
        {
            if (original != null)
            {
                string id = original.GetPoolObjectID();
                if (pool.TryGetValue(id, out PoolContainer container))
                {
                    foreach (PoolObject poolObject in container.GetObjectsStack())
                    {
#if UNITY_EDITOR
                        DestroyImmediate(poolObject);
#else
                    Destroy(poolObject);
#endif
                    }
                    container = null;
                    pool.Remove(id);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checking that the pool container with the specified id is contained in the pool.
        /// </summary>
        /// <param name="id">Pool container id.</param>
        /// <returns>
        /// True if container contains in pool.
        /// Else false.
        /// </returns>
        public bool ContainsContainer(string id)
        {
            return pool.ContainsKey(id);
        }

        /// <summary>
        /// Checking that the pool container with the original PoolObject is contained in the pool.
        /// </summary>
        /// <param name="original">Original PoolObject instance.</param>
        /// <returns>
        /// True if original pool object not null & container contains in pool.
        /// Else false.
        /// </returns>
        public bool ContainsContainer(PoolObject original)
        {
            if (original != null)
            {
                return pool.ContainsKey(original.GetPoolObjectID());
            }
            return false;
        }

        /// <summary>
        /// Get pool container from pool manager.
        /// </summary>
        /// <param name="id">Pool container id.</param>
        /// <returns>
        /// If pool container contains in pool manager return container.
        /// Else return null.
        /// </returns>
        public PoolContainer GetPoolContainer(string id)
        {
            pool.TryGetValue(id, out PoolContainer container);
            return container;
        }

        /// <summary>
        /// Try to get container from pool by id.
        /// </summary>
        /// <param name="id">Unique container id.</param>
        /// <param name="container">Container reference.</param>
        /// <returns>True if pool contains container with this id, otherwise false.</returns>
        public bool TryGetPoolContainer(string id, out PoolContainer container)
        {
            return pool.TryGetValue(id, out container);
        }

        /// <summary>
        /// Try to get container from pool by id.
        /// </summary>
        /// <param name="original">Original pool object of container.</param>
        /// <param name="container">Container reference.</param>
        /// <returns>True if pool contains container with this id, otherwise false.</returns>
        public bool TryGetPoolContainer(PoolObject original, out PoolContainer container)
        {
            if(original != null)
            {
                return pool.TryGetValue(original.GetPoolObjectID(), out container);
            }
            container = null;
            return false;
        }

        /// <summary>
        /// Get pool container from pool manager.
        /// </summary>
        /// <param name="original">Original pool object of container.</param>
        /// <returns>
        /// If pool container contains in pool manager return container.
        /// Else return null.
        /// </returns>
        public PoolContainer GetPoolContainer(PoolObject original)
        {
            if (original != null)
            {
                pool.TryGetValue(original.GetPoolObjectID(), out PoolContainer container);
                return container;
            }
            return null;
        }

        #region [Editor Section]
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Aurora FPS Engine/Create/World/Pool Manager", false, 111)]
        [UnityEditor.MenuItem("GameObject/Aurora FPS Engine/World/Pool Manager", false, 10)]
        private static void CreatePoolManager()
        {
            PoolManager poolManager = PoolManager.GetRuntimeInstance();
            UnityEditor.Selection.activeObject = poolManager;
            UnityEditor.EditorGUIUtility.PingObject(poolManager);
        }
#endif
#endregion

        #region [Getter / Setter]
        public int ContainerCount()
        {
            return pool.Count;
        }
        #endregion
    }
}