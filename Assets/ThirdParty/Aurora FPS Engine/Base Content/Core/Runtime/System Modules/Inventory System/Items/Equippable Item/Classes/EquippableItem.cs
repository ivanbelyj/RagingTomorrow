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

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    [HideScriptField]
    [CreateAssetMenu(fileName = "New Equippable Item", menuName = "Aurora FPS Engine/Inventory/Equippable Item", order = 91)]
    [ComponentIcon("Equippable Item")]
    public class EquippableItem : InventoryItem
    {
        [SerializeField]
        [Label("Reference")]
        [NotNull]
        [Foldout("First Person Settings", Style = "Header")]
        private GameObject firstPersonObject;

        [SerializeField]
        [Label("Position")]
        [Foldout("First Person Settings", Style = "Header")]
        private Vector3 firstPersonPosition = Vector3.one;

        [SerializeField]
        [Label("Rotation")]
        [Foldout("First Person Settings", Style = "Header")]
        private Vector3 firstPersonRotation = Vector3.one;

        [SerializeField]
        [Label("Scale")]
        [Foldout("First Person Settings", Style = "Header")]
        private Vector3 firstPersonScale = Vector3.one;

        [SerializeField]
        [MinValue(0.0f)]
        [Foldout("Equip Settings", Style = "Header")]
        [Order(252)]
        private float selectTime = 0.1f;

        [SerializeField]
        [MinValue(0.0f)]
        [Foldout("Equip Settings", Style = "Header")]
        [Order(253)]
        private float hideTime = 0.1f;

        #region [Getter / Setter]
        public GameObject GetFirstPersonObject()
        {
            return firstPersonObject;
        }

        public void SetFirstPersonObject(GameObject value)
        {
            firstPersonObject = value;
        }

        public Vector3 GetFirstPersonPosition()
        {
            return firstPersonPosition;
        }

        public void SetFirstPersonPosition(Vector3 value)
        {
            firstPersonPosition = value;
        }

        public Vector3 GetFirstPersonRotation()
        {
            return firstPersonRotation;
        }

        public void SetFirstPersonRotation(Vector3 value)
        {
            firstPersonRotation = value;
        }

        public Vector3 GetFirstPersonScale()
        {
            return firstPersonScale;
        }

        public void SetFirstPersonScale(Vector3 value)
        {
            firstPersonScale = value;
        }

        public float GetSelectTime()
        {
            return selectTime;
        }

        public void SetSelectTime(float value)
        {
            selectTime = value;
        }

        public float GetHideTime()
        {
            return hideTime;
        }

        public void SetHideTime(float value)
        {
            hideTime = value;
        }
        #endregion
    }
}