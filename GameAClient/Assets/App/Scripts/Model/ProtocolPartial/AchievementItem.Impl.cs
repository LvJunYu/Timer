using System;
using System.Collections.Generic;
using System.Linq;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    /// <summary>
    /// 成就条目
    /// </summary>
    public partial class AchievementItem
    {
        private Dictionary<int, Table_Achievement> _lvDic;
        private int _curLevel = -1;
        public int Type;
        public int CurValue;

        public Dictionary<int, Table_Achievement> LvDic
        {
            get { return _lvDic; }
        }

        public int FinishLevel
        {
            get
            {
                if (-1 == _curLevel)
                {
                    _curLevel = 0;
                    for (int i = 1; i < LvDic.Count; i++)
                    {
                        if (LvDic[i].Condition <= CurValue)
                            _curLevel = i;
                        else
                            break;
                    }
                }
                return _curLevel;
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
                if (FinishLevel + 1 > MaxLevel)
                    return null;
                return FinishLevel + 1;
            }
        }

        public AchievementItem(int type, int value)
        {
            Type = type;
            CurValue = value;
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