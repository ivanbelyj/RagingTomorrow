using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using System;

/// <summary>
/// Компонент, предоставляющий возможность взаимодействовать с интерактивным объектом
/// </summary>
public class Interactor : NetworkBehaviour
{
    /// <summary>
    /// Для вызова событий LookedOnObject и LookedAwayFromObject недостаточно знать только
    /// соответствующий каждой стратегии collider, который был доступен для взаимодействия в
    /// последнем кадре (грубо говоря, увиден). Случай, когда collider равен null, ведет
    /// к неоднозначности, т.к.
    /// указывает на одну из двух ситуаций:
    /// 1) в последнем кадре был "увиден" предмет, недоступный для взаимодействия данной стратегии
    /// 2) collider был установлен в null движком, т.к. интерактивный объект был удален из сцены
    /// 3) Объект изначально не был установлен
    /// </summary>
    private class InteractableInLastFrame {
        public enum CauseOfNull {
            Destroyed, // Если причина установки null не указана явно, то collider был уничтожен
            NotInteractiveOrUndefined,
        }
        public Collider Collider { get; private set; }

        // Неоднозначность избегается с помощью явного указания ситуации, причины null
        public CauseOfNull Cause { get; set; }
        public InteractableInLastFrame(Collider col)
        {
            Collider = col;
            Cause = CauseOfNull.Destroyed;
        }
        public InteractableInLastFrame(CauseOfNull cause)
        {
            Collider = null;
            Cause = cause;
        }
    }

    /// <summary>
    /// Максимальное расстояние, на котором можно взаимодействовать с интерактивными предметами
    /// </summary>
    public float interactRadius;
    // private IInteractorStrategy[] _strategies;
    private Transform _targetTransform;

    private bool _isInitialized = false;

    /// <summary>
    /// Пара словаря - стратегия, а также коллайдер увиденного в последнем кадре объекта,
    /// с которым стратегия может взаимодействовать. null, если
    /// в последнем кадре для стратегии был увиден объект, с которым она не может
    /// взаимодействовать.
    /// Также используется для хранения всех стратегий Interactor'а.
    /// Строго говоря, корректнее называть объекты не увиденными, а "потенциально используемыми"
    /// </summary>
    private Dictionary<IInteractorStrategy, InteractableInLastFrame> _lastSeen;
    private void SetLastSeenCanInteract(IInteractorStrategy strategy, Collider seenInLastFrame)
    {
        // Nullability
        // 4 случая
        // Если увиден тот же самый коллайдер (или null == null)
        if (_lastSeen[strategy].Collider == seenInLastFrame) {
            // Наименее очевидный случай: _lastSeen[strategy] был уничтожен в сцене,
            // а после этого новых объектов для взаимодействия не оказалось.
            // Если не обрабатывать этот случай отдельно,
            // то strategy.LookedAwayFromObject() не будет вызван
            if (_lastSeen[strategy].Collider == null &&
                _lastSeen[strategy].Cause == InteractableInLastFrame.CauseOfNull.Destroyed) {
                // Помимо всего прочего это один из тех случаев, когда is null и == null
                // работают по-разному
                Debug.Log("Object was destroyed, looked away");
                strategy.LookedAwayFromObject();
                // Обрабатывать ситуацию повторно не имеет смысла
                _lastSeen[strategy].Cause
                    = InteractableInLastFrame.CauseOfNull.NotInteractiveOrUndefined;
            }
            // В остальных случаях ничего не нужно делать
            return;
        }

        // Код немного дублируется, но такой ценой легче анализировать и разделять условия

        // Если до этого момента Interactor видел объект (не null), а теперь
        // с последним увиденным объектом нельзя взаимодействовать
        if (_lastSeen[strategy].Collider is not null && seenInLastFrame is null) {
            // Interactor отвел взгляд от объекта
            strategy.LookedAwayFromObject();
            _lastSeen[strategy] = new InteractableInLastFrame(
                InteractableInLastFrame.CauseOfNull.NotInteractiveOrUndefined);
            return;
        }

        // Если Interactor не видел объект, а с новым увиденным можно взаимодействовать
        if (_lastSeen[strategy].Collider is null && seenInLastFrame is not null) {
            // Interactor посмотрел на объект
            strategy.LookedAtObject(seenInLastFrame);
            _lastSeen[strategy] = new InteractableInLastFrame(seenInLastFrame);
            return;
        }

        // Not null values
        // Если игрок посмотрел на иной объект, с которым можно взаимодействовать
        if (_lastSeen[strategy].Collider != seenInLastFrame) {
            // Если до этого он видел объект, то теперь отвел взгляд
            strategy.LookedAwayFromObject();

            // И посмотрел на новый
            strategy.LookedAtObject(seenInLastFrame);
            _lastSeen[strategy] = new InteractableInLastFrame(seenInLastFrame);
            return;
        }
    }

