using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AppearanceCustomization3D {
    public class CustomizableAppearance : MonoBehaviour
    {
        [SerializeField]
        private AppearanceData _initialAppearanceData;

        private AppearanceTypesManager _appearanceTypesManager;

        private void Awake() {
            _appearanceTypesManager = FindObjectOfType<AppearanceTypesManager>();
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
                .AppearanceTypes[data.AppearanceTypeId];
            
            HashSet<int> dataElemIdsSet = new HashSet<int>();
            foreach (int elemId in data.AppearanceElementIds) {
                dataElemIdsSet.Add(elemId);
            }

            Transform[] bones = null;
            GameObject armature = null;
            if (appearanceType.HasRig) {
                GameObject bonesAndArmatureHolder = Instantiate(appearanceType.BonesAndArmatureHolder);
                bonesAndArmatureHolder.transform.SetParent(transform);
                GetBonesAndArmature(bonesAndArmatureHolder, out bones, out armature);
            }

            Transform appearanceElementsRoot = new GameObject("AppearanceElements").transform;
            appearanceElementsRoot.SetParent(transform);
            
            foreach (AppearanceElement element in appearanceType.AppearanceElements.Values) {
                // Создаем нестатическое (например, одежду, броню, что может потребоваться потом),
                // а также статическое, что дано изначально (например, глаза, голову, бедра)
                if (!element.IsStatic || dataElemIdsSet.Contains(element.LocalId)) {
                    Debug.Log("Создание элемента кастомизации с local id " + element.LocalId);
                    GameObject elemGO = Instantiate(element.Prefab);
                    elemGO.transform.SetParent(appearanceElementsRoot);
                    if (appearanceType.HasRig) {
                        elemGO.GetComponent<SkinnedMeshRenderer>().bones = bones;
                    }
                    // Todo: network spawn
                }
            }
        }

        private void GetBonesAndArmature(GameObject bonesAndArmatureHolder,
            out Transform[] bones, out GameObject armature) {
            bones = bonesAndArmatureHolder.transform.Find("BonesHolder")
                .GetComponent<SkinnedMeshRenderer>().bones;
            armature = bonesAndArmatureHolder.transform.Find("Armature").gameObject;
        }
    }
}
