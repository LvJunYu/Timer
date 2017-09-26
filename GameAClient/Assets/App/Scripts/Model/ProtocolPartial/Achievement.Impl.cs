using System.Collections.Generic;
using GameA.Game;
using SoyEngine;

namespace GameA
{
    public partial class Achievement
    {
        private const int _maxAchievementNum = 40;
        private Dictionary<int, AchievementStatisticItem> _allAchievements;
        private bool _hasInited;

        public Dictionary<int, AchievementStatisticItem> AllAchievements
        {
            get
            {
                if (!_hasInited)
                {
                    RequestData();
                }
                return _allAchievements;
            }
        }

        private void RequestData()
        {
//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            Request(LocalUser.Instance.UserGuid,
                () =>
                {
                    InitData();
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                },
                code =>
                {
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Error("Network error when get Achievement, {0}", code);
                });
            InitData();
        }

        private void InitData()
        {
            if (null == _allAchievements)
                _allAchievements = new Dictionary<int, AchievementStatisticItem>(_maxAchievementNum);
            var achievements = TableManager.Instance.Table_AchievementDic;
            foreach (Table_Achievement value in achievements.Values)
            {
                AchievementStatisticItem achievementItem;
                if (!_allAchievements.ContainsKey(value.Type))
                {
                    achievementItem = StatisticList.Find(p => p.Type == value.Type);
                    if (null == achievementItem)
                    {
                        //测试数据
//                        long count = Random.Range(1, 10);
                        achievementItem = new AchievementStatisticItem(value.Type, 1);
                    }
                    _allAchievements.Add(value.Type, achievementItem);
                }
                else
                    achievementItem = _allAchievements[value.Type];
                achievementItem.BuildLvDic(value.Level, value);
            }
            _hasInited = true;
        }

        public void AddAchievementCount(int type, int addCount)
        {
            if (!_hasInited)
                InitData();
            if (!_allAchievements.ContainsKey(type))
            {
                LogHelper.Error("Add achievement count but _allAchievements.ContainsKey(type) is false!");
                return;
            }
            if (_allAchievements.ContainsKey(type))
            {
                _allAchievements[type].AddAchievementCount(addCount);
            }
        }
    }
}