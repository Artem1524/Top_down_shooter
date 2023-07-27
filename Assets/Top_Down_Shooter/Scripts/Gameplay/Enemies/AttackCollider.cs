using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TDShooter.Controllers;

namespace TDShooter
{
    public class AttackCollider : MonoBehaviour
    {
        private EnemyAttackController _enemyAttackController;
        private List<string> _contacts = new List<string>();

        private Collider _collider;

        public bool ColliderActivity
        {
            get => _collider.enabled;
            set
            {
                _collider.enabled = value;
                if (!value) _contacts.Clear();
            }
        }

        private void Awake()
        {
            TryGetComponent(out _collider);
            DebugUtility.HandleErrorIfNullGetComponent<Collider, AttackCollider>(_collider, this, gameObject);

            ColliderActivity = false;
        }

        public void Initialize(EnemyAttackController attackController)
        {
            _enemyAttackController = attackController;
        }

        private void OnTriggerEnter(Collider other)
        {
            bool hasHealth = other.TryGetComponent(out Health health);
            int sourceLayer = _enemyAttackController.gameObject.layer;

            bool isSourceLayer = sourceLayer == other.gameObject.layer;

            if (!hasHealth || isSourceLayer || _contacts.Contains(health.name))
                return;

            _contacts.Add(health.name);

            float damage = _enemyAttackController.GetDamage();
            health.OnDamage(damage, sourceLayer);
        }
    }
}