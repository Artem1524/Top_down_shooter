using UnityEngine;
using UnityEngine.UI;

using TDShooter.Managers;

namespace TDShooter
{
    public class Player : MonoBehaviour
    {
        private const string ANIM_TRIGGER_ON_DIE = "OnDie";

        private Health _health;
        private Animator _animator;

        private void Awake()
        {
            TryGetComponent(out _health);
            DebugUtility.HandleErrorIfNullGetComponent<Health, Player>(_health, this, gameObject);

            _health.OnHealthChanged += OnHealthChanged;
            _health.OnDie += OnDie;

            TryGetComponent(out _animator);
            DebugUtility.HandleErrorIfNullGetComponent<Animator, Player>(_animator, this, gameObject);
        }

        private void OnHealthChanged(float healthRestored)
        {
            Image healthBar = _health.GetHealthBar();
            healthBar.fillAmount = _health.CurrentHealth / _health.MaxHealth;
        }

        private void OnDie(float overDamage)
        {
            _animator.SetTrigger(ANIM_TRIGGER_ON_DIE);
            GameManager.DeactivatePlayerControls();
            GameManager.ClosePicklockGameUI();
        }
    }
}