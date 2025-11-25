/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    [HideScriptField]
    [CreateAssetMenu(fileName = "New Inventory Item", menuName = "Aurora FPS Engine/Inventory/Item", order = 90)]
    [ComponentIcon("Inventory Item")]
    public class InventoryItem : BaseItem
    {
        [SerializeField]
        [Label("Type")]
        [Tooltip("What type does this item belong to?\n(Optional)")]
        [Order(-998)]
        private string itemType;

        [SerializeField]
        [Label("Reference")]
        [Foldout("Drop Settings", Style = "Header")]
        [Order(201)]
        private PoolObject dropObject;

        [SerializeField]
        [Label("Rotation")]
        [Foldout("Drop Settings", Style = "Header")]
        [Order(201)]
        private Vector3 dropRotation;

        [SerializeField]
        [Label("Scale")]
        [Foldout("Drop Settings", Style = "Header")]
        [Order(201)]
        private Vector3 dropScale = Vector3.one;

        [SerializeField]
        [Label("Forward Multiplier")]
        [Foldout("Drop Settings", Style = "Header")]
        [Order(201)]
        private float dropForwardMultiplier;

        [SerializeField]
        [Label("Force")]
        [Foldout("Drop Settings", Style = "Header")]
        [Order(201)]
        private float dropForce;

        [SerializeField]
        [Label("Torque")]
        [Foldout("Drop Settings", Style = "Header")]
        [Order(201)]
        private float dropTorque;

        [SerializeField]
        [Foldout("Inventory Settings", Style = "Header")]
        [Tooltip("Can this item be reused?\nTrue: can be used an unlimited number of times.\nFalse: Will be removed after first use.")]
        [Order(301)]
        private bool reusable = false;

        [SerializeField]
        [Foldout("UI Settings", Style = "Header")]
        [Order(400)]
        private Sprite itemImage;

        [SerializeField]
        [Foldout("Custom Data", Style = "Header")]
        [Order(492)]
        private CustomDatas customDatas;

        [SerializeField]
        [ReorderableList(DisplayHeader = false, DisplaySeparator = true)]
        [Foldout("Event Settings", Style = "Header")]
        [Tooltip("Use function event settings. Will be called when this item selected.")]
        [Order(492)]
        private CustomEvent[] functions;

        public bool CompareType(InventoryItem item)
        {
            return item.GetItemType() == itemType;
        }

        #region [Internal Method]
        internal void InitializeEventFunctions(Component component)
        {
            for (int i = 0; i < functions.Length; i++)
            {
                functions[i].Initialize(component);
            }
        }

        internal void InvokeEventFunctions()
        {
            for (int i = 0; i < functions.Length; i++)
            {
                functions[i].Invoke();
            }
        }
        #endregion

        #region [Getter / Setter]
        public string GetItemType()
        {
            return itemType;
        }

        public void SetItemType(string value)
        {
            itemType = value;
        }

        public PoolObject GetDropObject()
        {
            return dropObject;
        }

        public void SetDropObject(PoolObject value)
        {
            dropObject = value;
        }

        public Vector3 GetDropRotation()
        {
            return dropRotation;
        }

        public void SetDropRotation(Vector3 value)
        {
            dropRotation = value;
        }

        public Vector3 GetDropScale()
        {
            return dropScale;
        }

        public void SetDropScale(Vector3 value)
        {
            dropScale = value;
        }

        public float GetDropForwardMultiplier()
        {
            return dropForwardMultiplier;
        }

        public void SetDropForwardMultiplier(float value)
        {
            dropForwardMultiplier = value;
        }

        public float GetDropForce()
        {
            return dropForce;
        }

        public void SetDropForce(float value)
        {
            dropForce = value;
        }

        public float GetDropTorque()
        {
            return dropTorque;
        }

        public void SetDropTorque(float value)
        {
            dropTorque = value;
        }

        public bool Reusable()
        {
            return reusable;
        }

        public void Reusable(bool value)
        {
            reusable = value;
        }

        public Sprite GetItemImage()
        {
            return itemImage;
        }

        public void SetItemImage(Sprite value)
        {
            itemImage = value;
        }

        public bool TryGetCustomData(string key, CustomValue.ValueType type, out object value)
        {
            if(customDatas.TryGetValue(key, out CustomValue customValue) && customValue.valueType == type)
            {
                switch (type)
                {
                    case CustomValue.ValueType.Integer:
                        value = System.Convert.ToInt32(customValue.numberValue);
                        return true;
                    case CustomValue.ValueType.Float:
                        value = customValue.numberValue;
                        return true;
                    case CustomValue.ValueType.String:
                        value = customValue.stringValue;
                        return true;
                    case CustomValue.ValueType.Bool:
                        value = System.Convert.ToBoolean(customValue.numberValue);
                        return true;
                    case CustomValue.ValueType.Vector2:
                        value = new Vector2(customValue.axesValue.x, customValue.axesValue.y);
                        return true;
                    case CustomValue.ValueType.Vector3:
                        value = new Vector3(customValue.axesValue.x, customValue.axesValue.y, customValue.axesValue.z);
                        return true;
                    case CustomValue.ValueType.Quaternion:
                        value = customValue.axesValue;
                        return true;
                    case CustomValue.ValueType.Object:
                        value = customValue.objectValue;
                        return true;
                }
            }
            value = null;
            return false;
        }

        public bool TrySetCustomData(string key, object value)
        {
            if(customDatas.TryGetValue(key, out CustomValue outputValue))
            {
                outputValue.numberValue = CustomValue.DefalutNumber;
                outputValue.stringValue = CustomValue.DefalutString;
                outputValue.axesValue = CustomValue.DefalutAxes;
                outputValue.objectValue = CustomValue.DefalutObject;
                switch (outputValue.valueType)
                {
                    case CustomValue.ValueType.Integer:
                        outputValue.numberValue = System.Convert.ToInt32(value);
                        break;
                    case CustomValue.ValueType.Float:
                        outputValue.numberValue = System.Convert.ToSingle(value);
                        break;
                    case CustomValue.ValueType.String:
                        outputValue.stringValue = System.Convert.ToString(value);
                        break;
                    case CustomValue.ValueType.Bool:
                        outputValue.numberValue = System.Convert.ToSingle(value);
                        break;
                    case CustomValue.ValueType.Vector2:
                        Vector2 vector2 = (Vector2)value;
                        outputValue.axesValue = new Quaternion(vector2.x, vector2.y, 0, 0);
                        break;
                    case CustomValue.ValueType.Vector3:
                        Vector3 vector3 = (Vector3)value;
                        outputValue.axesValue = new Quaternion(vector3.x, vector3.y, vector3.z, 0);
                        break;
                    case CustomValue.ValueType.Quaternion:
                        outputValue.axesValue = (Quaternion)value;
                        break;
                    case CustomValue.ValueType.Object:
                        outputValue.objectValue = (Object)value;
                        break;
                }
                return true;
            }
            return false;
        }

        public bool AddCustomData(string key, CustomValue.ValueType type, object value)
        {
            if (!customDatas.ContainsKey(key))
            {
                CustomValue customValue = new CustomValue();
                customValue.valueType = type;
                customValue.numberValue = CustomValue.DefalutNumber;
                customValue.stringValue = CustomValue.DefalutString;
                customValue.axesValue = CustomValue.DefalutAxes;
                customValue.objectValue = CustomValue.DefalutObject;
                switch (customValue.valueType)
                {
                    case CustomValue.ValueType.Integer:
                        customValue.numberValue = System.Convert.ToInt32(value);
                        break;
                    case CustomValue.ValueType.Float:
                        customValue.numberValue = System.Convert.ToSingle(value);
                        break;
                    case CustomValue.ValueType.String:
                        customValue.stringValue = System.Convert.ToString(value);
                        break;
                    case CustomValue.ValueType.Bool:
                        customValue.numberValue = System.Convert.ToSingle(value);
                        break;
                    case CustomValue.ValueType.Vector2:
                        Vector2 vector2 = (Vector2)value;
                        customValue.axesValue = new Quaternion(vector2.x, vector2.y, 0, 0);
                        break;
                    case CustomValue.ValueType.Vector3:
                        Vector3 vector3 = (Vector3)value;
                        customValue.axesValue = new Quaternion(vector3.x, vector3.y, vector3.z, 0);
                        break;
                    case CustomValue.ValueType.Quaternion:
                        customValue.axesValue = (Quaternion)value;
                        break;
                    case CustomValue.ValueType.Object:
                        customValue.objectValue = (Object)value;
                        break;
                }
                customDatas.Add(key, customValue);
                return true;
            }
            return false;
        }

        public bool RemoveCustomData(string key)
        {
            return customDatas.Remove(key);
        }

        public bool ContainsCustomData(string key)
        {
            return customDatas.ContainsKey(key);
        }

        public CustomEvent[] GetEventFunction()
        {
            return functions;
        }

        public void SetEventFunction(CustomEvent[] value)
        {
            functions = value;
        }
        #endregion
    }
}