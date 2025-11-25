/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/FSM/Other/Way Point Map")]
    [DisallowMultipleComponent]
    public sealed class WayPointMap : MonoBehaviour
    {
        [SerializeField]
        [ReorderableList(ElementLabel = null)]
        private List<Transform> wayPoints = new List<Transform>();
        
        /// <summary>
        /// Add new way point to map.
        /// </summary>
        /// <param name="wayPoint">Transform of way point.</param>
        public void Add(Transform wayPoint)
        {
            wayPoints.Add(wayPoint);
        }

        /// <summary>
        /// Remove way point from map by reference.
        /// </summary>
        /// <param name="wayPoint">Transform reference of way point.</param>
        /// <returns>True if way point transform reference contain in map. Otherwise false.</returns>
        public bool Remove(Transform wayPoint)
        {
            return wayPoints.Remove(wayPoint);
        }

        /// <summary>
        /// Remove way point from map by index.
        /// </summary>
        /// <param name="index">Index of way point.</param>
        public void Remove(int index)
        {
            wayPoints.RemoveAt(index);
        }

        /// <summary>
        /// Get way point position in world space.
        /// </summary>
        /// <param name="index">Index of position.</param>
        /// <returns></returns>
        public Vector3 GetPointPosition(int index)
        {
            return wayPoints[index].position;
        }

        #region [Getter / Setter]
        public List<Transform> GetPoints()
        {
            return wayPoints;
        }

        public void SetPoints(List<Transform> value)
        {
            wayPoints = value;
        }

        public Transform GetPoint(int index)
        {
            return wayPoints[index];
        }

        public void SetPoint(int index, Transform value)
        {
            wayPoints[index] = value;
        }

        public int GetPointCount()
        {
            return wayPoints.Count;
        }
        #endregion
    }
}