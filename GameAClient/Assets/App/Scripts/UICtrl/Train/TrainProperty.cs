using System.Collections.Generic;
using GameA.Game;
using SoyEngine.Proto;

namespace GameA
{
    /// <summary>
    /// 训练的属性
    /// </summary>
    public partial class TrainProperty
    {
        private int[] _gradeMaxLv =
        {
            3, 6, 10, 15
        };

        private int _curGrade;
        private Dictionary<int, Table_CharacterUpgrade> _lvDic;

        public int Cost
        {
            get { return _lvDic[_level+1].DevelopPoint; }
        }

        public int Time
        {
            get { return _lvDic[_level+1].TrainingTime; }
        }

        public int MaxLv
        {
            get { return _gradeMaxLv[_curGrade - 1]; }
        }

        public float Value
        {
            get { return _lvDic[_level].Value; }
        }

        public TrainProperty(int propertyId, int level, int curGrade)
        {
            _property = (ETrainPropertyType) propertyId;
            _level = level;
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