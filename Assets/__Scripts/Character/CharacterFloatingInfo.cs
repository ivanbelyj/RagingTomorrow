using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterDataProvider))]
[RequireComponent(typeof(Entity))]
public class CharacterFloatingInfo : NetworkBehaviour
{
    [SerializeField]
    private GameObject floatingInfo;
    [SerializeField]
    public TextMeshPro characterNameText;
    [SerializeField]
    public TextMeshPro characterHealthText;

    private CharacterDataProvider _characterDataProvider;
    private Entity _entity;

    private void Awake() {
        _characterDataProvider = GetComponent<CharacterDataProvider>();
        _entity = GetComponent<Entity>();
        _characterDataProvider.CharacterDataChanged += (CharacterData newData) => {
            // Debug.Log("Character info is changed");
            characterNameText.text = newData.Name;
        };
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        BindFloatingInfo();
    }

    private void BindFloatingInfo() {
        SetHealthText(_entity.health.Value);
        _entity.health.OnValueChanged += (old, newVal) => {
            SetHealthText(newVal);
        };
        void SetHealthText(float val) {
            characterHealthText.text = string.Format("{0:F2}", System.Math.Round(val, 2));
        }
    }

    private void Update() {
        if (!isLocalPlayer)
            floatingInfo.transform.LookAt(Camera.main.transform);
    }
}
