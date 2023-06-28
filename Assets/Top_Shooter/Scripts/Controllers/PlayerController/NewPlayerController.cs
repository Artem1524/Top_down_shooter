using System;
using UnityEngine;

using static UnityEngine.InputSystem.InputAction;

namespace TDShooter.Controllers
{
    public class NewPlayerController : BaseController
    {
        private static string ANIM_PARAM_IS_MOVING = "IsMoving";
        private static string ANIM_PARAM_IS_SHOOTING = "IsShooting";
        private static string ANIM_PARAM_IS_IDLE = "IsIdle";
        private static string ANIM_PARAM_SIDE_SPEED = "SideMove";
        private static string ANIM_PARAM_FRONT_SPEED = "FrontMove";
        private static string ANIM_PARAM_MOVE_SPEED_ANIM_SPEED = "MoveSpeedAnimSpeed";

        [SerializeField, Range(0.05f, 10f)]
        [Tooltip("Множитель скорости воспроизведения анимации перемещения (относительно скорости перемещения)")]
        private float ANIM_SPEED_MULTIPLY_MOVE_SPEED = 0.7f;
        [SerializeField]
        private Animator _animator;

        private PlayerControls _controls;
        private Vector3 _motionVector = new Vector3();

        public bool IsMoving { get; private set; } = false;
        public bool IsShooting { get; private set; } = false;

        private void Awake()
        {
            _controls = new PlayerControls();
        }

        private void OnEnable()
        {
            _controls.Player.Enable();

            _controls.Player.Moving.performed += OnPlayerMove;
            _controls.Player.Moving.started += OnPlayerStartMoving;
            _controls.Player.Moving.canceled += OnPlayerStopMoving;

            ChangeMoveSpeed(_speed);
        }

        private void OnDisable()
        {
            _controls.Player.Disable();

            _controls.Player.Moving.performed -= OnPlayerMove;
            _controls.Player.Moving.started -= OnPlayerStartMoving;
            _controls.Player.Moving.canceled -= OnPlayerStopMoving;
        }

        private void Update()
        {
            Move();
        }

        #region Moving

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
            // Убрать idle, если стреляет
            IsMoving = true;

            _animator.SetBool(ANIM_PARAM_IS_MOVING, IsMoving);
            _animator.SetBool(ANIM_PARAM_IS_IDLE, GetIsIdle());
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
                IsMoving = false;
                _animator.SetBool(ANIM_PARAM_IS_MOVING, IsMoving);
                _animator.SetBool(ANIM_PARAM_IS_IDLE, GetIsIdle());
            }
        }

        private void SetMotion(float x, float y)
        {
            _motionVector.x = x;
            _motionVector.z = y;
        }

        private void SetAnimMoveParams(Vector3 motionVector)
        {
            _animator.SetFloat(ANIM_PARAM_SIDE_SPEED, motionVector.x);
            _animator.SetFloat(ANIM_PARAM_FRONT_SPEED, motionVector.z);
        }

        public void ChangeMoveSpeed(float newMoveSpeed)
        {
            _speed = newMoveSpeed;
            _animator.SetFloat(ANIM_PARAM_MOVE_SPEED_ANIM_SPEED, GetMovingAnimSpeed());
        }

        protected override void Move()
        {
            transform.position += _motionVector * _speed * Time.deltaTime;
        }

        private float GetMovingAnimSpeed()
        {
            return _speed * ANIM_SPEED_MULTIPLY_MOVE_SPEED;
        }

        #endregion

        #region Shooting

        private void OnShootingStarted(CallbackContext ctx)
        {
            IsShooting = true;
            _animator.SetBool(ANIM_PARAM_IS_SHOOTING, IsShooting);

            // ChangeRotation()
            // StartShooting()

            throw new NotImplementedException();
        }

        private void OnShootDirectionChanged(CallbackContext ctx)
        {
            // ChangeRotation
            throw new NotImplementedException();
        }

        private void OnShootingStopped(CallbackContext ctx)
        {
            Vector2 value = ctx.ReadValue<Vector2>();

            if (value.x == 0 && value.y == 0)
            {
                IsShooting = false;
                _animator.SetBool(ANIM_PARAM_IS_SHOOTING, IsShooting);
                // StopShooting()
            }

            throw new NotImplementedException();
        }

        #endregion

        public (float, float) GetMotionVectorXY()
        {
            return (_motionVector.x, _motionVector.z);
        }

        private bool GetIsIdle()
        {
            return (!IsMoving && !IsShooting);
        }
    }
}