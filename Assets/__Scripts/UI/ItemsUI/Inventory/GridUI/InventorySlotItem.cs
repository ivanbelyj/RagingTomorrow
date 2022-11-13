using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private float _slotWidth;
    private float _slotHeight;
    private float _gridSpacing;

    private void Awake() {
        _itemImage = _itemImageGO.GetComponent<Image>();
        _itemRectTransform = _itemImageGO.GetComponent<RectTransform>();
    }

    /// <summary>
    /// ItemStaticDataManager требуется для получения данных о предмете по имени,
    /// т.к. при установке предмета в слот передаются компактные данные
    /// </summary>
    public void Initialize(ItemStaticDataManager itemStaticDataManager,
        float slotWidth, float slotHeight, float gridSpacing) {
        _itemStaticDataManager = itemStaticDataManager;
        _slotWidth = slotWidth;
        _slotHeight = slotHeight;
        _gridSpacing = gridSpacing;
    }

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

    public void SetItem(GridSectionItem invItem) {
        ItemStaticData staticData = _itemStaticDataManager.GetStaticDataByName(
            invItem.itemData.itemStaticDataName);
        
        // Картинка предмета получает размер на некоторое кол-во слотов
        _itemRectTransform.sizeDelta =
            new Vector2(_slotWidth * staticData.Width + _gridSpacing * (staticData.Width - 1),
            _slotHeight * staticData.Height + _gridSpacing * (staticData.Height - 1));
        
        SetSprite(staticData.Sprite);
        SetItemsCountUI(invItem.count);
    }
}
