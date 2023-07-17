using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TDShooter.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        private const string PATH_TO_WEAPON_UPGRADE_CONFIGURATIONS = "Configurations/ScriptableObjects/WeaponUpgrades/";
        private readonly Quaternion ZERO_ROTATION = Quaternion.Euler(0, 0, 0);

        public static WeaponManager Self { get; private set; }

        public Action<WeaponShooter> OnSwitchingWeapon;

        [SerializeField]
        private List<WeaponShooter> _weapons;
        [SerializeField]
        private WeaponType _startingWeapon = WeaponType.Rifle;

#nullable enable
        [SerializeField]
        private WeaponSlot? _weaponParentSlot;
        [SerializeField]
        private Transform? _inactiveWeaponsSlot;
        [SerializeField]
        private BulletSpawn? _playerBulletSpawnPoint;

#nullable disable
        private Dictionary<WeaponType, WeaponUpgradeConfiguration> _weaponConfigurations = new Dictionary<WeaponType, WeaponUpgradeConfiguration>();

        private Dictionary<WeaponType, WeaponShooter> _weaponsByType = new Dictionary<WeaponType, WeaponShooter>();
        private Dictionary<WeaponShooter, BulletSpawn> _weaponBulletSpawners = new Dictionary<WeaponShooter, BulletSpawn>();
        private WeaponShooter _currentWeapon = null;

        private void Awake()
        {
            if (Self is null)
            {
                Self = this;
                LoadWeaponConfigurations(PATH_TO_WEAPON_UPGRADE_CONFIGURATIONS);
                OnSwitchingWeapon += OnWeaponSwitched;
                InitializeWeapons();
            }
        }

        private void InitializeWeapons()
        {
            foreach (WeaponShooter prefab in _weapons)
            {
                WeaponShooter initializedWeapon = Instantiate<WeaponShooter>(prefab, _weaponParentSlot?.transform); // Хак с преднастройкой расположения / поворота / размера объекта

                if (!_weaponsByType.TryAdd(prefab.GetWeaponType(), initializedWeapon)) // Оружие с таким типом уже добавлено
                    continue;

                SetWeaponStats(initializedWeapon, _weaponConfigurations);
                BulletSpawn bulletSpawn = initializedWeapon.GetComponentInChildren<BulletSpawn>();

                if (bulletSpawn)
                    _weaponBulletSpawners.TryAdd(initializedWeapon, bulletSpawn);

                DisactivateWeapon(initializedWeapon);
            }

            SwitchWeapon(_startingWeapon);
        }

        public static List<WeaponType> GetWeaponTypes()
        {
            return Self._weapons.Select(i => i.GetWeaponType()).ToList();
        }

        public static int GetWeaponUpgradeLevelsCount(WeaponType weaponType)
        {
            return Self.GetWeaponConfiguration(weaponType).WeaponStats.Length - 1;
        }
#nullable enable
        public static BulletSpawn? GetWeaponBulletSpawn()
        {
            return Self._playerBulletSpawnPoint;
        }
#nullable disable
        public static WeaponShooter GetCurrentWeapon()
        {
            return Self._currentWeapon;
        }

        public void SwitchWeapon(WeaponType weaponType)
        {
            WeaponShooter weapon = GetWeapon(weaponType);

            if (weapon is null) // Если оружие не найдено, оставляем текущее
                return;

            OnSwitchingWeapon?.Invoke(weapon);
        }

        public static void SetWeaponStatsByLevel(WeaponType weaponType, int level)
        {
            if (level < 0 || level > GetWeaponUpgradeLevelsCount(weaponType))
                return;

            Self._weaponsByType.TryGetValue(weaponType, out WeaponShooter weapon); // Логирование?
            WeaponUpgradeConfiguration weaponConfiguration = Self.GetWeaponConfiguration(weaponType);

            WeaponStats stats = weaponConfiguration.WeaponStats[level];

            weapon.UpdateWeaponStats(stats);
        }

        private void OnWeaponSwitched(WeaponShooter newWeapon)
        {
            if (_currentWeapon is not null)
            {
                DisactivateWeapon(_currentWeapon);
            }

            ActivateWeapon(newWeapon);

            _currentWeapon = newWeapon;
        }

        private void SetWeaponStats(WeaponShooter initializedWeapon, Dictionary<WeaponType, WeaponUpgradeConfiguration> weaponConfigurations)
        {
            WeaponType weaponType = initializedWeapon.GetWeaponType();

            WeaponUpgradeConfiguration weaponConfiguration = GetWeaponConfiguration(weaponType);

            WeaponStats stats = weaponConfiguration.WeaponStats[0]; // 0

            initializedWeapon.UpdateWeaponStats(stats);

            //SetWeaponStatsBasedOnLevel(initializedWeapon, weaponConfiguration, weaponLevel);
        }

        private void ActivateWeapon(WeaponShooter weapon)
        {
            InitializeWeaponTransformations(weapon);

            weapon.ShowWeapon(true);
        }

        private void InitializeWeaponTransformations(WeaponShooter weapon)
        {
            weapon.transform.parent = _weaponParentSlot?.transform;

            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = ZERO_ROTATION;
            weapon.transform.localScale = Vector3.one;
        }

        /*private void InitializeBulletSpawnPointTransformations(Transform bulletSpawnTransform)
        {
            _playerBulletSpawnPoint.transform.localPosition = bulletSpawnTransform.localPosition;
        }*/

        private void DisactivateWeapon(WeaponShooter weapon)
        {
            weapon.transform.parent = _inactiveWeaponsSlot;
            weapon.ShowWeapon(false);
        }

        private WeaponShooter GetWeapon(WeaponType weaponType) // Добавить метод GetWeaponSafe без выброса исключения (Метод GetWeapon используется для отладки)
        {
            if (_weaponsByType.TryGetValue(weaponType, out WeaponShooter weapon))
                return weapon;
            else
                throw new KeyNotFoundException("Не найдено оружие с типом " + weaponType.ToString());
        }

        private void LoadWeaponConfigurations(string pathToWeaponConfigs)
        {
            WeaponUpgradeConfiguration[] configs = Resources.LoadAll<WeaponUpgradeConfiguration>(pathToWeaponConfigs);

            foreach (WeaponUpgradeConfiguration config in configs)
            {
                if (!_weaponConfigurations.TryAdd(config.WeaponType, config))
                    DebugUtility.DebugMessage(this, "Обнаружена дублирующая конфигурация для оружия типа " + config.WeaponType.ToString() + "!");
            }
        }

        public WeaponUpgradeConfiguration GetWeaponConfiguration(WeaponType weaponType)
        {
            if (_weaponConfigurations.TryGetValue(weaponType, out WeaponUpgradeConfiguration weaponConfiguration) is false)
                throw new KeyNotFoundException("Не найдена конфигурация для оружия типа " + weaponType.ToString());

            return weaponConfiguration;
        }
    }
}