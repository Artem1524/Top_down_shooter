using UnityEngine;
using TDShooter.Weapons;
using TDShooter.Managers;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TDShooter.UI
{
    public class WeaponUpgrade : MonoBehaviour, IPointerClickHandler
    {
        private const string MAX_LEVEL_COST_TEXT = "MAX";

        [SerializeField]
        private WeaponType _weaponType;

        [SerializeField]
        private Text _levelText;
        [SerializeField]
        private Text _upgradeCostText;

        private WeaponUpgradeConfiguration _upgradeConfiguration;

        private void Start()
        {
            _upgradeConfiguration = WeaponManager.Self.GetWeaponConfiguration(_weaponType);
            UpdateWeaponUpgradeUI();
        }

        private void UpdateWeaponUpgradeUI()
        {
            _levelText.text = PlayerUpgradesManager.GetWeaponCurrentLevel(_weaponType).ToString();
            _upgradeCostText.text = GetUpgradeCostInfo();
        }

        private string GetUpgradeCostInfo()
        {
            int currentWeaponLevel = GetWeaponCurrentLevel();

            if (TryGetWeaponUpgradeCost(currentWeaponLevel, out int cost))
                return cost.ToString();

            return MAX_LEVEL_COST_TEXT;
        }

        private int GetWeaponCurrentLevel()
        {
            return PlayerUpgradesManager.GetWeaponCurrentLevel(_weaponType);
        }

        private bool TryGetWeaponUpgradeCost(int currentLevel, out int cost)
        {
            if (currentLevel >= PlayerUpgradesManager.GetWeaponUpgradeLevelsCount(_weaponType))
            {
                cost = 0;
                return false;
            }

            cost = _upgradeConfiguration.WeaponStats[currentLevel + 1].UpgradeCost;
            return true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int currentLevel = GetWeaponCurrentLevel();

            if (!TryGetWeaponUpgradeCost(currentLevel, out int cost))
                return;

            if (PlayerUpgradesManager.Self.Money < cost)
            {
                DebugUtility.DebugMessage(this, "Недостаточно денег для улучшения оружия " + _weaponType.ToString());
                return;
            }
            else
            {
                PlayerUpgradesManager.RemoveMoney(cost);
                PlayerUpgradesManager.AddWeaponUpgrade(_weaponType);
                UpdateWeaponUpgradeUI();
            }
        }
    }
}