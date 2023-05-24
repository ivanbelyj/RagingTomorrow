using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Данные, используемые для перемещения и поворота агента
/// </summary>
public class AISteering
{
    public float Angular { get; set; } = 0f;
    public Vector3 Linear { get; set; } = new Vector3();
}
