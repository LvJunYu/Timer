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

        private int _curGrade;
        private Dictionary<int, Table_CharacterUpgrade> _lvDic;

        public int CostTrainPoint
        {
            get { return _lvDic[_level + 1].DevelopPoint; }
        }

        public int CostGold
        {
            get { return _lvDic[_level + 1].TrainingPrice; }
        }

        public int Time
        {
            get { return _lvDic[_level + 1].TrainingTime; }
        }

        public string TimeDesc
        {
            get { return GetTimeString(Time); }
        }

        public int MaxLv
        {
            get { return _gradeMaxLv[_curGrade - 1]; }
        }

        public float Value
        {
            get { return _lvDic[_level].Value; }
        }

        public string ValueDesc
        {
            get { return GetValueString(_lvDic[_level].Value); }
        }

        public string NextValueDesc
        {
            get { return GetValueString(_lvDic[_level + 1].Value); }
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

        public int FinishCost
        {
            get { return (int) Math.Ceiling(RemainTrainingTime / (double) _lvDic[_level + 1].SecondsPerDiamond); }
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
        
        public void FinishUpgrade()
        {
            _level++;
            _isTraining = false;
            Messenger.Broadcast(EMessengerType.OnCharacterUpgradeProperty);
        }

        public void StartUpgrade()
        {
            _isTraining = true;
            _trainStartTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
            Messenger.Broadcast(EMessengerType.OnCharacterUpgradeProperty);
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