using UnityEngine;
using System.Collections;

using TDShooter.Weapons;
using TDShooter.Managers;

using static UnityEngine.InputSystem.InputAction;

namespace TDShooter.Controllers
{
    public class ShootController : PlayerComponentController
    {
        private const string ANIM_TRIGGER_SHOOT = "OnShoot";
        private const float RAD_TO_DEG = Mathf.Rad2Deg;

        [SerializeField]
        private Transform _bulletPool;
        [SerializeField, Space]
        private Transform _mesh;
        [SerializeField]
        private WeaponType _switchWeaponType1;
        [SerializeField]
        private WeaponType _switchWeaponType2;
        [SerializeField]
        private WeaponType _switchWeaponType3;

        private bool _shootActionsNotAdded = true;
        private float _shootReloadTimer = 0f;
        private float _firstShotTimer;

        private bool _isFirstShootMaking = false;

        private Coroutine _firstShotCoroutine = null;

        private void Start()
        {
            if (_shootActionsNotAdded)
                AddShootActions();
        }

        private void OnEnable()
        {
            if (GetControls() is not null)
                AddShootActions();
        }
    
        private void OnDisable()
        {
            GetControls().Player.Shooting.performed -= OnShootDirectionChanged;//
            GetControls().Player.Shooting.started -= OnShootingStarted;
            GetControls().Player.Shooting.canceled -= OnShootingStopped;

            GetControls().Player.SwitchWeapon1.performed -= OnSwitchedWeapon1;
            GetControls().Player.SwitchWeapon2.performed -= OnSwitchedWeapon2;
            GetControls().Player.SwitchWeapon3.performed -= OnSwitchedWeapon3;

            _shootActionsNotAdded = true;
        }
    
        private void Update()
        {
            if (CanShoot())
                Shoot();
        }

        private void OnShootingStarted(CallbackContext ctx)
        {
            ChangeRotation(ctx.ReadValue<Vector2>());

            _firstShotTimer = WeaponManager.GetCurrentWeapon().WeaponStatistics.DelayBeforeFirstShot;
            _isFirstShootMaking = true;
            StartCoroutine(TryShootFirstTime());
        }

        private void OnShootDirectionChanged(CallbackContext ctx)
        {
            ChangeRotation(ctx.ReadValue<Vector2>());
        }

        private void OnShootingStopped(CallbackContext ctx)
        {
            Vector2 value = ctx.ReadValue<Vector2>();

            if (value.x == 0 && value.y == 0)
            {
                _controlsMgr.IsShooting = false;
                _isFirstShootMaking = false;
            }
        }

        private void ChangeRotation(Vector2 controllerCoords)
        {
            float currentRotation = _mesh.localRotation.eulerAngles.y;

            float stickAngle = GetStickAngle(controllerCoords);

            float deltaAngle = stickAngle - currentRotation;
            _mesh.Rotate(_mesh.up, deltaAngle);
        }

        private void AddShootActions()
        {
            GetControls().Player.Shooting.performed += OnShootDirectionChanged;
            GetControls().Player.Shooting.started += OnShootingStarted;
            GetControls().Player.Shooting.canceled += OnShootingStopped;

            GetControls().Player.SwitchWeapon1.performed += OnSwitchedWeapon1;
            GetControls().Player.SwitchWeapon2.performed += OnSwitchedWeapon2;
            GetControls().Player.SwitchWeapon3.performed += OnSwitchedWeapon3;

            _shootActionsNotAdded = false;
        }

        #region SwitchWeapon

        private void OnSwitchedWeapon1(CallbackContext ctx) => OnSwitchedWeapon(_switchWeaponType1);

        private void OnSwitchedWeapon2(CallbackContext ctx) => OnSwitchedWeapon(_switchWeaponType2);

        private void OnSwitchedWeapon3(CallbackContext ctx) => OnSwitchedWeapon(_switchWeaponType3);

        private void OnSwitchedWeapon(WeaponType weaponType)
        {
            if (WeaponManager.GetCurrentWeapon().GetWeaponType() != weaponType)
                WeaponManager.Self.SwitchWeapon(weaponType);
        }

        #endregion

        #region Shooting

        private bool CanShoot()
        {
            if (_shootReloadTimer > 0f && _shootReloadTimer > Time.deltaTime)
            {
                _shootReloadTimer -= Time.deltaTime;
                return false;
            }
            else
            {
                _shootReloadTimer = 0f;
            }

            if (!_controlsMgr.IsShooting)
                return false;

            return true;
        }

        private IEnumerator TryShootFirstTime()
        {
            while (_firstShotTimer > 0f)
            {
                yield return null;

                if (!_isFirstShootMaking)
                {
                    _firstShotTimer = 0f;
                    _firstShotCoroutine = null;
                    yield break;
                }
                _firstShotTimer -= Time.deltaTime;
            }

            _firstShotTimer = 0f;
            _firstShotCoroutine = null; // null, если заново стреляем

            BeginShooting();

            yield break; // IsShooting = false;
        }

#nullable enable
        private void Shoot()
        {
            BulletSpawn? bulletSpawnPoint = WeaponManager.GetWeaponBulletSpawn();
            WeaponManager.GetCurrentWeapon().Shoot(_bulletPool, transform, bulletSpawnPoint);
            float fireRate = WeaponManager.GetCurrentWeapon().WeaponStatistics.FireRate;
            _shootReloadTimer = 1f / fireRate;
            GetAnimator().SetTrigger(ANIM_TRIGGER_SHOOT);
        }
#nullable disable

        private void BeginShooting()
        {
            _controlsMgr.IsShooting = true;
            GetAnimator().SetTrigger(ANIM_TRIGGER_SHOOT);
        }

        #endregion

        private float GetStickAngle(Vector2 controllerCoords)
        {
            float angleRelativeToAxisY = Mathf.Acos(controllerCoords.y) * RAD_TO_DEG;

            if (controllerCoords.x < 0) // if (-1 <= x < 0)
                return 360 - angleRelativeToAxisY;   // formula: 360N - angle (where N any integer, for example N = 1)   N любое целое число
            else                        // if (0 <= x <= 1)
                return angleRelativeToAxisY;         // formula: 360N + angle (for example N = 0)
        }
    }
}