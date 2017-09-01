using System.Collections.Generic;
using System.Linq;
using GameA.Game;

namespace GameA
{
    /// <summary>
    /// 成就条目
    /// </summary>
    public partial class AchievementStatisticItem
    {
        private Dictionary<int, Table_Achievement> _lvDic;
        private int _curLevel;

        public Dictionary<int, Table_Achievement> LvDic
        {
            get { return _lvDic; }
        }

        public int FinishLevel
        {
            get { return CurLevel - 1; }
        }

        public int CurLevel
        {
            get
            {
                if (0 == _curLevel)
                {
                    _curLevel = 1;
                    for (int level = 1; level <= LvDic.Count; level++)
                    {
                        if (_count >= LvDic[level].Condition)
                            _curLevel = level + 1;
                        else
                            break;
                    }
                }
                return _curLevel;
            }
            set { _curLevel = value; }
        }

        public int MaxLevel
        {
            get { return _lvDic.Keys.Max(); }
        }

        public int? NextLevel
        {
            get
            {
                if (CurLevel > MaxLevel)
                    return null;
                return CurLevel;
            }
        }

        public AchievementStatisticItem(int type, int count)
        {
            Type = type;
            Count = count;
        }

        public void AddLvDic(int lv, Table_Achievement achievement)
        {
            if (null == _lvDic)
                _lvDic = new Dictionary<int, Table_Achievement>(10);
            if (_lvDic.ContainsKey(lv))
                return;
            _lvDic.Add(lv, achievement);
        }
    }
}