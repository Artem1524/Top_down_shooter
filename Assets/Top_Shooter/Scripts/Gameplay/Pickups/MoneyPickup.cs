using System;
using UnityEngine;

using TDShooter.Managers;

namespace TDShooter.Pickups
{
    public class MoneyPickup : MonoBehaviour
    {
        [SerializeField, Range(1, 1000)]
        private int _moneyGain = 10;

        private void OnCollisionStay(Collision collision)
        {
            ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
            collision.GetContacts(contactPoints);

            LayerMask layerMask = LayerMaskManager.PlayerLayer;

            foreach (ContactPoint point in contactPoints)
            {
                if ((layerMask.value & (1 << point.otherCollider.gameObject.layer)) != 0)
                {
                    PickupMoney(_moneyGain);
                    return;
                }
            }
        }

        private void PickupMoney(int moneyGain)
        {
            PlayerUpgradesManager.AddMoney(moneyGain);
            Destroy(gameObject);
        }
    }
}