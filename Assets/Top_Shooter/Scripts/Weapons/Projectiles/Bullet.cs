using System;
using UnityEngine;

namespace TDShooter.Projectiles
{
    public class Bullet : MonoBehaviour
    {
        public event Action OnHit; // Инициализировать событие

        private float _range = 10f;
        private float _speed = 10f;
        private float _damage = 10f;

        private Transform _source;
        private LayerMask _collisionLayersToDestroyProjectile;

        public void Initialize(Transform source, float damage, float speed, float range, LayerMask collisionLayersToDestroyProjectile)
        {
            _range = range;
            _speed = speed;
            _source = source;
            _damage = damage;
            _collisionLayersToDestroyProjectile = collisionLayersToDestroyProjectile;
        }

        private void Update()
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
            _range -= _speed * Time.deltaTime;

            if (_range <= 0f)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            bool hasHealth = other.TryGetComponent(out Health targetHealth);
            bool isTargetDead = false;

            if (hasHealth && other.gameObject != _source.gameObject)
            {
                targetHealth.OnDamage(_damage, _source);
                isTargetDead = !targetHealth.IsAlive;
            }

            if (NeedDestroyProjectile(other.gameObject, _collisionLayersToDestroyProjectile, isTargetDead))
                Destroy(gameObject);
        }

        private bool NeedDestroyProjectile(GameObject collisionObject, LayerMask collisionLayersToDestroyProjectile, bool isTargetDead)
        {
            if (isTargetDead) // Например, если робот уничтожен, но ещё не взорвался
                return false;

            if ((collisionLayersToDestroyProjectile.value & (1 << collisionObject.layer)) != 0)
                return true;
            else
                return false;
        }
    }
}