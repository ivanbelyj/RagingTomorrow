using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI отдельного предмета, находящегося в инвентаре. Может использоваться как в сеточной секции
/// инвентаря, так и независимо
/// </summary>
public class InventorySlotItem : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemImageGO;

    // GameObject, содержащий TextMeshPro для отображения количества предметов
    [SerializeField]
    private GameObject _itemsCountGO;

    [SerializeField]
    private TextMeshProUGUI _itemsCountText;

    private Image _itemImage;
    private RectTransform _itemRectTransform;

    private ItemStaticDataManager _itemStaticDataManager;

    private float _slotSize;
    // private float _slotHeight;
    private float _gridSpacing;

    private ItemData _itemData;
    public ItemData ItemData => _itemData;

    private uint _itemLocalId;
    public uint ItemLocalId => _itemLocalId;

    private uint _sectionNetId;
    public uint SectionNetId => _sectionNetId;

    private void Awake() {
        _itemImage = _itemImageGO.GetComponent<Image>();
        _itemRectTransform = _itemImageGO.GetComponent<RectTransform>();
    }

    /// <summary>
    /// ItemStaticDataManager требуется для получения данных о предмете по имени,
    /// т.к. при установке предмета в слот передаются компактные данные
    /// </summary>
    public void Initialize(ItemData itemData, int count, uint itemLocalId, uint sectionNetId,
        ItemStaticDataManager itemStaticDataManager,
        float slotSize/*, float slotHeight*/, float gridSpacing) {
        _itemData = itemData;
        _itemLocalId = itemLocalId;
        _sectionNetId = sectionNetId;

        _itemStaticDataManager = itemStaticDataManager;
        _slotSize = slotSize;
        // _slotHeight = slotHeight;
        _gridSpacing = gridSpacing;

        SetItemUI(itemData, count);
    }

    // public void Initialize(GridSectionItem gridItem, uint itemLocalId,
    //     ItemStaticDataManager itemStaticDataManager,
    //     float slotWidth, float slotHeight, float gridSpacing) {
    //     Initialize(gridItem.itemData, gridItem.Count, , itemLocalId, itemStaticDataManager,
    //         slotWidth, slotHeight, gridSpacing);
    // }

    private void SetSprite(Sprite sprite) {
        _itemImage.gameObject.SetActive(sprite is not null);
        _itemImage.sprite = sprite;
    }

    private void SetItemsCountUI(int itemsCount) {
        if (itemsCount > 1) {
            _itemsCountGO.SetActive(true);
            _itemsCountText.text = itemsCount.ToString();
        } else {
            _itemsCountGO.SetActive(false);
        }
    }
    
    /// <summary>
    /// Устанавливает предмет для UI
    /// </summary>
    private void SetItemUI(ItemData itemData, int count) {
        ItemStaticData staticData = _itemStaticDataManager.GetStaticDataByName(
            itemData.itemStaticDataName);
        
        // Картинка предмета получает размер на некоторое кол-во слотов
        _itemRectTransform.sizeDelta =
            FromSlots(new Vector2Int(staticData.Width, staticData.Height));
            // new Vector2(_slotSize * staticData.Width + _gridSpacing * (staticData.Width - 1),
            // _slotSize * staticData.Height + _gridSpacing * (staticData.Height - 1));
        
        SetSprite(staticData.Sprite);
        SetItemsCountUI(count);
    }

    public Vector2 FromSlots(Vector2Int v) =>
        new Vector2(LenghtOfSlots(v.x), LenghtOfSlots(v.y));

    public Vector2Int ToSlots(Vector2 v) {
        return new Vector2Int(SlotsInLength(v.x), SlotsInLength(v.y));
        
        // var staticData = _itemStaticDataManager
        //     .GetStaticDataByName(_itemData.itemStaticDataName);
        // return new Vector2Int(
        //     (int)((v.x * _itemRectTransform.sizeDelta.x)
        //     / (_slotSize * _itemRectTransform.sizeDelta.x)),
        //     (int)((v.y * _itemRectTransform.sizeDelta.y)
        //     / (_slotSize * _itemRectTransform.sizeDelta.y))
        // );
        // slot * width -- transform.width
        // x * width -- v.x

        // x / transform.width = v.x / (slot * width)
        // x = (v.x * transform.width) / (slot * width)
    }
        

    private float LenghtOfSlots(int slots) {
        return _slotSize * slots + _gridSpacing * (slots - 1);
    }

    private int SlotsInLength(float l) {
        // l = _slotSize * slots + _gridSpacing * (slots - 1);
        // l = _slotSize * slots + _gridSpacing * slots - _gridSpacing;
        // l + _gridSpacing = slots * (_slotSize + _gridSpacing);
        // (l + _gridSpacing) / (_slotSize + _gridSpacing) = slots;
        return (int)((l + _gridSpacing) / (_slotSize + _gridSpacing));
    }
}
