using UnityEngine;

namespace TDShooter.UI.PicklockGame
{
    public class PicklockGameGearObj : MonoBehaviour
    {
        private PicklockGameGearImage _gearImage;

        private void Awake()
        {
            _gearImage = GetComponentInChildren<PicklockGameGearImage>();
            DebugUtility.HandleErrorIfNullGetComponentInChildren<PicklockGameGearImage, PicklockGameGearObj>(_gearImage, this, gameObject);
        }
    }
}