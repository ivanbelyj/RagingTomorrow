using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFlee : AIBehaviour
{
    public override AISteering GetSteering()
    {
        AISteering steering = new AISteering();
        steering.Linear = transform.position - Target.transform.position;
        steering.Linear.Normalize();
        steering.Linear *= agent.MaxAcceleration;
        return steering;
    }
}
