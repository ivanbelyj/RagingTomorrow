using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggedItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private InventoryDragAndDropUI _dragAndDrop;
    private Canvas _canvas;
    private Vector3 _dragPos;

    private void Awake() {
        // Todo: Возможно, здесь поможет Dependency Injection
        _dragAndDrop = FindObjectOfType<InventoryDragAndDropUI>();

        _canvas = FindObjectOfType<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag begins");
        _dragPos = transform.position;

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     _canvas.transform as RectTransform,
        //     eventData.position, _canvas.worldCamera,
        //     out Vector2 movePos);

        _dragPos += (Vector3)eventData.delta;
        Vector3 mousePos = _dragPos;

        transform.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drag ends");

        // RaycastResult results = new List<RaycastResult>();
        // graphicRaycaster.Raycast(eventData, results);
        // // Check all hits.
        // foreach (var hit in results)
        // {
        //     // If we found slot.
        //     var slot = hit.gameObject.GetComponent<UIDropSlot>();
        //     if (slot)
        //     {
        //         // We should check if we can place ourselves​ there.
        //         if (!slot.SlotFilled)
        //         {
        //             // Swapping references.
        //             currentSlot.currentItem = null;
        //             currentSlot = slot;
        //             currentSlot.currentItem = this;
        //         }
        //         // In either cases we should break check loop.
        //         break;
        //     }
        // }
    }
}
