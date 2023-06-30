using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.InputSystem.InputAction;

namespace TDShooter.Controllers
{
    public class MoveController : PlayerComponentController
    {
        private static string ANIM_PARAM_IS_MOVING = "IsMoving";
        private static string ANIM_PARAM_IS_IDLE = "IsIdle";
        private static string ANIM_PARAM_SIDE_SPEED = "SideMove";
        private static string ANIM_PARAM_FRONT_SPEED = "FrontMove";
        private static string ANIM_PARAM_MOVE_SPEED_ANIM_SPEED = "MoveSpeedAnimSpeed";

        [SerializeField, Range(0.05f, 10f)]
        [Tooltip("Ìíîæèòåëü ñêîðîñòè âîñïðîèçâåäåíèÿ àíèìàöèè ïåðåìåùåíèÿ (îòíîñèòåëüíî ñêîðîñòè ïåðåìåùåíèÿ)")]
        private float _animSpeedMoveSpeedMultiply = 0.7f;
        [SerializeField, Range(0.1f, 20f)]
        [Tooltip("Базовая скорость игрока")]
        private float _speed = 5f;

        private Vector3 _motionVector = new Vector3();
    
        private void OnEnable()
        {
            GetControls().Player.Moving.performed += OnPlayerMove;
            GetControls().Player.Moving.started += OnPlayerStartMoving;
            GetControls().Player.Moving.canceled += OnPlayerStopMoving;

            ChangeMoveSpeed(_speed);
        }
    
        private void OnDisable()
        {
            GetControls().Player.Moving.performed -= OnPlayerMove;
            GetControls().Player.Moving.started -= OnPlayerStartMoving;
            GetControls().Player.Moving.canceled -= OnPlayerStopMoving;
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
            GetAnimator().SetBool(ANIM_PARAM_IS_IDLE, GetIsIdle());
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
                GetAnimator().SetBool(ANIM_PARAM_IS_IDLE, GetIsIdle());
            }
        }

        private void SetMotion(float x, float y)
        {
            _motionVector.x = x;
            _motionVector.z = y;
        }

        private void SetAnimMoveParams(Vector3 motionVector)
        {
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
    }
}