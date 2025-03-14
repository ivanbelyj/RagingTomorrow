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
        /// <summary>
        /// Используется только для установки некоторых данных в инспекторе. Во время выполнения
        /// эти данные становятся неактуальными
        /// </summary>
        [Tooltip("Порядок элементов имеет значение: на его основе определяются локальные id элементов")]
        [SerializeField]
        private List<AppearanceElement> _initialApperanceElements;

        [Header("Скелет")]
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

        [Header("Камера")]
        [SerializeField]
        private bool _hasCamera;
        /// <summary>
        /// Устанавливать ли камеру при инстанцировании кастомизируемого объекта в соответствии
        /// с параметрами, указанными в объекте типа?
        /// Это актуально, например, для кастомизируемых объектов игрока
        /// </summary>
        public bool HasCamera => _hasCamera;

        [SerializeField]
        private string _cameraBoneName = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head";
        /// <summary>
        /// Кость, относительно которой будет определяться позиция камеры.
        /// Актуально, если тип кастомизируемого объекта предполагает скелет, а в приложении
        /// требуется смотреть от определенной кости данного объекта.
        /// Имя кости включает ее положение в иерархии скелета. Например:
        /// mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head  
        /// </summary>
        public string CameraBoneName => _cameraBoneName;

        [SerializeField]
        private Vector3 _cameraOffset;
        /// <summary>
        /// Смещение камеры относительно выбранной опорной кости
        /// </summary>
        public Vector3 CameraOffset => _cameraOffset;

        private Dictionary<AppearanceElementLocalId, AppearanceElement> _appearanceElements;

        /// <summary>
        /// Локальные id и соответствующие части кастомизации, которые можно применять к кастомизируемому
        /// объекту данного типа.
        /// Словарь используется для удобства и ускорения доступа к частям по id 
        /// </summary>
        public Dictionary<AppearanceElementLocalId, AppearanceElement> AppearanceElements {
            get => _appearanceElements;
            set => _appearanceElements = value;
        }

        public void Awake() {
            _appearanceElements = new Dictionary<AppearanceElementLocalId, AppearanceElement>();
            // uint newId = 0;
            foreach (AppearanceElement item in _initialApperanceElements) {
                // item.LocalId = new AppearanceElementLocalId(newId++);
                _appearanceElements.Add(item.LocalId, item);
            }
        }
    }
}
