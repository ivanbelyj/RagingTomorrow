using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Информация об инвентаре. Например, может содержать имя владельца.
/// </summary>
public class InventoryInfoUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;

    [SerializeField]
    private TextMeshProUGUI _subTitle;

    [SerializeField]
    private Image _avatar;

    private string Title {
        get => _title.text;
        set {
            _title.text = value;
        }
    }
    private string SubTitle {
        get => _subTitle.text;
        set {
            _subTitle.text = value;
        }
    }
    private Image Avatar => _avatar;
    private void SetAvatar(Sprite avatar) {
        if (avatar is not null)
            _avatar.sprite = avatar;
    }

    public void SetInfo(InventoryInfo invInfo) {
        Title = invInfo.Title;
        SubTitle = invInfo.SubTitle;
        SetAvatar(invInfo.Avatar);
    }
}
