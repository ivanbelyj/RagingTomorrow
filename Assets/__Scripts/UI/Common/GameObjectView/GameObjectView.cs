using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Компонент отвечает за отображение GameObject в UI
/// </summary>
[RequireComponent(typeof(RawImage))]
public class GameObjectView : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// Слой, который будет использован для отображения GameObject
    /// </summary>
    private string _layerName;

    [SerializeField]
    /// <summary>
    /// Камера, которая будет использована для отображения GameObject
    /// </summary>
    private Camera _camera;

    [SerializeField]
    /// <summary>
    /// Отступ камеры от целевого GameObject
    /// </summary>
    private Vector3 _cameraOffset = Vector3.forward * 3;

    [SerializeField]
    private RenderTexture _renderTexture;

    private RawImage _rawImage;

    private GameObject _target;

    [SerializeField]
    /// <summary>
    /// Камера следит за положением GameObject, смещенным на определенную величину
    /// </summary>
    private Vector3 _lookAtTargetOffset;

    /// <summary>
    /// Если true, то камера будет перемещаться вслед за целью
    /// </summary>
    private bool _followTarget = false;

    private void Awake() {
        _rawImage = GetComponent<RawImage>();
    }

    /// <summary>
    /// Устанавливает GameObject, который будет отображаться
    /// </summary>
    public void SetGameObject(GameObject go) {
        _target = go;
        _followTarget = true;
        int layerNumber = LayerMask.NameToLayer(_layerName);
        SetLayerRecursively(go, layerNumber);
        _camera.cullingMask = 1 << layerNumber;
        _camera.targetTexture = _renderTexture;
        _rawImage.texture = _renderTexture;
    }

    private void SetLayerRecursively(GameObject go, int layerNumber) {
        go.layer = layerNumber;
        for (int i = 0; i < go.transform.childCount; i++) {
            SetLayerRecursively(go.transform.GetChild(i).gameObject, layerNumber);
        }
    }

    private void LateUpdate() {
        if (_followTarget) {
            _camera.transform.position = _target.transform.position + _cameraOffset;
            _camera.transform.LookAt(_target.transform.position + _lookAtTargetOffset);
        }
    }
}
