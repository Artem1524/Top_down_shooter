using System;
using TDShooter.Managers;

namespace TDShooter
{
    [Serializable]
    public class CollectMoneyMission : Mission
    {
        private const string MISSION_TEXT = "Набрать {0} монет";

        private int _moneyCountNeed = 150;

        public override bool CheckMission()
        {
            if (PlayerUpgradesManager.Self.Money >= _moneyCountNeed)
                return true;

            return false;
        }

        public override string GetMissionInfo()
        {
            return String.Format(MISSION_TEXT, _moneyCountNeed);
        }
    }
}