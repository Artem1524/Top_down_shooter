using UnityEngine;
using System;

using TDShooter.Managers;
using TDShooter.UI.PicklockGame;

using static UnityEngine.InputSystem.InputAction;

namespace TDShooter.Controllers
{
    public class MoveController : PlayerComponentController
    {
        private const string ANIM_PARAM_IS_MOVING = "IsMoving";
        private const string ANIM_PARAM_SIDE_SPEED = "SideMove";
        private const string ANIM_PARAM_FRONT_SPEED = "FrontMove";
        private const string ANIM_PARAM_MOVE_SPEED_ANIM_SPEED = "MoveSpeedAnimSpeed";

        [SerializeField, Range(0.05f, 10f)]
        [Tooltip("Ìíîæèòåëü ñêîðîñòè âîñïðîèçâåäåíèÿ àíèìàöèè ïåðåìåùåíèÿ (îòíîñèòåëüíî ñêîðîñòè ïåðåìåùåíèÿ)")]
        private float _animSpeedMoveSpeedMultiply = 0.7f;
        [SerializeField, Range(0.1f, 20f)]
        [Tooltip("Базовая скорость игрока")]
        private float _speed = 5f;

        private PicklockGameUI _picklockGameUI;

        private Vector3 _motionVector = new Vector3();
        private bool _moveActionsNotAdded = true;
        private Chest _nearbyChest = null;

        // Хак исправления "бага" (Метод MoveController.OnEnable вызывается раньше метода PlayerControllersManager.Awake)
        private void Start()
        {
            if (_moveActionsNotAdded)
                AddMoveActionsAndUpdateAnimSpeed();

            _picklockGameUI = FindObjectOfType<PicklockGameUI>(true);
            DebugUtility.HandleErrorIfNullFindObject<PicklockGameUI, MoveController>(_picklockGameUI, this);

            _picklockGameUI.OnPicklockGameFinished += OnPicklockGameFinished;
        }

        private void OnEnable()
        {
            if (GetControls() is not null)
                AddMoveActionsAndUpdateAnimSpeed();
        }
    
        private void OnDisable()
        {
            GetControls().Player.Moving.performed -= OnPlayerMove;
            GetControls().Player.Moving.started -= OnPlayerStartMoving;
            GetControls().Player.Moving.canceled -= OnPlayerStopMoving;

            GetControls().PicklockGame.PicklockGameStartStop.performed -= OnPicklockGameButtonPressed;
            GetControls().PicklockGame.PicklockGamePush.performed -= OnPicklockPushButtonPressed;

            _moveActionsNotAdded = true;
        }
    
        private void Update()
        {
            Move();
        }
    
        private void OnPlayerMove(CallbackContext ctx)
        {
            Vector2 value = ctx.ReadValue<Vector2>();
            float x = value.x;
            float y = value.y;

            SetMotion(x, y);
            SetAnimMoveParams(_motionVector);
        }

        private void OnPlayerStartMoving(CallbackContext ctx)
        {
            // Óáðàòü idle, åñëè ñòðåëÿåò
            _controlsMgr.IsMoving = true;

            GetAnimator().SetBool(ANIM_PARAM_IS_MOVING, _controlsMgr.IsMoving);
        }

        private void OnPlayerStopMoving(CallbackContext ctx)
        {
            Vector2 value = ctx.ReadValue<Vector2>();
            float x = value.x;
            float y = value.y;

            SetMotion(x, y);
            SetAnimMoveParams(_motionVector);

            if (x == 0 && y == 0)
            {
                _controlsMgr.IsMoving = false;
                GetAnimator().SetBool(ANIM_PARAM_IS_MOVING, _controlsMgr.IsMoving);
            }
        }

        private void SetMotion(float x, float y)
        {
            _motionVector.x = x;
            _motionVector.z = y;
        }

        private void SetAnimMoveParams(Vector3 motionVector)
        {
            // Значения x;y относительно текущего поворота
            GetAnimator().SetFloat(ANIM_PARAM_SIDE_SPEED, motionVector.x);
            GetAnimator().SetFloat(ANIM_PARAM_FRONT_SPEED, motionVector.z);
        }

        public void ChangeMoveSpeed(float newMoveSpeed)
        {
            _speed = newMoveSpeed;
            GetAnimator().SetFloat(ANIM_PARAM_MOVE_SPEED_ANIM_SPEED, GetMovingAnimSpeed());
        }

        private void Move()
        {
            transform.position += _motionVector * _speed * Time.deltaTime;
        }

        private float GetMovingAnimSpeed()
        {
            return _speed * _animSpeedMoveSpeedMultiply;
        }

        private void AddMoveActionsAndUpdateAnimSpeed()
        {
            GetControls().Player.Moving.performed += OnPlayerMove;
            GetControls().Player.Moving.started += OnPlayerStartMoving;
            GetControls().Player.Moving.canceled += OnPlayerStopMoving;

            GetControls().PicklockGame.PicklockGameStartStop.performed += OnPicklockGameButtonPressed; //
            GetControls().PicklockGame.PicklockGamePush.performed += OnPicklockPushButtonPressed;

            _moveActionsNotAdded = false;

            ChangeMoveSpeed(_speed);
        }

        private void OnPicklockGameButtonPressed(CallbackContext ctx)
        {
            if (_nearbyChest is not null && _picklockGameUI.IsNeedPicklock)
            {
                DebugUtility.DebugMessage(this, "Миниигра");
                GameManager.OnPicklockGameButtonPressed();
            }
        }

        private void OnPicklockPushButtonPressed(CallbackContext ctx)
        {
            if (GameManager.Self.PicklockGameStarted && _nearbyChest is not null)
                UpdatePicklockGameProcess();
        }

        private void UpdatePicklockGameProcess()
        {
            // DebugUtility.DebugMessage(this, "Взаимодействие в миниигре");
            _picklockGameUI.PushButtonPressed();
        }

        private void OnPicklockGameFinished()
        {
            GameManager.StopPicklockGame();
            _picklockGameUI.SetIsNeedPicklock(true);

            _nearbyChest.PlayOpenChestAnimation();
            _nearbyChest.DisableNearbyPlayerChecker();
            _nearbyChest.DropMoney();

            ClearNearbyChestData();
        }

        public void SetNearbyChestData(Chest chest)
        {
            _nearbyChest = chest;
        }

        public void ClearNearbyChestData()
        {
            _nearbyChest = null;
        }

        public void ResetPlayer()
        {
            _picklockGameUI.SetIsNeedPicklock(true);
            ClearNearbyChestData();
        }
    }
}