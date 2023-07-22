using UnityEngine;
using UnityEngine.UI;

using TDShooter.Weapons;
using System.Collections.Generic;
using System;

namespace TDShooter.Managers
{
    public class PlayerUpgradesManager : MonoBehaviour
    {
        public static PlayerUpgradesManager Self { get; private set; }
        public int Money { get; private set; } = 0;

        public event Action<int> OnChangeMoney;

        [SerializeField]
        private Text _moneyInfoPanel;

        private Dictionary<WeaponType, int> _weaponUpgradeLevels = new Dictionary<WeaponType, int>(); // Заменить int на структуру WeaponUpgrades, если необходимо улучшать некоторые параметры оружия по отдельности

        private void OnEnable()
        {
            if (Self is null)
            {
                Self = this;
            }

            OnChangeMoney += MoneyChanged;
        }

        private void Start()
        {
            SetMoney(SettingsManager.GetMoney()); //SetMoney(500);
        }

        public static void AddMoney(int count)
        {
            Self.Money += count;
            Self.OnChangeMoney?.Invoke(Self.Money);
        }

        public static void SetMoney(int money)
        {
            Self.Money = money;
            Self.OnChangeMoney?.Invoke(Self.Money);
        }

        public static void RemoveMoney(int count)
        {
            if (Self.Money >= count)
                Self.Money -= count;
            else
                Self.Money = 0; // Добавить логирование, когда денег недостаточно, но покупка успешна

            Self.OnChangeMoney?.Invoke(Self.Money);
        }

        public static void AddWeaponUpgrade(WeaponType weaponType)
        {
            int maxWeaponLevel = GetWeaponUpgradeLevelsCount(weaponType);
            Self._weaponUpgradeLevels.TryGetValue(weaponType, out int currentLevel); // Логирование

            if (maxWeaponLevel > currentLevel)
            {
                int newLevel = currentLevel + 1;
                Self._weaponUpgradeLevels[weaponType] = newLevel;
                Self.UpdateWeaponStats(weaponType, newLevel);
            }
        }

        public static int GetWeaponUpgradeLevelsCount(WeaponType weaponType)
        {
            return WeaponManager.GetWeaponUpgradeLevelsCount(weaponType);
        }

        private void MoneyChanged(int money)
        {
            _moneyInfoPanel.text = money.ToString();
        }

        /* private void InitializeWeaponsBaseStats(Dictionary<WeaponType, int> weaponUpgradeLevels)
        {
            foreach (KeyValuePair<WeaponType, int> pair in weaponUpgradeLevels)
            {
                UpdateWeaponStats(pair.Key, pair.Value);
            }
        }*/

        private void UpdateWeaponStats(WeaponType weaponType, int weaponLevel)
        {
            WeaponManager.SetWeaponStatsByLevel(weaponType, weaponLevel);
        }

        public static int GetWeaponCurrentLevel(WeaponType weaponType)
        {
            Self._weaponUpgradeLevels.TryGetValue(weaponType, out int currentLevel);
            return currentLevel;
        }

        public static Dictionary<WeaponType, int> GetWeaponLevels()
        {
            return Self._weaponUpgradeLevels;
        }
    }
}