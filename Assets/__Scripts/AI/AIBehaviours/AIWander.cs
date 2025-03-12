using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWander : AIFace
{
    [Tooltip("Максимальное возможное смещение радиуса")]
    public float offset;

    [Tooltip("Определяет, насколько далеко могут находиться возможные позиции, в которые может "
        + "направляться агент при случайном блуждании")]
    public float radius;

    [Tooltip("Определяет, в каком диапазоне может измениться поворот при случайном блуждании")]
    public float orientationRate;

    public override void Awake()
    {
        Target = new GameObject("WanderTarget");
        Target.transform.position = transform.position;
        base.Awake();
    }

    public override AISteering GetSteering()
    {
        // Случайный поворот
        float wanderOrientation = Random.Range(-1f, 1f) * orientationRate;
        float newOrientation = wanderOrientation + agent.Orientation;

        // Случайная позиция, в которую пойдет агент
        Vector3 targetPos = (offset * OrientationToVector(agent.Orientation)) + transform.position;
        targetPos = targetPos + (OrientationToVector(newOrientation) * radius);

        // DrawRay(targetActual.transform.position, targetPos);
        targetActual.transform.position = targetPos;

        AISteering steering = base.GetSteering();
        steering.Linear = (targetActual.transform.position - transform.position).normalized
            * agent.MaxAcceleration;

        return steering;
    }

    private void DrawRay(Vector3 start, Vector3 end) {
        GameObject lineObj = new GameObject("Line");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
