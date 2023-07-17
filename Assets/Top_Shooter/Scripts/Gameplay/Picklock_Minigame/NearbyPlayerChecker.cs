using UnityEngine;

using TDShooter.Controllers;
using TDShooter.Managers;

namespace TDShooter
{
    public class NearbyPlayerChecker : MonoBehaviour
    {
        private Player _player;
        private MoveController _playerMoveController;
        private Chest _chest;

        private void Awake()
        {
            _player = FindObjectOfType<Player>();
            DebugUtility.HandleErrorIfNullFindObject<Player, NearbyPlayerChecker>(_player, this);

            _player.TryGetComponent(out _playerMoveController);
            DebugUtility.HandleErrorIfNullGetComponent<MoveController, NearbyPlayerChecker>(_playerMoveController, this, _player.gameObject);

            TryGetComponent(out _chest);
            DebugUtility.HandleErrorIfNullGetComponent<Chest, NearbyPlayerChecker>(_chest, this, gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled)
                return;

            int layer = other.gameObject.layer;

            if (LayerMaskManager.IsLayerExistsInLayerMask(layer, LayerMaskManager.PlayerLayer))
            {
                _playerMoveController.SetNearbyChestData(_chest);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled)
                return;

            int layer = other.gameObject.layer;

            if (LayerMaskManager.IsLayerExistsInLayerMask(layer, LayerMaskManager.PlayerLayer))
            {
                GameManager.StopPicklockGame();
                _playerMoveController.ClearNearbyChestData();
            }
        }
    }
}