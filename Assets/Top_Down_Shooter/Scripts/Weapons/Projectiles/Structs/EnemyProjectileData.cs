using System;

namespace TDShooter.Projectiles
{
    [Serializable]
    public struct EnemyProjectileData
    {
        public float BulletSpreadAngle;
        public float BulletSpeed;
        public float BulletMaxRange;

        public EnemyProjectileData(float bulletSpreadAngle, float bulletSpeed, float bulletMaxRange)
        {
            BulletSpreadAngle = bulletSpreadAngle;
            BulletSpeed = bulletSpeed;
            BulletMaxRange = bulletMaxRange;
        }
    }
}