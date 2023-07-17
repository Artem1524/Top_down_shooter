using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDShooter.Controllers
{
    public class PlayerControllersManager : MonoBehaviour
    {
        [SerializeField]
        public Animator Animator;

        public bool IsMoving { get; set; } = false;
        public bool IsShooting { get; set; } = false;
        public PlayerControls PlayerControls { get; private set; }

        private void Awake()
        {
            PlayerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            PlayerControls.Enable(); // При перекомпиляции скриптов PlayerControls становится null
        }

        private void OnDisable()
        {
            PlayerControls.Disable();
        }
    }
}