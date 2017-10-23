using System.Collections.Generic;
using System.Linq;
using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class AchiveItemData
    {
        public AchievementStatisticItem AchievementStatisticItem;
        public bool Finish;

        public AchiveItemData(AchievementStatisticItem achievementStatisticItem, bool finish)
        {
            AchievementStatisticItem = achievementStatisticItem;
            Finish = finish;
        }
    }
    
    public partial class AchievementStatisticItem
    {
        private Dictionary<int, Table_Achievement> _lvDic = new Dictionary<int, Table_Achievement>(5);
        private int _curLv;

        public Dictionary<int, Table_Achievement> LvDic
        {
            get { return _lvDic; }
        }

        public int FinishLevel
        {
            get { return CurLv - 1; }
        }

        public int CurLv
        {
            get
            {
                _curLv = 1;
                for (int level = 1; level <= LvDic.Count; level++)
                {
                    if (_count >= LvDic[level].Condition)
                        _curLv = level + 1;
                    else
                        break;
                }
                return _curLv;
            }
        }

        public int MaxLevel
        {
            get { return _lvDic.Keys.Max(); }
        }

        public int? NextLevel
        {
            get
            {
                if (CurLv > MaxLevel)
                    return null;
                return CurLv;
            }
        }

        public AchievementStatisticItem(int type, long count)
        {
            Type = type;
            Count = count;
        }

        public void SetValue(int lv, Table_Achievement achievement)
        {
            if (!_lvDic.ContainsKey(lv))
            {
                _lvDic.Add(lv, achievement);
            }
        }

        public void AddAchievementCount(int addCount)
        {
            int level = CurLv;
            Count += addCount;
            while (_lvDic.ContainsKey(level) && Count >= _lvDic[level].Condition)
            {
                Messenger<Table_Achievement>.Broadcast(EMessengerType.OnAchieve, _lvDic[level]);
                level++;
            }
        }
    }
}