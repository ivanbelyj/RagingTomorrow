using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AppearanceCustomization3D {
    /// <summary>
    /// Компактные данные о кастомизируемой сущности, позволяющие воссоздать ее внешний вид
    /// </summary>
    [Serializable]
    public class AppearanceData
    {
        [SerializeField]
        private AppearanceTypeId _appearanceTypeId;
        /// <summary>
        /// Id типа кастомизации
        /// </summary>
        public AppearanceTypeId AppearanceTypeId {
            get => _appearanceTypeId;
            set => _appearanceTypeId = value;
        }

        [SerializeField]
        private List<AppearanceElementLocalId> _appearanceElementIds;
        /// <summary>
        /// Список локальных для типа кастомизации id элементов внешнего вида
        /// </summary>
        public List<AppearanceElementLocalId> AppearanceElementIds {
            get => _appearanceElementIds;
            set => _appearanceElementIds = value;
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder($"AppearanceType: {_appearanceTypeId}; Ids: ");
            foreach (var elem in AppearanceElementIds)
                res.Append(" " + elem);
            return res.ToString();
        }
    }
}
