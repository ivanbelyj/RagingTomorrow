using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент, предоставляющий метод для перемещения элемента UI таким образом,
/// чтобы он помещался в рамки экрана.
/// Это может быть полезно для всплывающих подсказок и контекстных меню
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class FitToScreen : MonoBehaviour
{
    private Canvas _canvas;
    private bool _isFittingNow;

    private void Start() {
        _canvas = FindObjectOfType<Canvas>();
        // Пока объект потенциально не подстроен под размеры экрана, он скрывается
        // gameObject.SetActive(false);
        StartCoroutine(Fit());
    }
    
    private void Update() {
        // Debug.Log("transform.position to screen point: " + ToScreenPoint(transform.position));
        if (!_isFittingNow) {
            if (transform.hasChanged) {
                _isFittingNow = true;
                StartCoroutine(Fit());
            }
        }
    }

    private IEnumerator Fit() {
        if (GetUIElementSize().y < 1) {
            Debug.Log("Waiting until height of element is not zero");
            yield return null;
        }
            
        SetPosInBorderOfScreen();
        _isFittingNow = false;
    }

    private Vector2 GetUIElementSize() {
        return ((RectTransform)transform).sizeDelta;
    }

    /// <summary>
    /// Функция вычисления отступа, при котором всплывающая подсказка будет полностью
    /// отображаться на экране. Предполагается, что подсказка не может быть перекрыта сверху,
    /// а также слева из-за положения мыши в верхнем левом углу подсказки
    /// </summary>
    private void SetPosInBorderOfScreen() {
        Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        Vector2 uiElementSize = GetUIElementSize();
        if (uiElementSize.y < 1) {
            Debug.LogWarning("Tooltip height is less than 1!");
            // Debug.Break();
        }

        Vector2 posOnScreen = ToScreenPoint(transform.position);
        
        // Если какое-либо след. значение отрицательно, то по соотв. стороне 
        // переполнение, => необходим соотв. отступ, чтобы все поместилось на экране
        // float overflowX = windowWidth - oldPosOnScreen.x - uiElementSize.x;
        // float overflowY = windowHeight - oldPosOnScreen.y - uiElementSize.y;
        Vector2 overflow = new Vector2(); // windowSize - posOnScreen - uiElementSize;
        overflow.x = windowSize.x - posOnScreen.x - uiElementSize.x;
        overflow.y = posOnScreen.y - uiElementSize.y;
        // Debug.Log($"windowWidth: {windowWidth}; tooltipWidth: {uiElementSize.x}; mousePos.x: {oldPos.x}; "
        //     + $"negOverflow.x: {overflowX}");
        // Debug.Log($"windowHeight: {windowHeight}; tooltipHeight: {uiElementSize.y}; mousePos.y: {oldPos.y}; "
        //     + $"negOverflow.y: {overflowY}");
        
        if (overflow.x < 0 || overflow.y < 0) {
            // Vector2 screenPointWithOffset = new Vector2(posOnScreen.x + overflow.x,
            //     posOnScreen.y - overflow.y + 50);
            // Vector2 worldPosWithOffset = ToWorldPoint(screenPointWithOffset);
            // transform.position = worldPosWithOffset;

            Debug.Log($"windowSize: {windowSize}; uiElementSize: {uiElementSize}; " +
                $"posOnScreen: {posOnScreen}; overflow: {overflow}");
        }

        Vector2 newScreenPos = posOnScreen;

        if (overflow.x < 0) {
            newScreenPos.x = windowSize.x - uiElementSize.x;
        }
        if (overflow.y < 0) {
            newScreenPos.y = uiElementSize.y;
        }
        Vector2 worldPosWithOffset = ToWorldPoint(newScreenPos);
        transform.position = worldPosWithOffset;
    }

    private Vector2 ToScreenPoint(Vector3 pos) {
        return RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, pos);
    }

    private Vector3 ToWorldPoint(Vector3 screenPoint) {
        // Debug.Log("parent: " + transform.parent);
        // Debug.Log("screen point:" + screenPoint);
        // Debug.Log("canvas: " + _canvas + ". world camera: " + _canvas.worldCamera);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)transform,
            screenPoint, _canvas.worldCamera,
            out Vector3 res);
        return res;
    }
}
