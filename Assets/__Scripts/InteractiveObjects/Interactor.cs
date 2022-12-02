using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using System;

/// <summary>
/// Компонент, предоставляющий возможность взаимодействовать с интерактивным объектом
/// </summary>
public class Interactor : MonoBehaviour
{
    /// <summary>
    /// Максимальное расстояние, на котором можно взаимодействовать с интерактивными предметами
    /// </summary>
    public float interactRadius;

    private Transform _targetTransform;
    private bool _isInitialized = false;

    // public event Action<Collider> LookingToInteractiveObject;

    private IInteractorStrategy[] _strategies;

    private void Awake() {
        _strategies = GetComponents<IInteractorStrategy>();
    }

    /// <summary>
    /// Transfrom GameObject'а, к которому прикреплен ItemInteractor, может не быть направленным
    /// в сторону предмета, с которым требуется взаимодействовать
    /// </summary>
    public void Initialize(Transform targetTransform) {
        _targetTransform = targetTransform;
        _isInitialized = true;
    }

    public void Interact() {
        if (!_isInitialized) {
            Debug.LogError("Interactor is not initialized to interact!");
            return;
        }
        ActionWithInteractables((strategy, collider) => {
            strategy.InteractObject(collider);
        });
    }

    virtual protected void Update() {
        if (!_isInitialized) {
            Debug.Log("Interactor is not initialized to interact!");
            return;
        }
        ActionWithInteractables((strategy, collider) => {
            strategy.LookToObject(collider);
        });
    }

    /// <summary>
    /// Если есть доступные объекты для взаимодействия, выполняется действие с тем из них,
    /// который наиболее соответствует направлению "взгляда" Interactor
    /// </summary>
    private void ActionWithInteractables(
        Action<IInteractorStrategy, Collider> actionForEachStrategy) {
        Collider[] colliders = Physics.OverlapSphere(_targetTransform.position, interactRadius);
        if (colliders is null || colliders.Length == 0)
            return;
        
        Collider toInteract = null;
        float maxDot = float.MinValue;
        foreach (Collider col in colliders) {
            // Если ни одна стратегия взаимодействия не может быть применена
            if (_strategies.All(strategy => !strategy.CanInteract(col))) {
                continue;
            }

            // Вектор от interactor до интерактивного предмета
            Vector3 directionFromInteractor = col.transform.position - _targetTransform.position;
            Debug.DrawRay(_targetTransform.position, directionFromInteractor, Color.blue);

            float dot = Vector3.Dot(_targetTransform.forward, directionFromInteractor.normalized);
            // Debug.Log("dot with " + col.gameObject.name + " is " + dot);

            // Если interactor направлен к интерактивному объекту и объект наиболее близок
            // к направлению (на данный момент)
            if (dot > 0.5f && dot > maxDot) {
                toInteract = col;
                maxDot = dot;
            }
        }

        if (toInteract is not null) {
            foreach (var strategy in _strategies) {
                if (strategy.CanInteract(toInteract)) {
                    actionForEachStrategy(strategy, toInteract);
                }
            }
        }
    }

    // protected virtual bool CanInteract(Collider col) {
    //     return col.GetComponent<IInteractable>() is not null;
    // }

    // protected virtual void InteractObject(Collider col) {
    //     col.GetComponent<IInteractable>().Interact();
    // }
}
