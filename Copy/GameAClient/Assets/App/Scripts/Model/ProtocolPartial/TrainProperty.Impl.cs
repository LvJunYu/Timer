using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
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

        private const int _maxLv = 15;
        private UserTrainProperty _userTrainProperty;
        private Dictionary<int, Table_CharacterUpgrade> _lvDic;

        private Dictionary<int, Table_CharacterUpgrade> LvDic
        {
            get
            {
                if (null == _lvDic)
                {
                    var table_CharacterUpgradeDic = TableManager.Instance.Table_CharacterUpgradeDic;
                    _lvDic = new Dictionary<int, Table_CharacterUpgrade>(_maxLv);
                    foreach (Table_CharacterUpgrade value in table_CharacterUpgradeDic.Values)
                    {
                        if (value.Property == (int) _property)
                            _lvDic.Add(value.Level, value);
                    }
                }
                return _lvDic;
            }
        }

        public int CostTrainPoint
        {
            get { return LvDic[NextLevel].DevelopPoint; }
        }

        public int CostGold
        {
            get { return LvDic[NextLevel].TrainingPrice; }
        }

        public int Time
        {
            get { return LvDic[NextLevel].TrainingTime; }
        }

        public string TimeDesc
        {
            get { return GetTimeString(Time); }
        }

        public int NextLevel
        {
            get
            {
                int maxLv = _gradeMaxLv[_gradeMaxLv.Length - 1];
                if (_level < maxLv)
                    return _level + 1;
                return maxLv;
            }
        }

        public int MaxLv
        {
            get
            {
                if (null == _userTrainProperty)
                    _userTrainProperty = LocalUser.Instance.UserTrainProperty;
                int grade = _userTrainProperty.Grade;
                if (grade > _gradeMaxLv.Length)
                    return _gradeMaxLv[_gradeMaxLv.Length - 1];
                return _gradeMaxLv[grade - 1];
            }
        }

        public float Value
        {
            get { return LvDic[_level].Value; }
        }

        public string ValueDesc
        {
            get { return GetValueString(Value); }
        }

        public string NextValueDesc
        {
            get { return GetValueString(LvDic[NextLevel].Value); }
        }

        public int RemainTrainingTime
        {
            get { return (int) (RemainTrainingTimeMill / 1000); }
        }

        public long RemainTrainingTimeMill
        {
            get
            {
                if (!_isTraining) return 0;
                long trainingTime = DateTimeUtil.GetServerTimeNowTimestampMillis() - _trainStartTime;
                return Time * 1000 - trainingTime;
            }
        }

        public string RemainTrainingTimeDesc
        {
            get { return GetTimeString(RemainTrainingTime); }
        }

        public int FinishCostDiamond
        {
            get { return (int) Math.Ceiling(RemainTrainingTime / (double) LvDic[NextLevel].SecondsPerDiamond); }
        }

        public TrainProperty(int propertyId, int level)
        {
            Property = (ETrainPropertyType) propertyId;
            Level = level;
        }

        public void FinishUpgrade()
        {
            _level++;
            _isTraining = false;
            Messenger.Broadcast(EMessengerType.OnUpgradeTrainProperty);
        }

        public void StartUpgrade()
        {
            _isTraining = true;
            _trainStartTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
            Messenger.Broadcast(EMessengerType.OnUpgradeTrainProperty);
        }

        private string GetValueString(float value)
        {
            switch (Property)
            {
                case ETrainPropertyType.TPT_HPRegeneration:
                    return string.Format("{0}HP/s", value);
                case ETrainPropertyType.TPT_RunSpeed:
                    return string.Format("{0}m/s", value);
                case ETrainPropertyType.TPT_JumpHeight:
                    return string.Format("{0}m", value);
                case ETrainPropertyType.TPT_AntiStrike:
                    return value.ToString();
                case ETrainPropertyType.TPT_Magnet:
                    return string.Format("{0}m", value);
                default:
                    return null;
            }
        }

        private string GetTimeString(int second)
        {
            if (second < 60)
                return string.Format("{0} 秒", second);
            else if (second >= 60 && second < 3600)
                return string.Format("{0} 分钟 {1}", second / 60,
                    second % 60 == 0 ? "" : string.Format("{0} 秒", second % 60));
            else
                return string.Format("{0} 小时 {1}", second / 3600,
                    (second / 60) % 60 == 0 ? "" : string.Format("{0} 分", (second / 60) % 60));
        }
    }
}