using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDShooter.Controllers
{
    [RequireComponent(typeof(PlayerControllersManager))]
    public class PlayerComponentController : MonoBehaviour
    {
        protected PlayerControllersManager _controlsMgr;

        protected void Awake()
        {
            _controlsMgr = GetComponent<PlayerControllersManager>();
        }

        protected bool GetIsIdle()
        {
            return (!_controlsMgr.IsMoving && !_controlsMgr.IsShooting);
        }

        protected Animator GetAnimator()
        {
            return _controlsMgr.Animator;
        }

        protected PlayerControls GetControls()
        {
            return _controlsMgr.PlayerControls;
        }
    }
}