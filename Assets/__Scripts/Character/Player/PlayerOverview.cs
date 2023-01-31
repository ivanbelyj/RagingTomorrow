using System.Collections;
using System.Collections.Generic;
using AFPC;
using Mirror;
using UnityEngine;
using AppearanceCustomization3D;

[RequireComponent(typeof(Entity))]
public class PlayerOverview : NetworkBehaviour, ISetCameraOffset
{
    [SerializeField]
    private Overview _overview;
    public Overview Overview => _overview;
    private Entity _entity;

    private void Awake() {
        _entity = GetComponent<Entity>();
    }

    public void InitializeOnLocalPlayer()
    {
        // Для большей плавности
        QualitySettings.vSyncCount = 0;
        Cursor.lockState = CursorLockMode.Locked;
        _overview.camera = Camera.main;
    }

    private void Update() {
        if (!_entity.IsAlive) {
            return;
        }

        ReadInput();

        _overview.Looking();
        _overview.Aiming();
        _overview.Shaking();
    }

    private void FixedUpdate() {
        if (!_entity.IsAlive)
            return;

        // Физическое вращение камеры
        _overview.RotateRigigbodyToLookDirection(_entity.movement.rb);
    }

    private void LateUpdate() {
        if (!_entity.IsAlive)
            return;

        _overview.Follow(transform.position);
    }

    private void ReadInput() {
        _overview.lookingInputValues.x = Input.GetAxis("Mouse X");
        _overview.lookingInputValues.y = Input.GetAxis("Mouse Y");

        _overview.aimingInputValue = Input.GetMouseButton(1);
    }

    public void SetCameraOffset(Vector3 pos)
    {
        _overview.cameraOffset = pos;
    }
}
