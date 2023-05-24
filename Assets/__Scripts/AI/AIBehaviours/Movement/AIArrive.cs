using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIArrive : AIBehaviour
{
    [SerializeField] private float targetRadius;
    [SerializeField] private float slowRadius;
    [SerializeField] private float timeToTarget = 0.1f;

    public override AISteering GetSteering()
    {
        // Вычисление скорости в зависимости от расстояния до цели
        // и радиуса замедления
        AISteering steering = new AISteering();
        Vector3 dir = Target.transform.position - transform.position;
        float distance = dir.magnitude;

        // Агент пришел к цели
        if (distance <= targetRadius) {
            return steering;
        }
        // Агент замедляется
        float targetSpeed;
        if (distance <= slowRadius) {
            // Скорость уменьшается при приближении
            targetSpeed = distance / slowRadius * agent.MaxSpeed;
            Debug.Log($"Замедление. targetSpeed: {targetSpeed}");
        } else {
            targetSpeed = agent.MaxSpeed;
        }

        // Установка управляющих значений
        Vector3 desiredVelocity = dir.normalized * targetSpeed;
        steering.Linear = desiredVelocity - agent.Velocity;
        steering.Linear /= timeToTarget;

        if (steering.Linear.magnitude > agent.MaxAcceleration) {
            steering.Linear.Normalize();
            steering.Linear *= agent.MaxAcceleration;
        }

        return steering;
    }
}
