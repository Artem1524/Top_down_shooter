using System;
using TDShooter.Helpers;
using TDShooter.Projectiles;
using TDShooter.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace TDShooter
{
    public class BatBoss : MonoBehaviour
    {
        [SerializeField, Min(0.1f)]
        private float _bossAbilityReloadTime = 5f;
        [SerializeField, Min(1)]
        private int _bossAbilityBulletsCount = 8;

        [SerializeField, Space]
        private Bullet _bossAbilityPrefab;
        [SerializeField]
        private LayerMask _collisionLayersToDestroyProjectile;
        [SerializeField]
        private BulletSpawn _abilityBulletsSpawn;

        [SerializeField, Space]
        private float Damage = 40f;
        [SerializeField]
        private EnemyProjectileData ProjectileData = new EnemyProjectileData(5f, 10f, 14f);

        private Health _health;

        private float _bossAbilityTimer = 0f;

        private void Awake()
        {
            TryGetComponent(out _health);
            DebugUtility.HandleErrorIfNullGetComponent<Health, BatBoss>(_health, this, gameObject);

            _health.OnHealthChanged += OnHealthChanged;
        }

        private void Update()
        {
            UseAbilityBulletsCircle();
        }

        private void OnHealthChanged(float healthRestored)
        {
            Image healthBar = _health.GetHealthBar();
            healthBar.fillAmount = _health.CurrentHealth / _health.MaxHealth;
        }

        private void UseAbilityBulletsCircle()
        {
            if (!_health.IsAlive)
                return;

            _bossAbilityTimer -= Time.deltaTime;

            if (_bossAbilityTimer <= 0f)
            {
                _bossAbilityTimer = _bossAbilityReloadTime;
                LaunchBulletsCircle(_bossAbilityBulletsCount);
            }
        }

        private void LaunchBulletsCircle(int bossAbilityBulletsCount)
        {
            float angle = 360f / bossAbilityBulletsCount;

            for (int i = 0; i < bossAbilityBulletsCount; i++)
            {
                LaunchProjectile(i * angle);
            }
        }

        private void LaunchProjectile(float angle)
        {
            EnemyAttackData _enemyAttackData = GetEnemyAttackData();
            ShootBulletData shootBulletData = GetShootBulletData(_bossAbilityPrefab, _enemyAttackData, _collisionLayersToDestroyProjectile);
            BulletHelper.ShootBulletForAbility(BulletHelper.GetBulletPool(), transform, angle, _abilityBulletsSpawn, shootBulletData);
        }

        private ShootBulletData GetShootBulletData(Bullet bulletPrefab, EnemyAttackData enemyAttackData, LayerMask collisionLayersToDestroyProjectile)
        {
            return new ShootBulletData(bulletPrefab, enemyAttackData.ProjectileData.BulletSpreadAngle,
                                       enemyAttackData.Damage, enemyAttackData.ProjectileData.BulletSpeed,
                                       enemyAttackData.ProjectileData.BulletMaxRange, collisionLayersToDestroyProjectile);
        }

        private EnemyAttackData GetEnemyAttackData()
        {
            return new EnemyAttackData(1f, 1f, Damage, ProjectileData.BulletSpreadAngle,
                                       ProjectileData.BulletSpeed, ProjectileData.BulletMaxRange);
        }
    }
}