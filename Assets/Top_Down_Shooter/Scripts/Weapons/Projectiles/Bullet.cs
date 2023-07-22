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

        private int _sourceLayer;
        private LayerMask _collisionLayersToDestroyProjectile;

        public void Initialize(int sourceLayer, float damage, float speed, float range, LayerMask collisionLayersToDestroyProjectile)
        {
            _range = range;
            _speed = speed;
            _sourceLayer = sourceLayer;
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

            bool isSourceLayer = _sourceLayer == other.gameObject.layer;

            if (hasHealth && !isSourceLayer)
            {
                isTargetDead = !targetHealth.IsAlive;
                targetHealth.OnDamage(_damage, _sourceLayer);
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