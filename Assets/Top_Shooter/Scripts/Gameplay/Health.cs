using System;
using System.Collections;
using UnityEngine;

using TDShooter.FX;

using Random = UnityEngine.Random;

namespace TDShooter
{
    public class Health : MonoBehaviour
    {
        public event Action<float> OnDamaged;
        public event Action OnDie;

        //private const float DROP_PICKUP_SPREAD_BY_THIS_OBJECT_SCALE = 0.3f; // 0.3f = 30% of scale as random drop position
        [SerializeField, Range(1f, 10000f)]
        private float _maxHealth = 10f;
        [SerializeField]
        private float _health = 10f;
        [OverrideLabel("% Health Regen / sec")]
        [SerializeField]
        private float _healthRegen = 0f;

        [SerializeField, Space]
        private Animator _animator;

        [SerializeField, Range(0.1f, 1000f), Space]
        private float _delayBeforeDeath = 1f;
        [SerializeField]
        private FXExplosion _explosionWhenDiePrefab;
        [SerializeField]
        private bool _hasSpecialDeath = false;

        private float _overDamage = 0f; // ћожет использоватьс€ дл€ специальных методов, при получении большого количества сверхурона (взрыв робота)

        private bool _hasHealthRegeneration = false;
        private bool _invincible = false;

        private DropPickups _dropPickups;
        private float _deathDelayTimer;

        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public bool IsAlive { get; private set; } = true;
        public float HealthRegen
        {   get => _healthRegen;
            set
            {
                _healthRegen = value;

                if (_healthRegen == 0)
                    _hasHealthRegeneration = false;
                else
                    _hasHealthRegeneration = true;
            }
        }
        public float CurrentHealth 
        {   get => _health;
            set
            {
                _health = value;

                if (_health < 0)
                {
                    _overDamage = -1 * _health;
                    _health = 0;
                }
                if (_health > _maxHealth)
                    _health = _maxHealth;
            } 
        }

        private void Awake()
        {
            if (_healthRegen > 0)
                _hasHealthRegeneration = true;

            if (TryGetComponent<DropPickups>(out DropPickups dropPickups))
                _dropPickups = dropPickups;
        }

        private void Update()
        {
            if (_hasHealthRegeneration && CurrentHealth < MaxHealth)
                CurrentHealth += HealthRegen * MaxHealth * Time.deltaTime / 100;
        }

        public void OnDamage(float damage, Transform source)
        {
            if (_invincible)
                return;
            if (_health <= 0)
                return;
            if (!IsAlive)
                return;

            CurrentHealth -= damage;

            OnDamaged?.Invoke(damage);

            if (CurrentHealth <= 0)
            {
                SetDeadProps(_hasSpecialDeath);
            }
        }

        public void SetDeadProps(bool hasSpecialDeath)
        {
            IsAlive = false;
            _hasHealthRegeneration = false;

            OnDie?.Invoke();

            //  оллайдеры, атаки

            if (! hasSpecialDeath)
            {
                //_animator.Play("Death");
                LaunchDeathPreProcess(_dropPickups, _delayBeforeDeath);
            }
        }

        private void LaunchDeathPreProcess(DropPickups dropPickups, float delayBeforeDeath)
        {
            Animation deathAnimation = null;

            if (deathAnimation is not null)
            {
                deathAnimation.Play(PlayMode.StopAll);
            }

            StartCoroutine(DeathCoroutine(dropPickups, delayBeforeDeath));
        }

        private IEnumerator DeathCoroutine(DropPickups dropPickups, float delayBeforeDeath)
        {
            _deathDelayTimer = delayBeforeDeath;

            while (_deathDelayTimer > 0f)
            {
                yield return null;

                _deathDelayTimer -= Time.deltaTime;
            }

            TryDropPickups(dropPickups);
            LaunchExplosionWhenDie();
            Destroy(gameObject);
        }

        private void TryDropPickups(DropPickups dropPickups)
        {
            if (dropPickups is null)
                return;

            foreach (DropPickupData data in dropPickups.GetDropPickups())
            {
                if (Random.Range(0f, 1f) <= data.DropRate)
                    DropPickup(data.DropPickup);
            }
        }

        private void DropPickup(Pickup dropPickup)
        {
            Vector3 currentPos = transform.position;
            Vector3 dropPosition = new Vector3(currentPos.x, currentPos.y + Random.Range(0f, 2.5f), currentPos.z); //

            PickupCreatorHelper.InstantiatePickup<Pickup>(dropPickup, dropPosition, dropPickup.transform.rotation);
        }

        private void LaunchExplosionWhenDie()
        {
            if (_explosionWhenDiePrefab is not null)
            {
                FXExplosion explosion = Instantiate<FXExplosion>(_explosionWhenDiePrefab, transform.position, transform.rotation);
                explosion.LaunchExplosion();
            }
        }
    }
}