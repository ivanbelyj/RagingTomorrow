using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFace : AIAlign
{
    protected GameObject targetActual;
    // private Agent targetAgent;

    public override void Awake()
    {
        base.Awake();
        targetActual = Target;
        Target = new GameObject("FaceTarget");
        Target.AddComponent<AIAgent>();
    }

    private void OnDestroy() {
        Destroy(Target);
    }

    public override AISteering GetSteering()
    {
        // Расстояние от агента до фактической цели
        Vector3 dir = targetActual.transform.position - transform.position;
        if (dir.magnitude > 0f) {
            // Todo: кеширование Agent
            float rotation = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            Target.GetComponent<AIAgent>().Orientation = rotation;
        }
        return base.GetSteering();
    }
}
