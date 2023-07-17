using System;

namespace TDShooter
{
    [Serializable]
    public struct EnemyAttackData
    {
        public float AttackSpeed;
        public float ProjectileSpeed;
        public float Range;
        public float Damage;
    }
}