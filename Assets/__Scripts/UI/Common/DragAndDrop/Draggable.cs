using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // private IDraggedDataProvider<T> _draggedDataProvider;
    private T _draggedData;

    /// <summary>
    /// Перемещаемый по Canvas UI элемент может сопровождаться какой-либо информацией, которую
    /// требуется переместить между сущностями.
    /// Теоретически, эта информация может измениться во время перетаскивания. В таком случае можно
    /// изменить реализация Draggable<T> и использовать объект для отложенного получения
    /// актуальных данных
    /// </summary>
    public T DraggedData => _draggedData;

    protected Canvas _canvas;
    // private Vector3 _dragPos;
    private Vector3 _initialPos;
    protected GraphicRaycaster _graphicRaycaster;

    /// <summary>
    /// В момент, когда элемент переносится, он должен отображаться над всеми секциями
    /// инвентаря, т.е. быть последним дочерним эл-том главного родительского UI.
    /// </summary>
    // private Transform _uiParent;

    private Transform _oldParent;

    /// <summary>
    /// true, если при следующем событии Drag нужно сбросить перетаскивание 
    /// </summary>
    private bool _shouldResetDrag = false;

    protected virtual void Awake() {
        _canvas = FindObjectOfType<Canvas>();
        // _uiParent = _canvas.transform;

        _graphicRaycaster = _canvas.GetComponent<GraphicRaycaster>();
        Debug.Log($"canvas: {_canvas}; raycaster: {_graphicRaycaster}");
    }

    protected virtual void Start() {
        
    }

    public virtual void Initialize(/*IDraggedDataProvider<T> draggedDataProvider*/T draggedData) {
        // _draggedDataProvider = draggedDataProvider;
        _draggedData = draggedData;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        _shouldResetDrag = false;

        Debug.Log("Drag begins");
        _initialPos = transform.position;

        _oldParent = transform.parent;
        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();
    }

    public virtual void OnDrag(PointerEventData eventData) {
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     _canvas.transform as RectTransform,
        //     eventData.position, _canvas.worldCamera,
        //     out Vector2 movePos);

        if (_shouldResetDrag) {
            eventData.pointerDrag = null;
        }

        // _dragPos += (Vector3)eventData.delta;
        // Vector3 mousePos = _dragPos;

        // transform.position = mousePos;
        transform.position += (Vector3)eventData.delta;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drag ends");

        var results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(eventData, results);
        foreach (var hit in results)
        {
            var dropAcceptor = hit.gameObject.GetComponent<IDropAcceptor<T>>();
            if (dropAcceptor is not null)
            {
                Debug.Log("End drag on DropAcceptor");
                dropAcceptor.AcceptDrop(this, _draggedData/*_draggedDataProvider.GetDraggedData()*/);
                return;
            }
        }
        ResetDrag();
    }

    public virtual void ResetDrag() {
        Debug.Log("Reset Drag");
        transform.position = _initialPos;
        transform.SetParent(_oldParent);
        _shouldResetDrag = true;
    }
}
