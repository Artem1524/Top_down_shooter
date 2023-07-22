using System;
using TDShooter.Projectiles;

namespace TDShooter
{
    [Serializable]
    public struct EnemyAttackData
    {
        public float AttackSpeedTime;
        public float AttackRange;
        public float Damage;

        public EnemyProjectileData ProjectileData;

        public EnemyAttackData(float attackSpeedTime, float attackRange, float damage,
                               float bulletSpreadAngle, float bulletSpeed, float bulletMaxRange)
        {
            AttackSpeedTime = attackSpeedTime;
            AttackRange = attackRange;
            Damage = damage;

            ProjectileData = new EnemyProjectileData(bulletSpreadAngle, bulletSpeed, bulletMaxRange);
        }
    }
}