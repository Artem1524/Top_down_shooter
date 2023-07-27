using UnityEngine;

using TDShooter.Helpers;
using TDShooter.Projectiles;
using TDShooter.Weapons;

namespace TDShooter.Controllers
{
    public class EnemyAttackController : MonoBehaviour
    {
        private const string ANIMATION_EVENT_STRING_PARAM_COLLIDER = "Collider";
        private const string ANIMATION_EVENT_END_ATTACK = "Attack";

        private const string ANIM_PARAM_IS_IDLE = "IsIdle";
        private const string ANIM_PARAM_ATTACK_SPEED_TIME = "AttackSpeedTime";
        private const string ANIM_TRIGGER_ON_ATTACK = "OnAttack";

        private Animator _animator;

        private AttackCollider _attackCollider;

        private EnemyAttackData _enemyAttackData;
        private AIState _aiState;

        private EnemyMovementController _enemyMovementController;

        private bool _canMove;

        [SerializeField]
        private float DetectionRange = 10; // Дальность, при которой персонаж начинает двигаться
        [SerializeField]
        private float AttackRange = 5;
        [SerializeField]
        private float MinimumAttackRange = 3;
        [SerializeField]
        private float LossTargetRange = 12;

        [SerializeField, Space]
        private float AttackSpeedTime = 1.5f;
        [SerializeField]
        private float Damage = 10f;
        [SerializeField]
        private EnemyProjectileData ProjectileData = new EnemyProjectileData(5f, 10f, 14f);
        [SerializeField]
        private bool IsRangedAttack = false;

        [SerializeField, Space]
        private Bullet _bulletPrefab;
        [SerializeField]
        private BulletSpawn _bulletSpawnPoint;
        [SerializeField]
        private LayerMask _collisionLayersToDestroyProjectile;

        private bool _isTargetDetected;

        private bool _isIdle = true;
        private bool _isMoving;
        private bool _isAttack;
        private Transform _attackTarget;

        private Health _health;

        private float _attackTimer = 0f;

        private void Awake()
        {
            TryGetComponent(out _animator);
            DebugUtility.HandleErrorIfNullGetComponent<Animator, EnemyAttackController>(_animator, this, gameObject);

            _canMove = TryGetComponent(out _enemyMovementController);
            DebugUtility.HandleErrorIfNullGetComponent<EnemyMovementController, EnemyAttackController>(_enemyMovementController, this, gameObject);

            _attackTarget = FindObjectOfType<Player>()?.transform;
            DebugUtility.HandleErrorIfNullFindObject<Player, EnemyAttackController>(_attackTarget, this);

            TryGetComponent(out _health);
            DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyAttackController>(_health, this, gameObject);

            if (!IsRangedAttack)
                InitializeMeleeAttack();

            _aiState = AIState.Idle;
            _isMoving = false;
            _isIdle = true;
            SetAnimatorIsIdleParam(_isIdle);
        }

        private void Update()
        {
            UpdateAIStateTransitions();
        }

        private void UpdateAIStateTransitions()
        {
            if (!_health.IsAlive)
                return;

            switch (_aiState)
            {
                case AIState.Idle:
                    if (IsTargetDetected(_attackTarget))//!_isIdle)     // Цель обнаружена
                    {
                        _aiState = AIState.Detected;
                        //OnTargetDetected(_attackTarget);
                        FollowTarget(_attackTarget);
                    }
                    break;

                case AIState.Detected:
                    if (IsTargetLoss(_attackTarget))                    // Цель скрылась из виду
                    {
                        _aiState = AIState.Idle;
                        OnTargetLoss(_attackTarget);
                    }
                    else if (IsTargetInRangeOfAttack(_attackTarget))    // Цель на дистанции атаки
                    {
                        _aiState = AIState.Attack;
                        OnBeginAttack(_attackTarget);
                        Attack(_attackTarget);
                    }
                    else                                                // Цель НЕ на дистанции атаки
                    {
                        FollowTarget(_attackTarget);
                    }
                    break;

                case AIState.Attack:
                    if (IsTargetInRangeOfAttack(_attackTarget))         // Цель на дистанции атаки
                    {
                        Attack(_attackTarget);
                    }
                    else                                                // Цель ушла с дистанции атаки (перемещение делается в другом блоке)
                    {
                        _aiState = AIState.Detected;
                        StopAttack(_attackTarget);
                    }

                    if (IsTargetReachMinimumAttackRange(_attackTarget)) // Цель слишком близко
                    {
                        OnTargetReachMinimumAttackRange(_attackTarget);
                    }
                    else                                                // Цель достаточно далеко (на дистанции атаки или дальше), не дублируем перемещение
                    {
                        SetMoveAnimationIfNeed();
                        FollowTarget(_attackTarget);
                    }
                    break;
            }
        }

        private void SetMoveAnimationIfNeed()
        {
            if (!_isMoving)
            {
                _isMoving = true;
                _isIdle = false;
                SetAnimatorIsIdleParam(_isIdle);
            }
        }