    // Start вызывается на тот момент, когда isLocalPlayer уже установлено.
    private void Start() {
        string side = isLocalPlayer ? "local player" : "NOT local player";
        Debug.Log("Interactor starts on " + side);
        // Логика взаимодействия имеет смысл только на локальном устройстве
        if (!isLocalPlayer) {
            this.enabled = false;
            return;
        }

        _lastSeen = new Dictionary<IInteractorStrategy, InteractableInLastFrame>();
        foreach(var strategy in GetComponents<IInteractorStrategy>()) {
            _lastSeen.Add(strategy, new InteractableInLastFrame(
                InteractableInLastFrame.CauseOfNull.NotInteractiveOrUndefined));
        }
    }

    /// <summary>
    /// Transfrom GameObject'а, к которому прикреплен ItemInteractor, может не быть направленным
    /// в сторону предмета, с которым требуется взаимодействовать, этим и обусловлена необходимость
    /// в целевом transform
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

        CheckInteractables(interact: true);
        // Вариант без перепроверки всего, с чем вокруг можно повзаимодействовать
        // foreach (var pair in _lastSeen) {
        //     if (pair.Value is not null) {
        //         // Значит, в последний кадр было, с чем повзаимодействовать
        //         pair.Key.InteractObject(pair.Value);
        //     }
        // }
    }

    private void Update() {
        if (!_isInitialized) {
            Debug.Log("Interactor is not initialized to interact!");
            return;
        }
        CheckInteractables(interact: false);
    }

    /// <summary>
    /// Если есть доступные объекты для взаимодействия, выполняется действие с тем из них,
    /// который наиболее соответствует направлению "взгляда" Interactor
    /// </summary>
    private void CheckInteractables(bool interact) {
        Collider[] colliders = Physics.OverlapSphere(_targetTransform.position, interactRadius);
        if (colliders is null || colliders.Length == 0)
            return;
        
        Collider toInteract = null;
        float maxDot = float.MinValue;
        foreach (Collider col in colliders) {
            // Если ни одна стратегия взаимодействия не может быть применена
            if (_lastSeen.Keys.All(strategy => !strategy.CanInteract(col))) {
                continue;
            }

            // Вектор от interactor до интерактивного предмета
            Vector3 directionFromInteractor = col.transform.position - _targetTransform.position;

            float dot = Vector3.Dot(_targetTransform.forward, directionFromInteractor.normalized);
            // Debug.Log("dot with " + col.gameObject.name + " is " + dot);

            // Если interactor направлен к интерактивному объекту и объект наиболее близок
            // к направлению (на данный момент)
            if (dot > 0.5f && dot > maxDot) {
                toInteract = col;
                maxDot = dot;
            }
        }

        var strategies = _lastSeen.Keys.ToArray();
        foreach (var strategy in strategies) {
            if (toInteract is not null && strategy.CanInteract(toInteract)) {
                SetLastSeenCanInteract(strategy, toInteract);
                if (interact) {
                    strategy.InteractObject(toInteract);
                }
            } else {
                SetLastSeenCanInteract(strategy, null);
            }
        }
    }
}
