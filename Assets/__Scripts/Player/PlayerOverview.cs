using System.Collections;
using System.Collections.Generic;
using AFPC;
using UnityEngine;
using AppearanceCustomization3D;
using Mirror;

[RequireComponent(typeof(EntityLifecycle))]
// [RequireComponent(typeof(IKLooking))]
public class PlayerOverview : NetworkBehaviour, ISetCameraToAppearance
{
    [SerializeField]
    private Overview overview;

    [SerializeField]
    private CharacterPartsVerticalRotateOnLook characterLook;

    [SerializeField]
    private Rigidbody rigidbodyToRotate;

    public Overview Overview => overview;
    private EntityLifecycle entity;

    /// <summary>
    /// GameObject, в позицию которого перемещается камера
    /// </summary>
    private GameObject cameraPositionGO;

    /// </summary>
    /// GameObject для вращения позиции камеры. В данной реализации поворот позиции производится
    /// только по вертикали, по горизонтали - за счет RigidBody
    /// </summary>
    // private GameObject _cameraPositionRotateGO;

    // private IKLooking ikLooking;

    private void Awake() {
        entity = GetComponent<EntityLifecycle>();
        // ikLooking = GetComponent<IKLooking>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        // Для большей плавности
        QualitySettings.vSyncCount = 0;
        Cursor.lockState = CursorLockMode.Locked;
        overview.camera = Camera.main;
    }

    private void Update() {
        if (!isLocalPlayer)
            return;
        if (!entity.IsAlive) {
            return;
        }

        ReadInput();

        overview.Looking();
        overview.Aiming();
        overview.Shaking();

        // _ikLooking.Target = _overview.camera.transform.forward;
        characterLook.Target = overview.camera.transform.forward;
    }

    // private void OnAnimatorIK(int layerIndex) {
    //     _characterLook.Look();
    // }

    private void FixedUpdate() {
        if (!isLocalPlayer)
            return;
        if (!entity.IsAlive)
            return;

        overview.RotateRigigbodyToLookDirection(rigidbodyToRotate);
    }

    private void LateUpdate() {
        if (!isLocalPlayer)
            return;
        if (!entity.IsAlive || cameraPositionGO == null)
            return;

        characterLook.Target = overview.camera.transform.forward;
        characterLook.Look();

        overview.Follow(cameraPositionGO.transform.position);
    }

    private void ReadInput() {
        overview.lookingInputValues.x = Input.GetAxis("Mouse X");
        overview.lookingInputValues.y = Input.GetAxis("Mouse Y");

        overview.aimingInputValue = Input.GetMouseButton(1);
    }

    public void SetCamera(Transform bone, Vector3 cameraPos)
    {
        // Создание объекта, в позицию которого будет перемещаться камера,
        // и прикрепление его к кости
        
        cameraPositionGO = new GameObject("CameraPosition");
        cameraPositionGO.transform.SetParent(bone);
        cameraPositionGO.transform.localPosition = cameraPos;

        characterLook.AddTransformToRotate(cameraPositionGO.transform);
    }
}
