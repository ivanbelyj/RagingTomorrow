using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Компонент активации всплывающей подсказки, когда на элемент UI наведен указатель
/// </summary>
public class TooltipActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerMoveHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject _tooltipPrefab;

    private Canvas _canvas;

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
    private bool _blockTooltip;

    /// <summary>
    /// Отступ всплывающей подсказки от точки указателя
    /// </summary>
    private readonly Vector2 _tooltipOffset = new Vector2(20, -20);
    
    private void Awake() {
        _dataProvider = GetComponent<ITooltipContentProvider>();
        _canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_blockTooltip)
            return;
        
        _tooltip = Instantiate(_tooltipPrefab).GetComponent<Tooltip>();
        _tooltip.transform.SetParent(_canvas.transform);
        _tooltip.transform.SetAsLastSibling();

        _tooltip.SetData(_dataProvider.GetTooltipContent());

        _tooltip.transform.localPosition = GetLocalPosInCanvasByPointer(eventData.position) + _tooltipOffset;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltip != null) {
            DestroyTooltip();
        }
    }

    private Vector2 GetLocalPosInCanvasByPointer(Vector2 pointerPosition) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            pointerPosition, _canvas.worldCamera,
            out Vector2 res);
        return res;
    } 

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_blockTooltip || _tooltip == null)
            return;
        _tooltip.transform.localPosition = GetLocalPosInCanvasByPointer(eventData.position) + _tooltipOffset;
    }

    private void DestroyTooltip() {
        Destroy(_tooltip.gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _blockTooltip = true;
        DestroyTooltip();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _blockTooltip = false;
    }
}
