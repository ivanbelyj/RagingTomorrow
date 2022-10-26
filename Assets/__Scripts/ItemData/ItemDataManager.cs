using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemDataManager : MonoBehaviour
{
    // Загружаемые из bundled asset предметы могут спавниться
    [SerializeField]
    private NetMan _netManager;
    public string bundleName = "itemdata";
    
    private Dictionary<string, ItemData> _namesAndData;
    public Dictionary<string, ItemData> NamesAndData => _namesAndData;

    public ItemData GetItemDataByName(string name) => _namesAndData[name];

    private void Start() {
        AssetBundle localAssetBundle = AssetBundle.LoadFromFile(
            Path.Combine(Application.streamingAssetsPath, bundleName)
        );

        if (localAssetBundle is null) {
            Debug.LogError($"Failed to load {bundleName} AssetBundle");
            return;
        }

        ItemData[] assets = localAssetBundle.LoadAllAssets<ItemData>();
        CreateItemDataDictionary(assets);
        Debug.Log("ItemData dictionary is initialized");
        foreach (var pair in _namesAndData) {
            string typeName = pair.Value.GetType().Name;
            Debug.Log($"\t{pair.Key}: {pair.Value}. ItemData type: " + typeName);
        }

        localAssetBundle.Unload(false);
    }

    private void CreateItemDataDictionary(ItemData[] arr) {
        _namesAndData = new Dictionary<string, ItemData>();
        foreach (ItemData itemData in arr) {
            _namesAndData.Add(itemData.name, itemData);

            Item item = itemData.Item;
            _netManager.spawnPrefabs.Add(item.gameObject);
        }
    }
}
