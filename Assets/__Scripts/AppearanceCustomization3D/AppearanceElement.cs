using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppearanceCustomization3D {
    /// <summary>
    /// Часть кастомизируемого объекта
    /// </summary>
    [System.Serializable]
    public class AppearanceElement
    {
        /// <summary>
        /// Локальный для типа кастомизируемого объекта id части
        /// </summary>
        [SerializeField]
        private int _localId;
        public int LocalId { get => _localId; set => _localId = value; }
        
        [SerializeField]
        private GameObject _prefab;
        public GameObject Prefab { get => _prefab; set => _prefab = value; }

        [SerializeField]
        private bool _isStatic;

        /// <summary>
        /// Части могут быть помечены статичными. Статичные части либо не меняются,
        ///  либо теряются навсегда. Например, для персонажей это части тела, данные природой.
        /// Это позволяет не хранить в памяти в выключенном состоянии все возможные
        /// варианты частей тела, которые не были выбраны (и не будут),
        /// а также потерянные в бою (их можно только динамически заменить протезом)
        /// </summary>
        public bool IsStatic { get => _isStatic; set => _isStatic = value; }
    }
}
