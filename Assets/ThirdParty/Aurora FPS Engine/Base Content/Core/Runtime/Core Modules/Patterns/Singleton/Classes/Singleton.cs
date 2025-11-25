/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.Pattern
{
    /// <summary>
    /// It's a generating pattern that guarantees the existence of only one object of a certain class, 
    /// and also allows you to reach this object from anywhere in the program.
    /// 
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    /// <typeparam name="TMono">Certain class that implemented of MonoBehaviour.</typeparam>
    public abstract class Singleton<TMono> : MonoBehaviour where TMono : MonoBehaviour
    {
        private static TMono instance;
        private static object lockInstance = new object();
        private static bool applicationIsQuitting = false;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            instance = GetComponent<TMono>();
            if(transform == transform.root)
            {
                DontDestroyOnLoad(this);
            }
        }

        /// <summary>
        /// Runtime instance reference of TMono.
        /// </summary>
        public static TMono GetRuntimeInstance()
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(TMono) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (lockInstance)
            {
                if (instance == null)
                {
#if UNITY_2022_2_OR_NEWER
                    TMono[] instances = FindObjectsByType<TMono>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
                    TMono[] instances = FindObjectsOfType<TMono>();
#endif

                    if (instances != null && instances.Length > 0)
                    {
                        instance = instances[0];

                        if (instances.Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return instance;
                        }
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<TMono>();
                        singleton.name = string.Format("[Singleton] - {0}", typeof(TMono).Name);
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// 
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
    }
}