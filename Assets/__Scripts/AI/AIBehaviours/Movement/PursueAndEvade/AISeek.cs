using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISeek : AIBehaviour
{
    public override AISteering GetSteering()
    {
        AISteering steering = new AISteering();
        steering.Linear = Target.transform.position - transform.position;
        steering.Linear.Normalize();
        steering.Linear *= agent.MaxAcceleration;
        return steering;
    }
}
