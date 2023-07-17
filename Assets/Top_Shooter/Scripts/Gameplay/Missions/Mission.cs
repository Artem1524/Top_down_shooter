using System;
using UnityEngine;

namespace TDShooter
{
    [Serializable]
    public abstract class Mission : MonoBehaviour
    {
        public abstract bool CheckMission();
        public abstract string GetMissionInfo();
    }
}