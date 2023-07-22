using UnityEngine;

namespace TDShooter.Managers
{
    public class LayerMaskManager : MonoBehaviour
    {
        public static LayerMaskManager Self { get; private set; }

        [SerializeField]
        private LayerMask _default;
        [SerializeField]
        private LayerMask _player;
        [SerializeField]
        private LayerMask _enemy;
        [SerializeField]
        private LayerMask _prop;
        [SerializeField]
        private LayerMask _wall;
        [SerializeField]
        private LayerMask _projectile;
        [SerializeField]
        private LayerMask _pickup;

        public static LayerMask DefaultLayer => Self._default;
        public static LayerMask PlayerLayer => Self._player;
        public static LayerMask EnemyLayer => Self._enemy;
        public static LayerMask PropMask => Self._prop;
        public static LayerMask WallLayer => Self._wall;
        public static LayerMask ProjectileLayer => Self._projectile;
        public static LayerMask PickupLayer => Self._pickup;

        private void Awake()
        {
            if (Self is null)
            {
                Self = this;
            }
        }

        public static bool IsLayerExistsInLayerMask(int layer, LayerMask layerMask)
        {
            if ((layerMask.value & (1 << layer)) != 0)
                return true;
            else
                return false;
        }
    }
}