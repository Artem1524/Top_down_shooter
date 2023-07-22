using System.Collections.Generic;
using UnityEngine;

using TDShooter.Weapons;

namespace TDShooter.Managers
{
    public class SettingsManager : MonoBehaviour
    {
        private const string CURRENT_LEVEL_KEY_NAME = "CurrentLevel";
        private const string MONEY_KEY_NAME = "Money";
        private const string WEAPON_KEY_NAME_PREFIX = "Weapon_";
        private const int DEFAULT_LEVEL_INDEX = 1;

        public static void SetCurrentLevelIndex(int currentLevelIndex)
        {
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY_NAME, currentLevelIndex);
        }

        public static void IncreaseCurrentLevelIndex()
        {
            int currentLevel = GetCurrentLevelIndex(); // —охран€ть значение локально в каком-либо классе
            SetCurrentLevelIndex(currentLevel + 1);
        }

        public static void SavePlayerData()
        {
            PlayerPrefs.SetInt(MONEY_KEY_NAME, PlayerUpgradesManager.Self.Money);

            SaveWeaponLevelsData();
        }

        public static int GetMoney()
        {
            return PlayerPrefs.GetInt(MONEY_KEY_NAME, 0);
        }

        public static int GetCurrentLevelIndex()
        {
            return PlayerPrefs.GetInt(CURRENT_LEVEL_KEY_NAME, DEFAULT_LEVEL_INDEX);
        }

        private static void SaveWeaponLevelsData()
        {
            Dictionary<WeaponType, int> weaponLevels = PlayerUpgradesManager.GetWeaponLevels();

            foreach (KeyValuePair<WeaponType, int> pair in weaponLevels)
            {
                PlayerPrefs.SetInt(GetWeaponKeyNameWithPrefix(pair.Key), pair.Value);
            }
        }

        public static void LoadWeaponLevelsData()
        {
            List<WeaponType> weaponTypes = WeaponManager.GetWeaponTypes();
            
            Dictionary<WeaponType, int> weaponLevels = PlayerUpgradesManager.GetWeaponLevels();

            weaponLevels.Clear(); // fix

            foreach (WeaponType weaponType in weaponTypes)
            {
                weaponLevels[weaponType] = PlayerPrefs.GetInt(GetWeaponKeyNameWithPrefix(weaponType), 0);
            }
        }

        private static string GetWeaponKeyNameWithPrefix(WeaponType weaponType)
        {
            return WEAPON_KEY_NAME_PREFIX + weaponType.ToString();
        }
    }
}