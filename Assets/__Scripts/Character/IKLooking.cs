using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKLooking : MonoBehaviour
{
    [SerializeField]
    private float _weight;
    [SerializeField]
    private float _bodyWeight;
    [SerializeField]
    private float _headWeight;
    [SerializeField]
    private float _eyesWeight;
    [SerializeField]
    private float _clampWeight;
    private Animator _animator;

    public Vector3 Target { get; set; }

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    private void OnAnimatorIK() {
        _animator.SetLookAtWeight(_weight, _bodyWeight, _headWeight, _eyesWeight, _clampWeight);
        _animator.SetLookAtPosition(Target);
        SetIK(AvatarIKGoal.LeftHand);
        SetIK(AvatarIKGoal.RightHand);
    }

    private void SetIK(AvatarIKGoal ikGoal) {
        _animator.SetIKPositionWeight(ikGoal, 1f);
        _animator.SetIKPosition(ikGoal, Target);
    }
}
