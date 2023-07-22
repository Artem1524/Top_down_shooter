using UnityEngine;
using TDShooter.UI.PicklockGame;
using TDShooter.Controllers;
using System;

namespace TDShooter.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Self { get; private set; }

        public bool PicklockGameStarted { get; private set; } = false;

        private PicklockGameUI _picklockGameUI;
        private PlayerControllersManager _playerControllersManager;
        private Player _player;
        private MoveController _moveController;
        private ShootController _shootController;

        private void OnEnable()
        {
            if (Self is null)
            {
                Self = this;
            }

            Self._picklockGameUI = FindObjectOfType<PicklockGameUI>(true);
            DebugUtility.HandleErrorIfNullFindObject<PicklockGameUI, GameManager>(Self._picklockGameUI, this);
            SetActivePicklockGameUI(false);

            Self._playerControllersManager = FindObjectOfType<PlayerControllersManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerControllersManager, GameManager>(Self._playerControllersManager, this);

            Self._player = FindObjectOfType<Player>(true);
            DebugUtility.HandleErrorIfNullFindObject<Player, GameManager>(Self._player, this);

            _player.TryGetComponent(out Self._moveController);
            DebugUtility.HandleErrorIfNullGetComponent<MoveController, GameManager>(Self._moveController, Self, Self.gameObject);

            _player.TryGetComponent(out Self._shootController);
            DebugUtility.HandleErrorIfNullGetComponent<ShootController, GameManager>(Self._shootController, Self, Self.gameObject);
        }

        public static void OnPicklockGameButtonPressed()
        {
            if (Self.PicklockGameStarted)
                StopPicklockGame();
            else
                StartPicklockGame();
        }

        public static void StartPicklockGame()
        {
            Self.PicklockGameStarted = true;
            SetActivePicklockGameUI(true);
            DeactivatePlayerControls();
        }


        public static void StopPicklockGame()
        {
            ClosePicklockGameUI();
            ActivatePlayerControls();
        }

        public static void ClosePicklockGameUI()
        {
            Self.PicklockGameStarted = false;
            SetActivePicklockGameUI(false);
        }

        public static void ActivatePlayerControls()
        {
            Self._playerControllersManager.PlayerControls.Player.Enable();
        }

        public static void DeactivatePlayerControls()
        {
            Self._playerControllersManager.PlayerControls.Player.Disable();
        }

        public static void SetActivePicklockGameUI(bool isActive)
        {
            Self._picklockGameUI.gameObject.SetActive(isActive);
        }
    }
}