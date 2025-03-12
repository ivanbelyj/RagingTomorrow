using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathFollower : AISeek
{
    public AIPath path;
    public float pathOffset = 0f;
    private float currentParam;

    public override void Awake()
    {
        base.Awake();
        Target = new GameObject("PathFollower Target");
        currentParam = 0f;
    }

    public override AISteering GetSteering()
    {
        currentParam = path.GetParam(transform.position, currentParam);
        float targetParam = currentParam + pathOffset;
        Target.transform.position = path.GetPosition(targetParam);
        return base.GetSteering();
    }
}
