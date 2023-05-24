using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILeave : AIBehaviour
{
    [Tooltip("Радиус, которого агент избегает с максимальной скоростью")]
    [SerializeField] float escapeRadius;

    [Tooltip("Радиус, на котором агент избегает цели, но замедляется")]
    [SerializeField] float dangerRadius;
    [SerializeField] float timeToTarget = 0.1f;

    public override AISteering GetSteering()
    {
        AISteering steering = new AISteering();
        Vector3 dir = transform.position - Target.transform.position;
        float distance = dir.magnitude;

        float reduce;
        if (distance <= escapeRadius) {
            reduce = 0f;
        }
        else if (distance <= dangerRadius) {
            // Опасность есть, но можно замедляться
            reduce = distance / dangerRadius * agent.MaxSpeed;
        }
        else {
            // Опасность слишком далеко
            return steering;
        }

        float targetSpeed = agent.MaxSpeed - reduce;

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
