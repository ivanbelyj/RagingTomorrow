/* ================================================================
 ----------------------------------------------------------------
 Project   :   Aurora FPS Engine
 Publisher :   Renowned Games
 Developer :   Tamerlan Shakirov
 ----------------------------------------------------------------
 Copyright 2022 Renowned Games All rights reserved.
 ================================================================ */

using AuroraFPSRuntime.CoreModules.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AuroraFPSRuntime.CoreModules
{
    public static class AuroraExtension
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        #region [Reflection]
        /// <summary>
        /// Find subclasses of specific class.
        /// </summary>
        /// <param name="directDescendants">True: Find only direct descendants classes. False: Find all ssubclasses of specific class. </param>
        public static IEnumerable<Type> FindSubclassesOf<T>(bool directDescendants = false)
        {
            Assembly assembly = typeof(T).Assembly;
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => directDescendants ? t.BaseType == typeof(T) : t.IsSubclassOf(typeof(T)));
            return subclasses;
        }

        /// <summary>
        /// Find subclasses of specific class.
        /// </summary>
        /// <param name="directDescendants">True: Find only direct descendants classes. False: Find all ssubclasses of specific class. </param>
        public static IEnumerable<Type> FindSubclassesOf<T>(Assembly assembly, bool directDescendants = false)
        {
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => directDescendants ? t.BaseType == typeof(T) : t.IsSubclassOf(typeof(T)));
            return subclasses;
        }

        public static T GetAttribute<T>(Type target) where T : Attribute
        {
            // Get instance of the attribute.
            return (T)Attribute.GetCustomAttribute(target, typeof(T));
        }

        public static T[] GetAttributes<T>(Type target) where T : Attribute
        {
            // Get instance of the attribute.
            return (T[])Attribute.GetCustomAttributes(target, typeof(T));
        }
        #endregion

        #region [Extension Methods]
        /// <summary>
        /// Rotates the transform so the forward vector points at target's current position.
        /// </summary>
        /// <param name="target">Object to point towards.</param>
        /// <param name="smooth">Smooth values.</param>
        public static void LookAt(this Transform referenes, Transform target, float smooth)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - referenes.position);
            referenes.rotation = Quaternion.Slerp(referenes.rotation, targetRotation, smooth * Time.deltaTime);
        }

        /// <summary>
        /// Freeze specific rotation axis of transform.
        /// </summary>
        /// <param name="axis">Axis type.</param>
        /// <param name="value">Freeze value.</param>
        public static void FreezeRotation(this Transform referenes, Axis axis, float value)
        {
            Vector3 localEulerAngles = referenes.localEulerAngles;
            switch (axis)
            {
                case Axis.X:
                    localEulerAngles.x = value;
                    break;
                case Axis.Y:
                    localEulerAngles.y = value;
                    break;
                case Axis.Z:
                    localEulerAngles.z = value;
                    break;
            }
            referenes.localEulerAngles = localEulerAngles;
        }

        /// <summary>
        /// Freeze specific position axis of transform.
        /// </summary>
        /// <param name="axis">Axis type.</param>
        /// <param name="value">Freeze value.</param>
        public static void FreezePosition(this Transform referenes, Axis axis, float value)
        {
            Vector3 localPosition = referenes.localPosition;
            switch (axis)
            {
                case Axis.X:
                    localPosition.x = value;
                    break;
                case Axis.Y:
                    localPosition.y = value;
                    break;
                case Axis.Z:
                    localPosition.z = value;
                    break;
            }
            referenes.localPosition = localPosition;
        }

        /// <summary>
        /// Change all rigidbodys kinematic state of current transfrom
        /// </summary>
        public static void SetKinematic(this Transform transform, bool value)
        {
            Rigidbody[] rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
            for (int i = 1; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].isKinematic = value;
            }
        }

        /// <summary>
        /// Set layer for transform and transform child.
        /// </summary>
        public static void SetLayerRecursively(this Transform target, int layer)
        {
            target.gameObject.layer = layer;
            for (int i = 0, length = target.childCount; i < length; i++)
            {
                Transform child = target.GetChild(i);
                child.gameObject.layer = layer;
                SetLayerRecursively(child, layer);
            }
        }

        public static void CrossFadeInFixedTime(this Animator animator, AnimatorState animatorState)
        {
            animator.CrossFadeInFixedTime(animatorState.GetNameHash(), animatorState.GetFixedTime(), animatorState.GetLayer());
        }

        public static LayerMask ExcludeLayer(this LayerMask mask, int layerToRemove)
        {
             return mask & ~(1 << layerToRemove);
        }

        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
        #endregion

        /// <summary>
        /// Set layer for gameobjects
        /// </summary>
        public static void SetLayer(GameObject[] gameObjects, int layer)
        {
            for (int i = 0, length = gameObjects.Length; i < length; i++)
            {
                gameObjects[i].layer = layer;
            }
        }

        /// <summary>
        /// Generate text id.
        /// </summary>
        public static string GenerateID(int length = 32)
        {
            string id = string.Empty;

            do
                id += Guid.NewGuid().ToString();
            while (id.Length < length);

            if (id.Length > length)
                id = id.Substring(0, length);

            id = id.Replace("-", "");
            return id;
        }

        public static string GetPath(this Transform current)
        {
            if (current.parent == null)
                return current.name;
            return current.parent.GetPath() + "/" + current.name;
        }
    }
}