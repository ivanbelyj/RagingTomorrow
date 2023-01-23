using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppearanceCustomization3D {
    /// <summary>
    /// Компактные данные о кастомизируемой сущности
    /// </summary>
    [Serializable]
    public class AppearanceData
    {
        [SerializeField]
        private string _appearanceTypeId;
        public string AppearanceTypeId { get => _appearanceTypeId; set => _appearanceTypeId = value; }

        [SerializeField]
        private List<int> _appearanceElementIds;
        public List<int> AppearanceElementIds {
            get => _appearanceElementIds;
            set => _appearanceElementIds = value;
        }
    }
}
