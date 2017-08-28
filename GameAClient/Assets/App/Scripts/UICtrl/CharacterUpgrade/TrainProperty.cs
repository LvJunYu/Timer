using System.Collections;
using System;
using System.Collections.Generic;
using GameA.Game;
using UnityEngine;
using Text = UnityEngine.UI.Text;

namespace GameA
{
    /// <summary>
    /// 训练的属性
    /// </summary>
    public class TrainProperty : SyncronisticData
    {
        private int[] _gradeMaxLv =
        {
            2, 5, 9, 14
        };

        private int _property;
        private int _curLv;
        private int _curGrade;
        private Dictionary<int, Table_CharacterUpgrade> _lvDic;

        public int Property
        {
            get { return _property; }
        }

        public int CurLv
        {
            get { return _curLv; }
        }

        public int Cost
        {
            get { return _lvDic[_curLv].DevelopPoint; }
        }

        public int Time
        {
            get { return _lvDic[_curLv].TrainingTime; }
        }

        public int MaxLv
        {
            get { return _gradeMaxLv[_curGrade]; }
        }

        public TrainProperty(int property, int level, int curGrade)
        {
            _property = property;
            _curLv = level;
            _curGrade = curGrade;
            var table_CharacterUpgradeDic = TableManager.Instance.Table_CharacterUpgradeDic;
            _lvDic = new Dictionary<int, Table_CharacterUpgrade>(14);
            foreach (Table_CharacterUpgrade value in table_CharacterUpgradeDic.Values)
            {
                if (value.Property == _property)
                    _lvDic.Add(value.Level, value);
            }
        }

        private int GetGrade(int lv)
        {
            int grade = 0;
            for (int i = 0; i < _gradeMaxLv.Length; i++)
            {
                if (_gradeMaxLv[i] > lv)
                    grade = i;
                else
                    break;
            }
            return grade;
        }
    }
}