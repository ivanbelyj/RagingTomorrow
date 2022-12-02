using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemImageGO;

    private Image _itemImage;
    private RectTransform _itemRectTransform;

    private ItemStaticDataManager _itemStaticDataManager;

    private float _slotSize;
    private float _gridSpacing;

    private ItemData _itemData;
    public ItemData ItemData => _itemData;

    private void Awake() {
        _itemImage = _itemImageGO.GetComponent<Image>();
        _itemRectTransform = _itemImageGO.GetComponent<RectTransform>();
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    /// <summary>
    /// ItemStaticDataManager требуется для получения данных о предмете по имени,
    /// т.к. при установке предмета в слот передаются компактные данные
    /// </summary>
    public void Initialize(ItemData itemData,
        float slotSize, float gridSpacing) {
        _itemData = itemData;

        _slotSize = slotSize;
        _gridSpacing = gridSpacing;

        SetItemUI(itemData);
    }

    private void SetSprite(Sprite sprite) {
        _itemImage.gameObject.SetActive(sprite is not null);
        _itemImage.sprite = sprite;
    }
    
    /// <summary>
    /// Устанавливает предмет для UI
    /// </summary>
    protected void SetItemUI(ItemData itemData) {
        ItemStaticData staticData = _itemStaticDataManager.GetStaticDataByName(
            itemData.itemStaticDataName);
        
        // Картинка предмета получает размер на некоторое кол-во слотов
        _itemRectTransform.sizeDelta =
            FromSlots(new Vector2Int(staticData.Width, staticData.Height));
        
        SetSprite(staticData.Sprite);
    }

    public Vector2 FromSlots(Vector2Int v) =>
        new Vector2(LenghtOfSlots(v.x), LenghtOfSlots(v.y));

    public Vector2Int ToSlots(Vector2 v) {
        return new Vector2Int(SlotsInLength(v.x), SlotsInLength(v.y));
    }    

    private float LenghtOfSlots(int slots) {
        return _slotSize * slots + _gridSpacing * (slots - 1);
    }

    private int SlotsInLength(float l) {
        // Выведено из LengthOfSlots по правилам математики
        return (int)((l + _gridSpacing) / (_slotSize + _gridSpacing));
    }
}
