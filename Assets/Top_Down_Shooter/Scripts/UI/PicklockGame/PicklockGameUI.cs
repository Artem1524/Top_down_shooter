using System;
using System.Collections;
using UnityEngine;

namespace TDShooter.UI.PicklockGame
{
    public class PicklockGameUI : MonoBehaviour
    {
        public event Action OnPicklockGameFinished;

        [SerializeField]
        private float _WaitTimeAfterPicklockFinished = 1f;
        [SerializeField]
        private Transform _gearFinishPoint;
        [SerializeField]
        private Transform _gearStartPoint;
        [SerializeField]
        private PicklockGameGearObj _gearObj;

        [SerializeField, Range(0f, 10f), Space]
        private float _progressDepleteSpeedPerSec = 0.05f;
        [SerializeField, Range(0.01f, 1f)]
        private float _progressGainedOnButtonPressed = 0.05f;

        private float _progress = 0.5f;
        public bool IsNeedPicklock { get; private set; } = true;

        private void Awake()
        {
            _gearObj = GetComponentInChildren<PicklockGameGearObj>();
            DebugUtility.HandleErrorIfNullGetComponentInChildren<PicklockGameGearObj, PicklockGameUI>(_gearObj, this, gameObject);
        }

        private void Update()
        {
            DecreaseProgress();
            UpdateGearPositionY();
        }

        private void OnEnable()
        {
            IsNeedPicklock = true;
            _progress = 0f;
            UpdateGearPositionY();
        }

        private void UpdateGearPositionY()
        {
            if (_progress == 1f)
                return;

            if (_progress == 0f)
                _gearObj.transform.position = _gearStartPoint.position;
            else
                _gearObj.transform.position = _gearStartPoint.position + _progress * GetVectorBetweenTwoPoints();
        }

        private void DecreaseProgress()
        {
            if (_progress == 0f || !IsNeedPicklock)
                return;

            _progress -= _progressDepleteSpeedPerSec * Time.deltaTime;

            if (_progress < 0f)
                _progress = 0f;
        }

        public void PushButtonPressed()
        {
            _progress += _progressGainedOnButtonPressed;

            if (_progress >= 1f)
            {
                _progress = 1f;
                IsNeedPicklock = false;
                UpdateGearPositionY();
                StartCoroutine(FinishGameCoroutine());
            }
        }

        private IEnumerator FinishGameCoroutine()
        {
            yield return new WaitForSeconds(_WaitTimeAfterPicklockFinished);
            OnPicklockGameFinished?.Invoke();
            yield return null;
        }

        public void SetIsNeedPicklock(bool isNeedPicklock)
        {
            IsNeedPicklock = isNeedPicklock;
        }

        private Vector3 GetVectorBetweenTwoPoints()
        {
            return _gearFinishPoint.position - _gearStartPoint.position;
        }
    }
}