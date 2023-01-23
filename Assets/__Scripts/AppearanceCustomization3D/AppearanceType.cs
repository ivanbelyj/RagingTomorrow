using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppearanceCustomization3D {
    /// <summary>
    /// Каждый кастомизируемый объект принадлежит к определенному типу. Тип определяет,
    /// какие части могут быть применены к объекту для кастомизации, какие - нет
    /// </summary>
    [CreateAssetMenu(fileName = "New AppearanceType", menuName = "Appearance Type/Appearance Type", order = 52)]
    public class AppearanceType : ScriptableObject
    {
        [SerializeField]
        private List<AppearanceElement> _initialApperanceElements;

        private Dictionary<int, AppearanceElement> _appearanceElements;

        [SerializeField]
        private bool _hasRig;
        public bool HasRig => _hasRig;

        [SerializeField]
        private GameObject _bonesAndArmatureHolder;
        /// <summary>
        /// Для каждого типа кастомизируемых объектов, имеющих скелет, должен быть установлен
        /// префаб, инстанцируемый вместе с каждым новым экземпляром кастомизируемого объекта,
        /// предоставляющий дочерний Armature (содержит иерархию костей)
        /// и BonesHolder, к которому присоединен SkinnedMeshRenderer с уже назначенным
        /// свойством bones (будет назначаться SkinnedMeshRenderer новых элементов)
        /// </summary>
        public GameObject BonesAndArmatureHolder => _bonesAndArmatureHolder;

        /// <summary>
        /// Локальные id и соответствующие части кастомизации, которые можно применять к кастомизируемому
        /// объекту данного типа.
        /// Словарь используется для удобства и ускорения доступа к частям по id 
        /// </summary>
        public Dictionary<int, AppearanceElement> AppearanceElements {
            get => _appearanceElements;
            set => _appearanceElements = value;
        }

        public void Awake() {
            _appearanceElements = new Dictionary<int, AppearanceElement>();
            foreach (var item in _initialApperanceElements) {
                _appearanceElements.Add(item.LocalId, item);
            }
        }
    }
}
