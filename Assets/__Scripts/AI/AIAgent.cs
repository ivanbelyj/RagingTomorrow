using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый класс моделей интеллектуального перемещения. Предназначен для основных расчетов
/// </summary>
public class AIAgent : MonoBehaviour
{
    [SerializeField]
    private float maxAcceleration;
    public float MaxAcceleration {
        get => maxAcceleration;
        set => maxAcceleration = value;
    }
    [SerializeField]
    private float maxSpeed;
    public float MaxSpeed {
        get => maxSpeed;
        set => maxSpeed = value;
    }

    public float Orientation { get; set; }
    public float RotationSpeed { get; private set; }
    public Vector3 Velocity { get; private set; } = Vector3.zero;

    [SerializeField] private float maxRotationSpeed;
    public float MaxRotationSpeed {
        get => maxRotationSpeed;
        set => maxRotationSpeed = value;
    }
    [SerializeField] private float maxAngularAcceleration;
    public float MaxAngularAcceleration {
        get => maxAngularAcceleration;
        set => maxAngularAcceleration = value;
    }

    protected AISteering steering = new AISteering();
    public void SetSteering(AISteering steering) {
        this.steering = steering;
    }

    public virtual void Update() {
        // Обработка перемещения в соотв. с текущими значениями
        Vector3 displacement = Velocity * Time.deltaTime;
        Orientation += RotationSpeed * Time.deltaTime;
        if (Orientation < 0f) {
            Orientation += 360f;
        } else if (Orientation > 360f) {
            Orientation -= 360f;
        }
        transform.Translate(displacement, Space.World);
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, Orientation);
    }

    public virtual void LateUpdate() {
        // Обновление управляющих воздействий для следующего кадра
        Velocity += steering.Linear * Time.deltaTime;
        Orientation += steering.Angular * Time.deltaTime;
        if (Velocity.magnitude > MaxSpeed) {
            Velocity = Velocity.normalized * MaxSpeed;
        }
        // Todo: Нужно ли ограничивать orientation?

        if (steering.Angular == 0f) {
            RotationSpeed = 0f;
        }
        if (steering.Linear.sqrMagnitude == 0f) {
            Velocity = Vector3.zero;
        }
        steering = new AISteering();
    }
}
