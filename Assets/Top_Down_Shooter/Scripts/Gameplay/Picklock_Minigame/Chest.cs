using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace TDShooter
{
    public class Chest : MonoBehaviour
    {
        private const string ANIM_TRIGGER_OPEN = "Open";

        [SerializeField]
        private NearbyPlayerChecker _nearbyPlayerChecker;
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private int _coinCount = 5;
        [SerializeField]
        private Pickup _coinPrefab;

        public void PlayOpenChestAnimation()
        {
            _animator.SetTrigger(ANIM_TRIGGER_OPEN);
        }

        public void DisableNearbyPlayerChecker()
        {
            _nearbyPlayerChecker.enabled = false;
        }

        public void DropMoney()
        {
            for (int i = 0; i < _coinCount; i++)
            {
                Vector3 deltaPos = GenerateRandomDeltaPosition();
                Vector3 dropPosition = new Vector3(
                                                    transform.position.x + deltaPos.x,
                                                    transform.position.y + deltaPos.y,
                                                    transform.position.z + deltaPos.z
                                                  );
                PickupCreatorHelper.InstantiatePickup<Pickup>(_coinPrefab, dropPosition, _coinPrefab.transform.rotation);
            }
        }

        private Vector3 GenerateRandomDeltaPosition()
        {
            float coinDropHeight = 2.5f;
            float posOffset = 3f;
            int randomSeed = Random.Range(1, 5);

            switch (randomSeed)
            {
                case 1:
                    return new Vector3(posOffset, coinDropHeight, 0);
                case 2:
                    return new Vector3(-posOffset, coinDropHeight, 0);
                case 3:
                    return new Vector3(0, coinDropHeight, posOffset);
                default:
                    return new Vector3(0, coinDropHeight, -posOffset);
            }
        }
    }
}