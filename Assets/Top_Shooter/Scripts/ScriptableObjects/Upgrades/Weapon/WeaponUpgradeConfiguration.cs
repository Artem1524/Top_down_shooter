using UnityEngine;
using TDShooter.Weapons;

namespace TDShooter
{
	[CreateAssetMenu(fileName = "NewWeaponUpgradeConfiguration", menuName = "Configs/Weapon Upgrade Configuration")]
	public class WeaponUpgradeConfiguration : ScriptableObject
	{
		[SerializeField]
		private WeaponType _weaponType;

		[SerializeField]
		private WeaponStats[] _stats;

		public WeaponType WeaponType => _weaponType;

		public WeaponStats[] WeaponStats => _stats;
	}
}