using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDragAndDropUI : MonoBehaviour
{
    // [SerializeField]
    // private Canvas _canvas;

    // private bool _isMoving;
    // private Transform _moving;

    public void Update() {
        // if (_isMoving) {
        //     Vector2 movePos;
        //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //         _canvas.transform as RectTransform,
        //         Input.mousePosition, _canvas.worldCamera,
        //         out movePos);

        //     Vector3 mousePos = _canvas.transform.TransformPoint(movePos);

        //     //Set fake mouse Cursor
        //     // mouseCursor.transform.position = mousePos;

        //     _moving.position = mousePos;
        // }
    }

    // public void Drag(DraggedItem draggedItem) {
    //     if (_isMoving) {
    //         Debug.LogError("Попытка перетаскивания более чем одного объекта");
    //     }
    //     _isMoving = true;
    //     _moving = draggedItem.gameObject.transform;
    // }

    /// <summary>
    /// Прекращает UI-аспект функциональности перетаскивания
    /// </summary>
    // public void Drop() {
    //     _isMoving = false;
    //     _moving = null;
    // }
}
