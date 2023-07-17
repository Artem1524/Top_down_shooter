using UnityEngine;

namespace TDShooter.Controllers
{
    public class EnemyAttackController : MonoBehaviour
    {
        private Animator _animator;

        private EnemyAttackData _enemyAttackData;
        private AIState _aiState;

        private EnemyMovementController _enemyMovementController;
        //private EnemyAIData _enemyAIData;
        private bool _canMove;

        private void Awake()
        {
            TryGetComponent(out _animator);
            DebugUtility.HandleErrorIfNullGetComponent<Animator, EnemyAttackController>(_animator, this, gameObject);

            _canMove = TryGetComponent(out _enemyMovementController);
            DebugUtility.HandleErrorIfNullGetComponent<EnemyMovementController, EnemyAttackController>(_enemyMovementController, this, gameObject);

            _aiState = AIState.Idle;
        }

        private void Update()
        {
            UpdateAIStateTransitions();
        }

        private void UpdateAIStateTransitions()
        {
            switch (_aiState)
            {
                case AIState.Idle:
                case AIState.Follow:
                case AIState.Attack:
                    break;
            }
        }
    }
}