using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AppearanceCustomization3D {
    /// <summary>
    /// Предоставляет типы кастомизируемых объектов, созданные в редакторе
    /// <summary>
    public class AppearanceTypesManager : MonoBehaviour
    {
        [SerializeField]
        private string _bundleName = "appearancetype";

        /// <summary>
        /// Id (имена) типов кастомизируемых объектов и объекты этих типов
        /// </summary>
        public Dictionary<AppearanceTypeId, AppearanceType> AppearanceTypes { get; private set; }

        private void Awake() {
            AssetBundle localAssetBundle = AssetBundle.LoadFromFile(
                System.IO.Path.Combine(Application.streamingAssetsPath, _bundleName)
            );

            if (localAssetBundle is null) {
                Debug.LogError($"Failed to load {_bundleName} AssetBundle");
                return;
            }

            AppearanceType[] assets = localAssetBundle.LoadAllAssets<AppearanceType>();
            AppearanceTypes = new Dictionary<AppearanceTypeId, AppearanceType>();
            foreach (AppearanceType appearance in assets) {
                AppearanceTypes.Add(new AppearanceTypeId(appearance.name), appearance);
            }
            Debug.Log("Appearance types dictionary is initialized");
            foreach (var pair in AppearanceTypes) {
                Debug.Log("\t" + pair.Key + ": " + pair.Value.name);
            }

            localAssetBundle.Unload(false);
        }
    }
}
