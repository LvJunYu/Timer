using System.Collections.Generic;
using GameA.Game;
using SoyEngine;

namespace GameA
{
    public partial class Achievement
    {
        private Dictionary<int, AchievementStatisticItem> _achieveDic =
            new Dictionary<int, AchievementStatisticItem>(100);

        private List<AchievementStatisticItem> _unFinishedAchieve = new List<AchievementStatisticItem>();
        private List<AchievementStatisticItem> _finishedAchive = new List<AchievementStatisticItem>();
        private List<AchiveItemData> _allAchiveData = new List<AchiveItemData>();
        private bool _hasBuild;

        public List<AchiveItemData> AllAchiveData
        {
            get { return _allAchiveData; }
        }

        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            BuildData();
        }

        public void BuildData()
        {
            var achievements = TableManager.Instance.Table_AchievementDic;
            //建立type字典
            foreach (Table_Achievement value in achievements.Values)
            {
                AchievementStatisticItem achievementItem = StatisticList.Find(p => p.Type == value.Type);
                if (null == achievementItem)
                {
                    if (!_achieveDic.ContainsKey(value.Type))
                    {
                        _achieveDic.Add(value.Type, new AchievementStatisticItem(value.Type, 0));
                    }
                }
                else
                {
                    if (!_achieveDic.ContainsKey(value.Type))
                    {
                   
                        _achieveDic.Add(value.Type, achievementItem);
                    }
                    else
                    {
                        _achieveDic[value.Type].DeepCopy(achievementItem);
                    }
                }
                
                _achieveDic[value.Type].SetValue(value.Level, value);
            }
            //建立未完成和已完成成就列表
            _unFinishedAchieve.Clear();
            _finishedAchive.Clear();
            foreach (var achieve in _achieveDic.Values)
            {
                if (achieve.CurLv <= achieve.MaxLevel)
                {
                    _unFinishedAchieve.Add(achieve);
                }
                if (achieve.CurLv > 1)
                {
                    _finishedAchive.Add(achieve);
                }
            }
            RefreshAchieveData();
            _hasBuild = true;
        }

        private void RefreshAchieveData()
        {
            _allAchiveData.Clear();
            for (int i = 0; i < _unFinishedAchieve.Count; i++)
            {
                _allAchiveData.Add(new AchiveItemData(_unFinishedAchieve[i], false));
            }
            for (int i = 0; i < _finishedAchive.Count; i++)
            {
                _allAchiveData.Add(new AchiveItemData(_finishedAchive[i], true));
            }
        }

        public void AddAchievementCount(int type, int addCount)
        {
            if (!_hasBuild)
            {
                BuildData();
            }
            if (!_achieveDic.ContainsKey(type))
            {
                LogHelper.Error("don't have achievement type == {0}", type);
            }
            else
            {
                _achieveDic[type].AddAchievementCount(addCount);
            }
        }
    }
}