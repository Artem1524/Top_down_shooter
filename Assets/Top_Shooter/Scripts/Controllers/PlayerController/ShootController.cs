using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.InputSystem.InputAction;

namespace TDShooter.Controllers
{
    public class ShootController : PlayerComponentController
    {
        private static string ANIM_PARAM_IS_SHOOTING = "IsShooting";

        private void OnEnable()
        {
            GetControls().Player.Shooting.performed += OnShootDirectionChanged;
            GetControls().Player.Shooting.started += OnShootingStarted;
            GetControls().Player.Shooting.canceled += OnShootingStopped;

            // ChangeMoveSpeed(_speed);
        }
    
        private void OnDisable()
        {
            GetControls().Player.Shooting.performed -= OnShootDirectionChanged;
            GetControls().Player.Shooting.started -= OnShootingStarted;
            GetControls().Player.Shooting.canceled -= OnShootingStopped;
        }
    
        private void Update()
        {
            if (_controlsMgr.IsShooting)
                Shoot();
        }

        private void OnShootingStarted(CallbackContext ctx)
        {
            _controlsMgr.IsShooting = true;
            GetAnimator().SetBool(ANIM_PARAM_IS_SHOOTING, _controlsMgr.IsShooting);

            // ChangeRotation()
            // StartShooting()

            // throw new NotImplementedException();
        }

        private void OnShootDirectionChanged(CallbackContext ctx)
        {
            // ChangeRotation
            // throw new NotImplementedException();
        }

        private void OnShootingStopped(CallbackContext ctx)
        {
            Vector2 value = ctx.ReadValue<Vector2>();

            if (value.x == 0 && value.y == 0)
            {
                _controlsMgr.IsShooting = false;
                GetAnimator().SetBool(ANIM_PARAM_IS_SHOOTING, _controlsMgr.IsShooting);
                // StopShooting()
            }

            // throw new NotImplementedException();
        }

        public void ChangeMoveSpeed(float newMoveSpeed)
        {
            // _speed = newMoveSpeed;
            // GetAnimator().SetFloat(ANIM_PARAM_MOVE_SPEED_ANIM_SPEED, GetMovingAnimSpeed());
        }

        private void Shoot()
        {
            // transform.position += _motionVector * _speed * Time.deltaTime;
        }

        private float GetMovingAnimSpeed()
        {
            return 12f;
            // return _speed * ANIM_SPEED_MULTIPLY_MOVE_SPEED;
        }
    }
}