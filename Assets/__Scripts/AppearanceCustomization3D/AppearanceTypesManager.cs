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
        public Dictionary<string, AppearanceType> AppearanceTypes { get; private set; }

        private void Awake() {
            AssetBundle localAssetBundle = AssetBundle.LoadFromFile(
                Path.Combine(Application.streamingAssetsPath, _bundleName)
            );

            if (localAssetBundle is null) {
                Debug.LogError($"Failed to load {_bundleName} AssetBundle");
                return;
            }

            AppearanceType[] assets = localAssetBundle.LoadAllAssets<AppearanceType>();
            AppearanceTypes = new Dictionary<string, AppearanceType>();
            foreach (AppearanceType appearance in assets) {
                AppearanceTypes.Add(appearance.name, appearance);
            }
            Debug.Log("Appearance types dictionary is initialized");

            localAssetBundle.Unload(false);
        }
    }
}
