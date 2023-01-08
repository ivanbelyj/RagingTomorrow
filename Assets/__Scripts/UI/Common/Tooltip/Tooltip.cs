using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private GameObject _tooltipLinePrefab;
    [SerializeField]
    private GameObject _textPrefab;

    private VerticalLayoutGroup _layoutGroup;

    private void Awake() {
        _layoutGroup = GetComponent<VerticalLayoutGroup>();
    }
    
    public void SetData(TooltipContent data) {
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
            // Создание текста
            GameObject textGO = Instantiate(_textPrefab);
            textGO.transform.SetParent(line.transform);
            textGO.transform.SetAsLastSibling();

            // Установка параметров в соответствии с данными
            TextMeshProUGUI textMesh = textGO.GetComponent<TextMeshProUGUI>();
            textMesh.text = elem.Text;
        }

        StartCoroutine(UpdateTooltipLayoutGroup());
    }

    private IEnumerator UpdateTooltipLayoutGroup()
    {
        yield return new WaitForEndOfFrame();
        _layoutGroup.enabled = false;
        _layoutGroup.CalculateLayoutInputVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.transform as RectTransform);
        _layoutGroup.enabled = true;
    }
}
