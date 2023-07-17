using UnityEngine;

using TDShooter.Projectiles;

namespace TDShooter.Weapons
{
    public class WeaponShooter : MonoBehaviour
    {
        [SerializeField]
        private WeaponType _weaponType;

        [SerializeField, Space]
        private Bullet _bulletPrefab;
        [SerializeField]
        private Transform _bulletSpawnTransform; // Не используется

        [SerializeField]
        private LayerMask _collisionLayersToDestroyProjectile;

        public WeaponStats WeaponStatistics { get; private set; }

        public WeaponType GetWeaponType() => _weaponType;

#nullable enable
        public void Shoot(Transform bulletPoolParent, Transform source, BulletSpawn? bulletSpawn)
        {
            for (int i = 0; i < WeaponStatistics.BulletsNumberInShot; i++)
                ShootBullet(bulletPoolParent, source, bulletSpawn);
        }
#nullable disable

        public void ShowWeapon(bool isActive)
        {
            transform.gameObject.SetActive(isActive);
        }

        public void UpdateWeaponStats(WeaponStats weaponStats)
        {
            WeaponStats temp = WeaponStatistics;

            temp.FireRate = weaponStats.FireRate;
            temp.Range = weaponStats.Range;
            temp.Damage = weaponStats.Damage;
            temp.BulletsNumberInShot = weaponStats.BulletsNumberInShot;
            temp.BulletSpeed = weaponStats.BulletSpeed;
            temp.BulletSpreadAngle = weaponStats.BulletSpreadAngle;
            temp.DelayBeforeFirstShot = weaponStats.DelayBeforeFirstShot;

            WeaponStatistics = temp;
        }

#nullable enable
        private void ShootBullet(Transform bulletPoolParent, Transform source, BulletSpawn? bulletSpawn)
        {
            // Можно не вызывать метод Random, если spreadAngle == 0
            Quaternion spreadAngle = Quaternion.Euler(
                0,
                Random.Range(-WeaponStatistics.BulletSpreadAngle, WeaponStatistics.BulletSpreadAngle),
                0
            );

            Quaternion bulletRotation = (bulletSpawn == null) ? Quaternion.Euler(0, 0, 0) : bulletSpawn.transform.rotation * spreadAngle;
            Vector3 bulletSpawnPosition = (bulletSpawn == null) ? Vector3.zero : bulletSpawn.transform.position;
            Bullet bullet = Instantiate<Bullet>(_bulletPrefab, bulletSpawnPosition, bulletRotation, bulletPoolParent);
            bullet.Initialize(source, WeaponStatistics.Damage, WeaponStatistics.BulletSpeed, WeaponStatistics.Range, _collisionLayersToDestroyProjectile);
        }
#nullable disable

        /*public Transform GetBulletSpawnTransform()
        {
            return _bulletSpawnTransform;
        }*/
    }
}