        private void SetIdleAnimationIfNeed()
        {
            if (_isMoving)
            {
                _isMoving = false;
                _isIdle = true;
                SetAnimatorIsIdleParam(_isIdle);
            }
        }

        private void OnTargetReachMinimumAttackRange(Transform attackTarget)
        {
            TurnToTarget(attackTarget);
            SetIdleAnimationIfNeed();
        }

        private bool IsTargetDetected(Transform attackTarget)
        {
            if (Vector3.Distance(transform.position, attackTarget.position) <= DetectionRange)
                return true;

            return false;
        }

        private void FollowTarget(Transform attackTarget)
        {
            TurnToTarget(attackTarget);
            SetMoveAnimationIfNeed();
            MoveToTarget(attackTarget);
        }

        private bool IsTargetLoss(Transform attackTarget)
        {
            if (Vector3.Distance(transform.position, attackTarget.position) > LossTargetRange)
                return true;

            return false;
        }

        private void OnTargetLoss(Transform attackTarget)
        {
            SetIdleAnimationIfNeed();
        }

        private bool IsTargetInRangeOfAttack(Transform attackTarget)
        {
            if (Vector3.Distance(transform.position, attackTarget.position) <= AttackRange)
                return true;

            return false;
        }

        private void Attack(Transform attackTarget)
        {
            _attackTimer -= Time.deltaTime;

            if (_attackTimer <= 0f)
            {
                LaunchAttack(attackTarget);
                _attackTimer = AttackSpeedTime;
            }
        }

        private void LaunchAttack(Transform attackTarget)
        {
            _animator.SetTrigger(ANIM_TRIGGER_ON_ATTACK);
            _animator.SetFloat(ANIM_PARAM_ATTACK_SPEED_TIME, 1 / AttackSpeedTime);

            if (IsRangedAttack)
            {
                LaunchProjectile(attackTarget);
            }
        }

        private void LaunchProjectile(Transform attackTarget)
        {
            _enemyAttackData = GetEnemyAttackData();
            ShootBulletData shootBulletData = GetShootBulletData(_bulletPrefab, _enemyAttackData, _collisionLayersToDestroyProjectile);
            BulletHelper.ShootBullet(BulletHelper.GetBulletPool(), transform, _bulletSpawnPoint, shootBulletData);
        }

        private EnemyAttackData GetEnemyAttackData()
        {
            return new EnemyAttackData(AttackSpeedTime, AttackRange, Damage, ProjectileData.BulletSpreadAngle,
                                       ProjectileData.BulletSpeed, ProjectileData.BulletMaxRange);
        }

        private void OnBeginAttack(Transform attackTarget)
        {
            _isAttack = true;
            //_animator.
        }

        private void StopAttack(Transform attackTarget)
        {
            _isAttack = false;
            //
        }

        private bool IsTargetReachMinimumAttackRange(Transform attackTarget)
        {
            if (Vector3.Distance(transform.position, attackTarget.position) <= MinimumAttackRange)
                return true;

            return false;
        }

        private void MoveToTarget(Transform attackTarget)
        {
            if (_canMove)
                _enemyMovementController.Move();
        }

        private void TurnToTarget(Transform attackTarget)
        {
            transform.LookAt(attackTarget, transform.up); // perf: Поворачивать персонажа только вокруг оси Y
        }

        private void SetAnimatorIsIdleParam(bool isIdle)
        {
            _animator.SetBool(ANIM_PARAM_IS_IDLE, isIdle);
            /*if (_isIdle && _aiState != AIState.Idle)
            {
                SetIdleAIState();
            }*/
        }

        private ShootBulletData GetShootBulletData(Bullet bulletPrefab, EnemyAttackData enemyAttackData, LayerMask collisionLayersToDestroyProjectile)
        {
            return new ShootBulletData(bulletPrefab, enemyAttackData.ProjectileData.BulletSpreadAngle,
                                       enemyAttackData.Damage, enemyAttackData.ProjectileData.BulletSpeed,
                                       enemyAttackData.ProjectileData.BulletMaxRange, collisionLayersToDestroyProjectile);
        }

        private void OnCollider_AnimationEvent(string param)
        {
            if (!ANIMATION_EVENT_STRING_PARAM_COLLIDER.Equals(param))
                return;

            bool isColliderEnabled = _attackCollider.ColliderActivity;

            _attackCollider.ColliderActivity = !_attackCollider.ColliderActivity;
        }

        private void OnActionEnd_AnimationEvent(string param)
        {
            if (ANIMATION_EVENT_END_ATTACK.Equals(param))
            {
                _attackCollider.ColliderActivity = false;
            }

        }

        private void InitializeMeleeAttack()
        {
            _attackCollider = GetComponentInChildren<AttackCollider>(true);

            if (_attackCollider is not null)
                _attackCollider.Initialize(this);
        }

        public float GetDamage()
        {
            return Damage;
        }
    }
}