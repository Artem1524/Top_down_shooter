using System;

namespace TDShooter
{
    [Serializable]
    public struct WeaponStats
    {
        public float FireRate; // 3f
        public float Range; // 10f
        public int BulletsNumberInShot; // 1
        public float Damage; // 10f

        public float BulletSpeed; // 10f
        public float BulletSpreadAngle; // 0f

        // "Delay before first shot after start using stick (in sec)"
        public float DelayBeforeFirstShot; // 0.3f

        public int UpgradeCost;
    }
}