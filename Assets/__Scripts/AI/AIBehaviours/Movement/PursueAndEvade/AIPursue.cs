using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPursue : AISeek
{
    [SerializeField]
    private float maxPrediction;
    public float MaxPrediction {
        get => maxPrediction;
        set => maxPrediction = value;
    }

    /// <summary>
    /// Target заменяется новым объектом, который следует по прогнозируемому пути
    /// (что облегчает достижение цели),
    /// а фактическая цель сохраняется в targetActual
    /// </summary>
    private GameObject targetActual;
    private AIAgent targetActualAgent;

    public override void Awake()
    {
        base.Awake();
        targetActual = Target;
        targetActualAgent = Target.GetComponent<AIAgent>();
        
        Target = new GameObject("PursueTarget");
    }

    private void OnDestroy() {
        Destroy(Target);
    }

    public override AISteering GetSteering()
    {
        // Направление от нас к оригинальной цели
        Vector3 dir = targetActual.transform.position - transform.position;
        // Расстояние
        float distance = dir.magnitude;
        // Скорость берется от агента цели
        float agentSpeed = agent.Velocity.magnitude;

        float prediction;
        if (agentSpeed <= distance / MaxPrediction) {
            prediction = MaxPrediction;
        } else {
            prediction = distance / agentSpeed;
        }

        // Позиция фактической цели берется за основу
        Target.transform.position = targetActual.transform.position;
        // И корректируется с учетом скорости агента цели
        Target.transform.position += targetActualAgent.Velocity * prediction;
        // Потом обычное преследование, только уже по скорректированной позиции
        return base.GetSteering();
    }
}
