using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый класс модели поведения
/// </summary>
[RequireComponent(typeof(AIAgent))]
public class AIBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    public virtual GameObject Target { get => target; set => target = value; }
    protected AIAgent agent;

    public virtual void Awake() {
        agent = GetComponent<AIAgent>();
    }
    public virtual void Update() {
        agent.SetSteering(GetSteering());
    }

    public virtual AISteering GetSteering() {
        return new AISteering();
    }

    /// <summary>
    /// Возвращает значение поворота в диапазоне [-180f; 180f]
    /// </summary>
    public static float MapOrientationToRange180(float orientation) {
        orientation %= 360f;
        if (Mathf.Abs(orientation) > 180f) {
            if (orientation < 0f) {
                orientation += 360f;
            } else {
                orientation -= 360f;
            }
        }
        return orientation;
    }

    /// <summary>
    /// Преобразование направления в вектор
    /// </summary>
    public static Vector3 OrientationToVector(float orientation) {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1f;
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1f;
        return vector.normalized;
    }
}
