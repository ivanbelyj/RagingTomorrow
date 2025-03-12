using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathSegment
{
    public Vector3 a;
    public Vector3 b;
    public AIPathSegment() : this(Vector3.zero, Vector3.zero) { }
    public AIPathSegment(Vector3 a, Vector3 b) {
        this.a = a;
        this.b = b;
    }
}
