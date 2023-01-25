using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AppearanceCustomization3D {
    public class CustomizableAppearance : MonoBehaviour
    {
        [SerializeField]
        private string _appearanceTypeId;

        [SerializeField]
        private AppearanceData _initialAppearanceData;
        private AppearanceTypesManager _appearanceTypesManager;
        private AppearanceOccupancy _occupancy;

        /// <summary>
        /// GameObject'ы, представляющие элементы кастомизации,
        /// которые могут активироваться / деактивироваться во время игры,
        /// доступные по local id элемента кастомизации
        /// </summary>
        private Dictionary<uint, GameObject> _nonStaticElements;

        private void Awake() {
            _appearanceTypesManager = FindObjectOfType<AppearanceTypesManager>();
            _occupancy = new AppearanceOccupancy();
            _occupancy.Initialize();
            _nonStaticElements = new Dictionary<uint, GameObject>();
        }

        private void Start() {
            InstantiateByAppearanceData(_initialAppearanceData);
        }

        public void Initialize(AppearanceData initialAppearanceData) {
            _initialAppearanceData = initialAppearanceData;
        }

        public void InstantiateByAppearanceData(AppearanceData data) {
            // Todo: network destroy old appearance

            // Какого типа создать кастомизируемый объект?
            AppearanceType appearanceType = _appearanceTypesManager
                .AppearanceTypes[this._appearanceTypeId];
            
            HashSet<uint> dataElemIdsSet = new HashSet<uint>();
            foreach (uint elemId in data.AppearanceElementIds) {
                dataElemIdsSet.Add(elemId);
            }

            Transform[] bones = null;
            GameObject armature = null;
            if (appearanceType.HasRig) {
                // Создание GameObject со скелетом и получение костей для дальнейшей установки 
                GameObject bonesAndArmatureHolder = Instantiate(appearanceType.BonesAndArmatureHolder);
                bonesAndArmatureHolder.transform.SetParent(transform);
                bonesAndArmatureHolder.transform.localPosition = Vector3.zero;
                GetBonesAndArmature(bonesAndArmatureHolder, out bones, out armature);
            }

            // Созданные части для красоты под одним объектом
            Transform appearanceElementsRoot = new GameObject("AppearanceElements").transform;
            appearanceElementsRoot.SetParent(transform);
            
            foreach (AppearanceElement element in appearanceType.AppearanceElements.Values) {
                // Создаем нестатическое (например, одежду, броню, что может потребоваться потом),
                // а также статическое, что может быть разве что потеряно (например, глаза, голову, бедра)
                if (!element.IsStatic || dataElemIdsSet.Contains(element.LocalId)) {
                    Debug.Log("Создание элемента кастомизации с local id " + element.LocalId);
                    GameObject elemGO = Instantiate(element.Prefab);
                    elemGO.transform.SetParent(appearanceElementsRoot);
                    if (appearanceType.HasRig) {
                        elemGO.GetComponent<SkinnedMeshRenderer>().bones = bones;
                    }
                    // Todo: network spawn

                    // Например, нужная одежда, броня, оружие будут установлены извне
                    if (!element.IsStatic) {
                        _nonStaticElements[element.LocalId] = elemGO;
                        elemGO.SetActive(false);
                    }
                }
            }
        }

        private void GetBonesAndArmature(GameObject bonesAndArmatureHolder,
            out Transform[] bones, out GameObject armature) {
            bones = bonesAndArmatureHolder.transform.Find("BonesHolder")
                .GetComponent<SkinnedMeshRenderer>().bones;
            armature = bonesAndArmatureHolder.transform.Find("Armature").gameObject;
        }


        /// <summary>
        /// Возвращает элементы внешнего вида, которые препятствовали бы активации новых элементов
        /// с заданными id
        /// </summary>
        public List<AppearanceElement> GetOccupied(IEnumerable<uint> appearanceElementsIds) {
            var res = new List<AppearanceElement>();
            AppearanceType appearanceType = _appearanceTypesManager.AppearanceTypes[this._appearanceTypeId];
            foreach (uint appearanceId in appearanceElementsIds) {
                res.AddRange(_occupancy.GetOccupied(
                    appearanceType.AppearanceElements[appearanceId].OccupancyIds
                ));
            }
            return res;
        }

        /// <summary>
        /// Активирует нестатичные элементы внешнего вида по их id, а также
        /// скрывает элементы, которые занимали место новых.
        /// Возвращает id элементов, которые были скрыты
        /// </summary>
        public List<uint> ActivateNonStaticElementsAndDeactivateOccupied(
            IEnumerable<uint> appearanceElementsIds) {
            var idsOfHidden = new List<uint>();
            // Скрываем мешающие элементы внешнего вида
            var occupied = GetOccupied(appearanceElementsIds);
            if (occupied.Count > 0) {
                foreach (AppearanceElement elem in occupied) {
                    _nonStaticElements[elem.LocalId].SetActive(false);
                    idsOfHidden.Add(elem.LocalId);
                }
            }

            SetNonStaticElementsActive(appearanceElementsIds, true);
            return idsOfHidden;
        }

        /// <summary>
        /// Скрывает элементы внешнего вида по заданным id.
        /// Важно: для обработки скрытия статичных элементов используются другие средства
        /// (это семантически иной случай, кроме того, его обработка сопровождается удалением
        /// элемента со сцены)
        /// </summary>
        public void DeactivateNonStaticElements(IEnumerable<uint> appearanceElementsIds) {
            SetNonStaticElementsActive(appearanceElementsIds, false);
        }

        private void SetNonStaticElementsActive(IEnumerable<uint> appearanceElementsIds, bool value) {
            AppearanceType type = _appearanceTypesManager.AppearanceTypes[_appearanceTypeId];
            foreach (uint id in appearanceElementsIds) {
                _nonStaticElements[id].SetActive(value);
                var elem = type.AppearanceElements[id];
                if (value)
                    _occupancy.Occupy(elem);
                else
                    _occupancy.Dispossess(elem);
            }
        }
    }
}
