using TDShooter.Projectiles;
using TDShooter.Weapons;
using UnityEngine;

namespace TDShooter.Helpers
{
    public class BulletHelper : MonoBehaviour
    {
        public static BulletHelper Self { get; private set; }

        [SerializeField]
        private Transform _bulletPool;

        private void OnEnable()
        {
            //if (Self is null)
            //{
                Self = this;
            //}
        }

        public static Transform GetBulletPool()
        {
            return Self._bulletPool;
        }
#nullable enable
        public static void ShootBullet(Transform bulletPoolParent, Transform source,
                                        BulletSpawn? bulletSpawn, ShootBulletData shootBulletData)
        {
            // Можно не вызывать метод Random, если spreadAngle == 0
            Quaternion spreadAngle = Quaternion.Euler(
                0,
                Random.Range(-shootBulletData.BulletSpreadAngle, shootBulletData.BulletSpreadAngle),
                0
            );

            Quaternion bulletRotation = (bulletSpawn == null) ? source.rotation : bulletSpawn.transform.rotation * spreadAngle;
            Vector3 bulletSpawnPosition = (bulletSpawn == null) ? source.position : bulletSpawn.transform.position;
            Bullet bullet = Instantiate<Bullet>(shootBulletData.BulletPrefab, bulletSpawnPosition, bulletRotation, bulletPoolParent);
            bullet.Initialize(source.gameObject.layer, shootBulletData.Damage, shootBulletData.BulletSpeed, shootBulletData.Range, shootBulletData.CollisionLayersToDestroyProjectile);
        }

        public static void ShootBulletForAbility(Transform bulletPoolParent, Transform source, float bulletAngle,
                                                 BulletSpawn abilityBulletsSpawn, ShootBulletData shootBulletData)
        {
            {
                // Можно не вызывать метод Random, если spreadAngle == 0
                Quaternion spreadAngle = Quaternion.Euler(
                    0,
                    Random.Range(-shootBulletData.BulletSpreadAngle, shootBulletData.BulletSpreadAngle),
                    0
                );

                Quaternion angle = Quaternion.Euler(0, bulletAngle, 0) * spreadAngle;
                Vector3 bulletSpawnPosition = abilityBulletsSpawn.transform.position;

                Bullet bullet = Instantiate<Bullet>(shootBulletData.BulletPrefab, bulletSpawnPosition, angle, bulletPoolParent);
                bullet.Initialize(source.gameObject.layer, shootBulletData.Damage, shootBulletData.BulletSpeed, shootBulletData.Range, shootBulletData.CollisionLayersToDestroyProjectile);
            }
        }
#nullable disable
    }
}