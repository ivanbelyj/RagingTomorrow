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

    /// <summary>
    /// Методы данного компонента, прикрепленного к подсказке, вызываются при изменении
    /// ее позиции, чтобы она помещалась в рамки экрана
    /// </summary>
    // private FitToScreen _fitToScreen;
    
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
        _tooltip.transform.localPosition = GetLocalPosInParentByPointer(mousePos);
        // GetLocalPosInParentWithOffset(mousePos);
        // _tooltip.GetComponent<FitToScreen>().Fit();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltip != null) {
            DestroyTooltip();
        }
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
        _tooltip.transform.localPosition = GetLocalPosInParentByPointer(eventData.position);
        // _tooltip.GetComponent<FitToScreen>().Fit();
        // GetLocalPosInParentWithOffset(eventData.position);
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
}
