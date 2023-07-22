using System;
using System.Collections;
using UnityEngine;

using TDShooter.FX;

using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace TDShooter
{
    public class Health : MonoBehaviour
    {
        private const string ANIM_TRIGGER_ON_DIE = "OnDie";

        public event Action<float> OnHealthChanged;
        public event Action<float> OnDie;

        //private const float DROP_PICKUP_SPREAD_BY_THIS_OBJECT_SCALE = 0.3f; // 0.3f = 30% of scale as random drop position
        [SerializeField, Range(1f, 10000f)]
        private float _maxHealth = 10f;
        [SerializeField]
        private float _health = 10f;
        [OverrideLabel("% Health Regen / sec")]
        [SerializeField]
        private float _healthRegen = 0f;
        [SerializeField]
        private Image _healthBar;

        [SerializeField, Space]
        private Animator? _animator;
        [SerializeField]
        private bool _hasAnimator = false;

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
            {
                float healthRestored = HealthRegen * MaxHealth * Time.deltaTime / 100;
                OnRegenHealth(healthRestored);
            }
        }

        public void OnDamage(float damage, int sourceLayer)
        {
            if (_invincible)
                return;
            if (_health <= 0)
                return;
            if (!IsAlive)
                return;

            CurrentHealth -= damage;

            OnHealthChanged?.Invoke(-damage);

            if (CurrentHealth <= 0)
            {
                SetDeadProps(_hasSpecialDeath);
            }
        }

        public void OnRegenHealth(float healthRestored)
        {
            CurrentHealth += healthRestored;
            OnHealthChanged?.Invoke(healthRestored);
        }

        public void SetDeadProps(bool hasSpecialDeath)
        {
            IsAlive = false;
            _hasHealthRegeneration = false;

            OnDie?.Invoke(_overDamage); // ƒл€ персонажа передаЄм сюда специальный метод

            //  оллайдеры, атаки

            if (! hasSpecialDeath)
            {
                LaunchDeathPreProcess(_dropPickups, _delayBeforeDeath);
            }
        }

        public Image GetHealthBar()
        {
            return _healthBar;
        }

        private void LaunchDeathPreProcess(DropPickups dropPickups, float delayBeforeDeath)
        {
            if (_hasAnimator)                         // fix: Ќе получаетс€ проверить _animator на null (если _animator не назначен), так как в Unity отображаетс€ значение None
                _animator.SetTrigger(ANIM_TRIGGER_ON_DIE);

            TryGetComponent(out Rigidbody rigidBody); // ”бираем столкновени€, если персонаж уничтожен
            TryGetComponent(out Collider collider);   // ¬озможно, лучше просто мен€ть layer на LayerMaskManager#DeathLayer (коллизи€ только с Wall)

            if (rigidBody is not null)
                Destroy(rigidBody);

            if (collider is not null)
                Destroy(collider);

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