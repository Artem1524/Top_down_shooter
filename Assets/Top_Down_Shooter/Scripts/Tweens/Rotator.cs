using System;
using UnityEngine;

namespace TDShooter.Tweens
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField, Range(0.2f, 30f)]
        private float _timeToRotation = 1f;

        [SerializeField]
        private Vector3 _rotationVector = Vector3.up * 360;

        private void Update()
        {
            Rotate(_rotationVector);
        }

        private void Rotate(Vector3 rotationVector)
        {
            Vector3 deltaVector = _rotationVector * Time.deltaTime / _timeToRotation;
            transform.Rotate(deltaVector);
        }
    }
}