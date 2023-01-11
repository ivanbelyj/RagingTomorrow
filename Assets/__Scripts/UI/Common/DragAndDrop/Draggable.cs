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

    private RectTransform _parent;

    private Vector3 _initialPos;
    protected GraphicRaycaster _graphicRaycaster;

    /// <summary>
    /// В момент, когда элемент переносится, он должен отображаться над всеми секциями
    /// инвентаря, т.е. быть последним дочерним эл-том главного родительского UI.
    /// </summary>
    // private Transform _uiParent;

    private Transform _parentBeforeDrag;

    /// <summary>
    /// true, если при следующем событии Drag нужно сбросить перетаскивание 
    /// </summary>
    private bool _shouldResetDrag = false;

    /// <summary>
    /// Не стоит выполнять лишние действия при отключении объекта, если перетаскивания не произошло
    /// </summary>
    private bool _shouldResetOnDisable = false;

    /// <summary>
    /// Нельзя установить объект в качестве родителя, пока он отключается, поэтому установка
    /// откладывается до включения
    /// </summary>
    private bool _shouldSetParentAndPosOnEnable = false;

    protected virtual void Awake() {
        _canvas = FindObjectOfType<Canvas>();

        _graphicRaycaster = _canvas.GetComponent<GraphicRaycaster>();
        // Debug.Log($"canvas: {_canvas}; raycaster: {_graphicRaycaster}");
    }

    protected virtual void Start() {
        
    }

    // В случае отключения объекта перетаскивание сбрасывается
    private void OnDisable() {
        
        if (_shouldResetOnDisable) {
            ResetDrag(deferredSetParentAndPos: true);
        }
    }

    private void OnEnable() {
        if (_shouldSetParentAndPosOnEnable) {
            SetParentAndPos();
            _shouldSetParentAndPosOnEnable = false;
        }
    }

    /// <summary>
    /// Каждый перетаскиваемый элемент несет какую-либо информацию, а также имеет родительский элемент,
    /// при скрытии которого должен скрываться и перетаскиваемый
    /// </summary>
    public virtual void Initialize(T draggedData, RectTransform parent) {
        _draggedData = draggedData;
        _parent = parent;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        _shouldResetDrag = false;
        _shouldResetOnDisable = true;

        // Debug.Log($"Drag begins. initial pos is changed from {_initialPos} to {transform.localPosition}");
        _initialPos = transform.localPosition;

        // Debug.Log("Initial pos on set begin drag: " + _initialPos
        //     + ". Actual: " + transform.localPosition);

        _parentBeforeDrag = transform.parent;
        transform.SetParent(_parent);
        transform.SetAsLastSibling();
    }

    public virtual void OnDrag(PointerEventData eventData) {
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     _canvas.transform as RectTransform,
        //     eventData.position, _canvas.worldCamera,
        //     out Vector2 movePos);

        if (_shouldResetDrag) {
            eventData.pointerDrag = null;
            return;
        }

        // _dragPos += (Vector3)eventData.delta;
        // Vector3 mousePos = _dragPos;

        // transform.position = mousePos;

        transform.position += (Vector3)eventData.delta;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // Todo: только для теста, убрать
        if (Player.dontEndDrag)
            return;

        var results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(eventData, results);
        foreach (var hit in results)
        {
            var dropAcceptor = hit.gameObject.GetComponent<IDropAcceptor<T>>();
            if (dropAcceptor is not null)
            {
                dropAcceptor.AcceptDrop(this, _draggedData/*_draggedDataProvider.GetDraggedData()*/);
                _shouldResetOnDisable = false;
                return;
            }
        }
        ResetDrag();
    }

    public void ResetDrag(bool deferredSetParentAndPos = false) {
        _shouldResetDrag = true;
        if (deferredSetParentAndPos) {
            _shouldSetParentAndPosOnEnable = true;
        } else {
            SetParentAndPos();
        }

        // Уже сделано
        _shouldResetOnDisable = false;
    }

    private void SetParentAndPos() {
        transform.SetParent(_parentBeforeDrag);
        transform.localPosition = _initialPos;
    }
}
