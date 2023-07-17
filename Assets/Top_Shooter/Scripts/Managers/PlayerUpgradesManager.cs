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

        private void Start()
        {
            if (Self is null)
            {
                Self = this;
                OnChangeMoney += MoneyChanged;
                OnChangeMoney?.Invoke(Money);
                InitializeWeaponsBaseStats(_weaponUpgradeLevels, WeaponManager.GetWeaponTypes());
            }
        }

        public static void AddMoney(int count)
        {
            Self.Money += count;
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
                Self._weaponUpgradeLevels[weaponType] = currentLevel + 1;
        }

        public static int GetWeaponUpgradeLevelsCount(WeaponType weaponType)
        {
            return WeaponManager.GetWeaponUpgradeLevelsCount(weaponType);
        }

        private void MoneyChanged(int money)
        {
            _moneyInfoPanel.text = money.ToString();
        }

        private void InitializeWeaponsBaseStats(Dictionary<WeaponType, int> weaponUpgradeLevels, List<WeaponType> weaponTypes)
        {
            foreach (WeaponType weaponType in weaponTypes)
            {
                weaponUpgradeLevels.TryAdd(weaponType, 0);
                WeaponManager.SetWeaponStatsByLevel(weaponType, 0);
            }
        }

        public static int GetWeaponCurrentLevel(WeaponType weaponType)
        {
            Self._weaponUpgradeLevels.TryGetValue(weaponType, out int currentLevel);
            return currentLevel;
        }
    }
}