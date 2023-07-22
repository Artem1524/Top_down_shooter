using TDShooter.Projectiles;
using UnityEngine;

namespace TDShooter
{
    public struct ShootBulletData
    {
        public Bullet BulletPrefab;

        public float BulletSpreadAngle;
        public float Damage;
        public float BulletSpeed;
        public float Range;

        public LayerMask CollisionLayersToDestroyProjectile;

        public ShootBulletData(Bullet bulletPrefab, float bulletSpreadAngle, float damage,
                               float bulletSpeed, float range, LayerMask collisionLayersToDestroyProjectile)
        {
            BulletPrefab = bulletPrefab;

            BulletSpreadAngle = bulletSpreadAngle;
            Damage = damage;
            BulletSpeed = bulletSpeed;
            Range = range;

            CollisionLayersToDestroyProjectile = collisionLayersToDestroyProjectile;
        }
    }
}