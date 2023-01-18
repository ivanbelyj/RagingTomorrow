using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Перетаскиваемая иконка предмета. В отличие от базового класса, не требует внешнего вызова
/// метода инициализации, т.к. данный компонент вызывает его с необходимыми данными сам
/// </summary>
[RequireComponent(typeof(InventorySlotItem))]
public class DraggableItem : Draggable<DraggedItemData>
{
    public GameObject test;

    private InventorySlotItem _itemProvider;

    override protected void Awake() {
        base.Awake();
        _itemProvider = GetComponent<InventorySlotItem>();
    }

    protected override void Start()
    {
        base.Start();
        uint playerNetId = NetworkClient.localPlayer.netId;
        // К методу Start local id уже должен быть инициализирован
        // Debug.Log($"Initialize draggable item: local id {_slotItem.ItemLocalId}; "
        //     + $"sectionNetId: {sectionNetId}; playerNetId: {playerNetId}");
        Debug.Log("Инициализация DraggableItem. PlacementId: " + _itemProvider.InventoryItem.PlacementId
            + "; DraggingPlayerNetId: " + playerNetId);
        DraggedItemData data = new DraggedItemData() {
            PlacementId = _itemProvider.InventoryItem.PlacementId,
            DraggingPlayerNetId = playerNetId,
        };

        // DraggableItem не требует явного вызова инициализации, как его базовый класс, т.к.
        // все данные известны заранее.
        // ItemsUI ищется в том числе среди неактивных GameObject, т.к. интерфейс может быть скрыт
        this.Initialize(data, (RectTransform)(FindObjectOfType<ItemsUIController>(true).transform));
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        Vector2Int offset = GetMouseOffset(eventData.position);
        DraggedData.MouseSlotsOffsetX = offset.x;
        DraggedData.MouseSlotsOffsetY = offset.y;
        
        // Debug.Log($"Отступ мыши от left top угла предмета в слотах: {offset}");
        // Debug.Log($"Dragged item with " + DraggedData.ItemLocalId);
    }

    private void SetDebugPoint(Transform parent, Vector2 pos, Color color = default(Color)) {
        var test0 = Instantiate(test, Vector3.zero, Quaternion.identity, parent);
        test0.transform.localPosition = pos;
        test0.transform.SetAsLastSibling();

        var img = test0.GetComponent<Image>();
        if (color == default(Color)) {
            img.color = Color.red;
        } else {
            img.color = color;
        }
    }

    private Vector2Int GetMouseOffset(Vector2 mousePos) {
        // Debug.Log("Позиция мыши: " + mousePos);
        // Позиция мыши преобразуется в координаты DraggableItem относительно canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)_canvas.transform,
            mousePos, _canvas.worldCamera,
            out Vector2 posInCanvas);
        // Debug.Log("Позиция мыши в canvas (yellow): " + posInCanvas);
        // SetDebugPoint(_canvas.transform, posInCanvas, Color.yellow);

        // Vector2 worldMousePos = (Vector2)_canvas.transform.TransformPoint(posInCanvas);
        // Debug.Log("Позиция мыши в глобальных коорд.: " + worldPosInRect);
        // SetDebugPoint(_canvas.transform, transform.position, Color.blue);

        // Разница между коорд. верхнего левого угла DraggableItem и преобразованной позицией
        // указателя
        Vector2 offset = posInCanvas - (Vector2)transform.localPosition;

        // debug
        // SetDebugPoint(transform, Vector2.zero);
        // SetDebugPoint(transform, offset);

        // Debug.Log("Отступ от left top угла предмета: " + offset);
        Vector2Int inSlots = _itemProvider.ToSlots(offset);
        inSlots.y *= -1;
        return inSlots;
    }
}
