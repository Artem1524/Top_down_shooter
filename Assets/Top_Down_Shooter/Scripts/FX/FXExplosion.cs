using UnityEngine;

namespace TDShooter.FX
{
    public class FXExplosion : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            TryGetComponent<ParticleSystem>(out _particleSystem);
        }

        public void LaunchExplosion()
        {
            float destroyTime = (_particleSystem != null) ? _particleSystem.main.duration : 0f;

            if (_particleSystem is not null)
                _particleSystem.Play();

            Destroy(gameObject, destroyTime);
        }
    }
}