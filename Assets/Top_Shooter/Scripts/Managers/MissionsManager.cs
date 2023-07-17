using System;
using UnityEngine;
using UnityEngine.UI;

namespace TDShooter.Managers
{
    public class MissionsManager : MonoBehaviour
    {
        [SerializeField]
        private Mission _mission;
        [SerializeField, Range(0.02f, 10f)]
        private float _checkMissionUpdateInterval = 0.2f;

        [SerializeField, Space]
        private Text _missionInfo;

        private float _updateTimer = 0f;
        private bool _missionCompleted = false;

        private void Start()
        {
            _missionInfo.text = UpdateMissionInfo();
        }

        private void Update()
        {
            CheckTimer();
        }

        private void CheckTimer()
        {
            if (_missionCompleted)
                return;

            _updateTimer -= Time.deltaTime;

            if (_updateTimer > 0)
                return;

            _updateTimer = _checkMissionUpdateInterval; // _updateTimer <= 0

            if (_mission.CheckMission())
            {
                OnAllMissionsCompleted();
            }
        }

        private void OnAllMissionsCompleted()
        {
            _missionCompleted = true;
            NextLevel();
        }

        private string UpdateMissionInfo()
        {
            return _mission.GetMissionInfo();
        }

        private void NextLevel()
        {
            DebugUtility.DebugMessage(this, "Миссия выполнена");
        }
    }
}