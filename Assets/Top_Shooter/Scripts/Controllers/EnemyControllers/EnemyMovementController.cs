using UnityEngine;

namespace TDShooter.Controllers
{
    public class EnemyMovementController : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        private AIState _aiState;

        private EnemyMovementData _enemyMovementData;

        private void Start()
        {
            _aiState = AIState.Idle;
        }
    }
}