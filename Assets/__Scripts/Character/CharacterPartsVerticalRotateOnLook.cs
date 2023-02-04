using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Позволяет вращать по оси X заданные Transform, относящиеся к персонажу,
/// а также кости персонажа (поворачиваются особо) в динамически определяемом направлении, 
/// покадрово вызывая метод Look
/// </summary>
[System.Serializable]
public class CharacterPartsVerticalRotateOnLook
{
    [SerializeField]
    private Animator _animator;
    public Animator Animator {
        get => _animator;
        set => _animator = value;
    }
    
    [SerializeField]
    private Vector3 _target;
    /// <summary>
    /// Точка в пространстве, в которую направлен персонаж
    /// </summary>
    public Vector3 Target {
        get => _target;
        set => _target = value;
    }

    [SerializeField]
    private List<HumanBodyBones> _bonesToRotate = new List<HumanBodyBones> {
        HumanBodyBones.UpperChest };

    [SerializeField]
    private List<Transform> _transformsToRotate;

    /// <summary>
    /// Для каждого Transform (включая кости) сохраняются изначальные значения поворотов
    /// </summary>
    // private Dictionary<Transform, Quaternion> _transformsAndInitialRotations
    //     = new Dictionary<Transform, Quaternion>();

    public void AddTransformToRotate(Transform t) {
        _transformsToRotate.Add(t);
        // AddTransformAndInitialRotation(t);
    }

    // private void AddTransformAndInitialRotation(Transform t) {
    //     _transformsAndInitialRotations.Add(t, t.rotation);
    // }

    /// <summary>
    /// Направляет части к заданному Target
    /// </summary>
    public void Look() {
        foreach (var bone in _bonesToRotate) {
            // RotateBone(bone);
            RotateTransform(_animator.GetBoneTransform(bone));
        }
        foreach (var transform in _transformsToRotate) {
            RotateTransform(transform);
        }
    }

    // private void RotateBone(HumanBodyBones bone) {
    //     Transform boneTransform = _animator.GetBoneTransform(bone);
    //     _animator.SetBoneLocalRotation(bone, GetLocalRotation(boneTransform));
    // }

    private void RotateTransform(Transform t) {
        Quaternion offset = GetOffset(t);
        t.localRotation = t.localRotation * offset;
        if (((int)Time.time) % 2 == 0) {

            // offset.ToAngleAxis(out float angle, out Vector3 axis);
            // Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.left);
            // t.rotation = newRotation;
        } else {

        }
    }

    private Quaternion GetOffset(Transform t) {
        var dir = _target - t.position;
        Quaternion rotToTarget = Quaternion.LookRotation(_target);
        Quaternion rotToTargetOnlyX = Quaternion.Euler(rotToTarget.eulerAngles.x, 0, 0);
        return rotToTargetOnlyX; // Quaternion.Euler(45, 0, 0);
        // return q * Quaternion.Inverse(oldRotation);
    }
}
 