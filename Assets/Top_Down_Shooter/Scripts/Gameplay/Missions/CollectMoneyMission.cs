using System;
using TDShooter.Managers;

namespace TDShooter
{
    [Serializable]
    public class CollectMoneyMission : Mission
    {
        private const string MISSION_TEXT = "Набрать {0} монет";

        private int _moneyCountNeed = 300;
        private int _baseMoney = 0;

        public override void Initialize()
        {
            _baseMoney = PlayerUpgradesManager.Self.Money;
        }

        public override bool CheckMission()
        {
            if (GetActualMoney() - _baseMoney >= _moneyCountNeed)
                return true;

            return false;
        }

        public override string GetMissionInfo()
        {
            int needCount = _baseMoney + _moneyCountNeed - GetActualMoney();
            return String.Format(MISSION_TEXT, (needCount < 0) ? 0 : needCount);
        }

        public int GetActualMoney()
        {
            return PlayerUpgradesManager.Self.Money;
        }
    }
}