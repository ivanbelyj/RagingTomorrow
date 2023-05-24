using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Выравнивание объекта так же, как и цель
/// </summary>
public class AIAlign : AIBehaviour
{
    [SerializeField] private float targetRadius;
    [SerializeField] private float slowRadius;
    [SerializeField] private float timeToTarget = 0.1f;

    public override AISteering GetSteering()
    {
        AISteering steering = new AISteering();

        float orientationDifference = Target.GetComponent<AIAgent>().Orientation
            - agent.Orientation;
        orientationDifference = MapOrientationToRange180(orientationDifference);
        float differenceAbs = Mathf.Abs(orientationDifference);

        // Агент уже повернут к цели
        if (differenceAbs <= targetRadius) {
            Debug.Log("Агент повернут к цели");
            return steering;
        }
        
        float newRotation;
        if (differenceAbs <= slowRadius) {
            // Замедление поворота
            newRotation = differenceAbs / slowRadius * agent.MaxRotationSpeed;
            Debug.Log($"{orientationDifference} <= {slowRadius}. Замедление");
        } else  // Максимально быстрый поворот
            newRotation = agent.MaxRotationSpeed;

        // Направление поворота
        newRotation *= orientationDifference / differenceAbs;
        
        steering.Angular = newRotation - agent.RotationSpeed;
        steering.Angular /= timeToTarget;

        float angularAccelAbs = Mathf.Abs(steering.Angular);
        // Ограничение максимальным ускорением
        if (angularAccelAbs > agent.MaxAngularAcceleration) {
            steering.Angular /= angularAccelAbs;
            steering.Angular *= agent.MaxAngularAcceleration;
        }
        return steering;
    }
}
