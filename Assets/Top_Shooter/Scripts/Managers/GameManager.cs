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

        private void Awake()
        {
            if (Self is null)
            {
                Self = this;
                Self._picklockGameUI = FindObjectOfType<PicklockGameUI>(true);
                DebugUtility.HandleErrorIfNullFindObject<PicklockGameUI, GameManager>(Self._picklockGameUI, this);
                SetActivePicklockGameUI(false);

                Self._playerControllersManager = FindObjectOfType<PlayerControllersManager>();
                DebugUtility.HandleErrorIfNullFindObject<PlayerControllersManager, GameManager>(Self._playerControllersManager, this);
            }
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
            Self.PicklockGameStarted = false;
            SetActivePicklockGameUI(false);
            ActivatePlayerControls();
        }

        private static void ActivatePlayerControls()
        {
            Self._playerControllersManager.PlayerControls.Player.Enable();
        }

        private static void DeactivatePlayerControls()
        {
            Self._playerControllersManager.PlayerControls.Player.Disable();
        }

        public static void SetActivePicklockGameUI(bool isActive)
        {
            Self._picklockGameUI.gameObject.SetActive(isActive);
        }
    }
}