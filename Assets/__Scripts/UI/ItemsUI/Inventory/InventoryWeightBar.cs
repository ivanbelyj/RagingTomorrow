using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryWeightBar : MonoBehaviour
{
    [SerializeField]
    private string weightInfoFormatString = "Вес: {0:F2} / {1:F2} кг";

    [SerializeField]
    private TextMeshProUGUI weightInfoText;

    private void Start() {
        UpdateWeightInfoText(0, 40);
    }

    public void UpdateWeightInfoText(float weight, float maxWeight) {
        weightInfoText.text = string.Format(weightInfoFormatString, weight, maxWeight);
    }
}
