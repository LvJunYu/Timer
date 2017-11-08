using System.Collections.Generic;
using UnityEditor;

namespace GameA
{
    public class RewardSave
    {
        private RewardSave()
        {
            
        }

        private static RewardSave _rewardSave;

        public static RewardSave Instance
        {
            get
            {
                if (_rewardSave == null)
                {
                    _rewardSave = new RewardSave();
                }return _rewardSave;
            }
            set { _rewardSave = value; }
        }

        public bool IsQQBlueNewPlayerColltion = false;
        public List<int> IsQQBlueEveryDayColltion = new List<int>();
        public bool IsQQHallNewPlayerColltion = false;
        public List<int> IsQQHallEveryDayColltion = new List<int>();
        public string RewardKey = "QQReward";
    }
}