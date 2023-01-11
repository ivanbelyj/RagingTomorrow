using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[AddComponentMenu("Tooltip/Tooltip")]
public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private GameObject _tooltipLinePrefab;
    [SerializeField]
    private GameObject _textPrefab;
    [SerializeField]
    private float _headerFontSize = 24f;

    private VerticalLayoutGroup _layoutGroup;

    private void Awake() {
        _layoutGroup = GetComponent<VerticalLayoutGroup>();
    }

    // Например, если пользователь скрыл интерфейс нажатием клавиши, подсказка должна безвозвратно
    // пропасть
    private void OnDisable() {
        Destroy(this.gameObject);
    }
    
    public void DisplayContent(TooltipContent data) {
        foreach (TooltipLine elem in data.Elements) {
            Line(elem.Elements);
        }
    }

    private void Line(List<TooltipText> elements) {
        // Создание линии
        GameObject line = Instantiate(_tooltipLinePrefab);
        line.transform.SetParent(this.transform);
        line.transform.SetAsLastSibling();
        foreach (TooltipText elem in elements) {
            GameObject textGO = Instantiate(_textPrefab);

            // Установка параметров в соответствии с данными
            TextMeshProUGUI textMesh = textGO.GetComponent<TextMeshProUGUI>();
            textMesh.text = elem.Text;
            textMesh.color = elem.Color;
            if (elem.IsHeader)
                textMesh.fontSize = _headerFontSize;

            RectTransform textRectTransform = (RectTransform)(textGO.transform);
            textRectTransform.sizeDelta = new Vector2(textRectTransform.sizeDelta.x,
                textRectTransform.sizeDelta.y + elem.BottomVSpace);
            
            // Добавление текста в линию
            textGO.transform.SetParent(line.transform);
            textGO.transform.SetAsLastSibling();
        }

        // StartCoroutine(UpdateTooltipLayoutGroup());
    }

    // private IEnumerator UpdateTooltipLayoutGroup()
    // {
    //     yield return new WaitForEndOfFrame();
    //     _layoutGroup.enabled = false;
    //     _layoutGroup.CalculateLayoutInputVertical();
    //     LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.transform as RectTransform);
    //     _layoutGroup.enabled = true;
    // }
}
