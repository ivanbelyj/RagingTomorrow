using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterDataProvider))]
[RequireComponent(typeof(EntityLifecycle))]
public class CharacterFloatingInfo : NetworkBehaviour
{
    [SerializeField]
    private GameObject _floatingInfo;
    [SerializeField]
    public TextMeshPro _characterNameText;
    [SerializeField]
    public TextMeshPro _characterHealthText;

    private CharacterDataProvider _characterDataProvider;
    private EntityLifecycle _entity;

    private void Awake() {
        _characterDataProvider = GetComponent<CharacterDataProvider>();
        _entity = GetComponent<EntityLifecycle>();
        _characterDataProvider.CharacterDataChanged += (CharacterData newData) => {
            _characterNameText.text = newData.Name;
        };
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Локальному персонажу не нужна информация о себе.
        // netId униикален для сетевых gameObject
        if (isLocalPlayer && netId == NetworkClient.localPlayer.netId) {
            Disable();
        } else {
            BindFloatingInfo();
        }
    }

    private void Disable() {
        this.enabled = false;
        _floatingInfo.SetActive(false);
    }

    private void BindFloatingInfo() {
        SetHealthText(_entity.Parameters[LifecycleParameterEnum.Health].Value);
        _entity.Parameters[LifecycleParameterEnum.Health].OnValueChanged +=
            (old, newVal) => {
                SetHealthText(newVal);
            };
        void SetHealthText(float val) {
            _characterHealthText.text = string.Format("{0:F2}", System.Math.Round(val, 2));
        }
    }

    private void Update() {
        if (!isLocalPlayer)
            _floatingInfo.transform.LookAt(Camera.main.transform);
    }
}
