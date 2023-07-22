using UnityEngine;

namespace TDShooter.Pickups
{
    public class AutoRemovable : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 1000f)]
        private float _expirationTime = 12f;

        private void Update()
        {
            _expirationTime -= Time.deltaTime;
            if (_expirationTime <= 0f)
                Destroy(gameObject);
        }
    }
}