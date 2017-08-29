using System.Collections;
using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine.Proto;
using UnityEngine;
using Text = UnityEngine.UI.Text;

namespace GameA
{
    /// <summary>
    /// 训练的属性
    /// </summary>
    public partial class TrainProperty : SyncronisticData
    {
        private int[] _gradeMaxLv =
        {
            3, 6, 10, 15
        };

        private int _curLv;
        private int _curGrade;
        private Dictionary<int, Table_CharacterUpgrade> _lvDic;

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
            get { return _gradeMaxLv[_curGrade - 1]; }
        }

        public TrainProperty(int propertyId, int level, int curGrade)
        {
            _property = (ETrainPropertyType) propertyId;
            _curLv = level;
            _curGrade = curGrade;
            var table_CharacterUpgradeDic = TableManager.Instance.Table_CharacterUpgradeDic;
            _lvDic = new Dictionary<int, Table_CharacterUpgrade>(14);
            foreach (Table_CharacterUpgrade value in table_CharacterUpgradeDic.Values)
            {
                if (value.Property == (int) _property)
                    _lvDic.Add(value.Level, value);
            }
        }
    }
}