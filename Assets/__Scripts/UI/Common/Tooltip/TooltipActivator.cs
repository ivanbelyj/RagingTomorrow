using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Компонент активации всплывающей подсказки, когда на элемент UI наведен указатель
/// </summary>
[AddComponentMenu("Tooltip/TooltipActivator")]
public class TooltipActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerMoveHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject _tooltipPrefab;

    private Canvas _canvas;
    private RectTransform _parent;

    // Provider используется только при создании всплывающей подсказки, т.е., информация не обновляется
    // при ее обновлении в источнике
    private ITooltipContentProvider _dataProvider;

    /// <summary>
    /// Объект всплывающей подсказки, созданный в момент наведения указателя на TooltipActivator
    /// </summary>
    private Tooltip _tooltip;

    /// <summary>
    /// В некоторых ситуациях всплывающая подсказка не появляется (например, при перетаскивании)
    /// </summary>
    private bool _dontCreateTooltip;
    
    private void Awake() {
        _dataProvider = GetComponent<ITooltipContentProvider>();
        _canvas = FindObjectOfType<Canvas>();
    }

    private void OnDisable() {
        // После временного сокрытия пользовательского интерфейса (когда предыдущая
        // всплывающая подсказка была уничтожена) можно и нужно создавать новую 
        _dontCreateTooltip = false;
    }

    // Например, уничтожает всплывающую подсказку в случае, когда другой игрок забрал предмет
    // из общего инвентаря (допустим, ящика) и иконка пропала
    private void OnDestroy() {
        if (_tooltip != null)
            Destroy(_tooltip);
    }

    /// <summary>
    /// Всплывающие подсказки должны иметь родительский объект, чтобы скрываться вместе с ним
    /// </summary>
    public void Initialize(RectTransform parent) {
        _parent = parent;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_dontCreateTooltip) {
            return;
        }
        
        CreateTooltip(eventData.position);
    }

    private void CreateTooltip(Vector2 mousePos) {
        _tooltip = Instantiate(_tooltipPrefab).GetComponent<Tooltip>();
        _tooltip.transform.SetParent(_parent);
        _tooltip.transform.SetAsLastSibling();

        _tooltip.DisplayContent(_dataProvider.GetTooltipContent());
        _tooltip.transform.localPosition = GetLocalPosInParentWithOffset(mousePos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltip != null) {
            DestroyTooltip();
        }
    }

    /// <summary>
    /// Получает координаты, создание всплывающей подсказки в которых будет наиболее оптимально
    /// (подсказка не перекрывает указатель, не выходит за рамки экрана)
    /// </summary>
    private Vector2 GetLocalPosInParentWithOffset(Vector2 pointerPosition) {
        return GetLocalPosInParentByPointer(pointerPosition)
            + GetOffsetFromEdgesOfWindow(pointerPosition);
    }

    private Vector2 GetLocalPosInParentByPointer(Vector2 pointerPosition) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parent as RectTransform,
            pointerPosition, _canvas.worldCamera,
            out Vector2 res);
        return res;
    } 

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_dontCreateTooltip || _tooltip == null)
            return;
        _tooltip.transform.localPosition = GetLocalPosInParentWithOffset(eventData.position);
    }

    private void DestroyTooltip() {
        Destroy(_tooltip.gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dontCreateTooltip = true;
        if (_tooltip != null)
            DestroyTooltip();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dontCreateTooltip = false;
    }

    /// <summary>
    /// Функция вычисления отступа, при котором всплывающая подсказка будет полностью
    /// отображаться на экране. Предполагается, что подсказка не может быть перекрыта сверху,
    /// а также слева из-за положения мыши в верхнем левом углу подсказки
    /// </summary>
    private Vector2 GetOffsetFromEdgesOfWindow(Vector2 mousePos) {
        float windowWidth = Screen.currentResolution.width;
        float windowHeight = Screen.currentResolution.height;
        Vector2 windowSize = new Vector2(windowWidth, windowHeight);

        Vector2 tooltipSize = ((RectTransform)_tooltip.transform).sizeDelta;
        if (tooltipSize.y < 1) {
            // Debug.LogWarning("Tooltip height is less than 1!");
        }
        
        // Если какое-либо след. значение отрицательно, то по соотв. стороне 
        // переполнение, => необходим соотв. отступ, чтобы все поместилось на экране
        float overflowX = windowWidth - mousePos.x - tooltipSize.x;
        float overflowY = mousePos.y - tooltipSize.y;
        // Debug.Log($"windowWidth: {windowWidth}; tooltipWidth: {tooltipSize.x}; mousePos.x: {mousePos.x}; "
        //     + $"negOverflow.x: {overflowX}");
        // Debug.Log($"windowHeight: {windowHeight}; tooltipHeight: {tooltipSize.y}; mousePos.y: {mousePos.y}; "
        //     + $"negOverflow.y: {overflowY}");
        return new Vector2(overflowX >= 0 ? 0 : overflowX,
            overflowY >= 0 ? 0 : -overflowY);
    }
}
