using UnityEngine;

namespace TDShooter.Controllers
{
    public class EnemyMovementController : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        private EnemyMovementData _enemyMovementData;

        [SerializeField]
        private float MoveSpeed = 3;

        public void Move()
        {
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;
        }
    }
}