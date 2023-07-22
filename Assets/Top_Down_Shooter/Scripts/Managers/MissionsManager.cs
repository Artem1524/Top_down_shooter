using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TDShooter.Managers
{
    public class MissionsManager : MonoBehaviour
    {
        private const float WAIT_TIME_BEFORE_SHOP_LEVEL = 3f;
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
            _mission.Initialize();
            UpdateMissionInfo(_missionInfo);
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

            UpdateMissionInfo(_missionInfo);

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

        private void UpdateMissionInfo(Text missionInfo)
        {
            missionInfo.text = _mission.GetMissionInfo();
        }

        private void NextLevel()
        {
            DebugUtility.DebugMessage(this, "Миссия выполнена");
            SettingsManager.IncreaseCurrentLevelIndex();
            StartCoroutine(ShopLevelCoroutine());
        }

        private IEnumerator ShopLevelCoroutine()
        {
            yield return new WaitForSeconds(WAIT_TIME_BEFORE_SHOP_LEVEL);
            LevelManager.LoadShopLevel();
            yield return null;
        }
    }
}