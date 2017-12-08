/********************************************************************
** Filename : MapStatistics
** Author : Dong
** Date : 2016/4/28 星期四 下午 4:08:50
** Summary : MapStatistics
***********************************************************************/

using System;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class MapStatistics
    {
        private bool _needSave;
        [SerializeField] private int _gemCount;
        [SerializeField] private int _finalCount;
        [SerializeField] private int _heroCageCount;
        [SerializeField] private int _spawnCount;
        [SerializeField] private int _monsterCount;
        [SerializeField] private int _keyCount;

        [SerializeField] private int _timeLimit;
        [SerializeField] private int _lifeCount;
        [SerializeField] private int _winCondition;
        private NetBattleData _netBattleData;
        private int _levelFinishCount;

        public EReviveType ReviveType
        {
            get
            {
                if (_netBattleData == null)
                {
                    return EReviveType.Original;
                }
                return (EReviveType) _netBattleData.ReviveType;
            }
        }

        public bool NeedSave
        {
            get { return _needSave; }
            set
            {
                if (value)
                {
                    ClearFinishCount();
                }
                _needSave = value;
            }
        }

        public int FinalCount
        {
            get { return _finalCount; }
        }

        public int SpawnCount
        {
            get { return _spawnCount; }
        }

        public int GemCount
        {
            get { return _gemCount; }
        }

        public int MonsterCount
        {
            get { return _monsterCount; }
        }

        public int HeroCageCount
        {
            get { return _heroCageCount; }
        }

        public int KeyCount
        {
            get { return _keyCount; }
        }

        public int TimeLimit
        {
            get { return _timeLimit; }
            set
            {
                if (_timeLimit == value)
                {
                    return;
                }
                NeedSave = true;
                _timeLimit = value;
            }
        }

        public int LifeCount
        {
            get { return _lifeCount; }
            set
            {
                if (_lifeCount == value)
                {
                    return;
                }
                NeedSave = true;
                _lifeCount = value;
            }
        }

        public byte WinCondition
        {
            get { return (byte) _winCondition; }
            set { _winCondition = value; }
        }

        public int MsgWinCondition
        {
            get
            {
                int result = 1;
                if (HasWinCondition(EWinCondition.WC_Monster))
                {
                    result |= 1 << (int) EWinCondition.WC_Monster;
                }
                if (HasWinCondition(EWinCondition.WC_Collect))
                {
                    result |= 1 << (int) EWinCondition.WC_Collect;
                }
                if (HasWinCondition(EWinCondition.WC_Arrive))
                {
                    result |= 1 << (int) EWinCondition.WC_Arrive;
                }
                // 
                if (result == 0)
                {
                    result |= 1 << (int) EWinCondition.WC_TimeLimit;
                }
                return result;
            }
        }

        public int LevelFinishCount
        {
            get { return _levelFinishCount; }
        }

        public NetBattleData NetBattleData
        {
            get { return _netBattleData; }
        }

        public int NetBattleTimeWinCondition
        {
            get { return _netBattleData.TimeWinCondition; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.TimeWinCondition != value)
                {
                    _netBattleData.TimeWinCondition = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleTimeLimit
        {
            get { return _netBattleData.TimeLimit; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.TimeLimit != value)
                {
                    _netBattleData.TimeLimit = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleMaxPlayerCount
        {
            get { return _netBattleData.PlayerCount; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.PlayerCount != value)
                {
                    _netBattleData.PlayerCount = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleLifeCount
        {
            get { return _netBattleData.LifeCount; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.LifeCount != value)
                {
                    _netBattleData.LifeCount = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleReviveTime
        {
            get { return _netBattleData.ReviveTime; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.ReviveTime != value)
                {
                    _netBattleData.ReviveTime = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleReviveInvincibleTime
        {
            get { return _netBattleData.ReviveInvincibleTime; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.ReviveInvincibleTime != value)
                {
                    _netBattleData.ReviveInvincibleTime = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleReviveType
        {
            get { return _netBattleData.ReviveType; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.ReviveType != value)
                {
                    _netBattleData.ReviveType = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleWinScore
        {
            get { return _netBattleData.WinScore; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.WinScore != value)
                {
                    _netBattleData.WinScore = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleArriveScore
        {
            get { return _netBattleData.ArriveScore; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.WinScore != value)
                {
                    _netBattleData.ArriveScore = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleCollectGemScore
        {
            get { return _netBattleData.CollectGemScore; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.CollectGemScore != value)
                {
                    _netBattleData.CollectGemScore = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleKillMonsterScore
        {
            get { return _netBattleData.KillMonsterScore; }
            set
            {
                if (_netBattleData == null) return;
                if (_netBattleData.KillMonsterScore != value)
                {
                    _netBattleData.KillMonsterScore = value;
                    NeedSave = true;
                }
            }
        }

        public int NetBattleKillPlayerScore
        {
            get { return _netBattleData.KillPlayerScore; }
            set
            {
                if (_netBattleData == null)
                {
                    return;
                }
                if (_netBattleData.KillPlayerScore != value)
                {
                    _netBattleData.KillPlayerScore = value;
                    NeedSave = true;
                }
            }
        }

        public bool NetBattleScoreWinCondition
        {
            get { return _netBattleData.ScoreWinCondition; }
            set
            {
                if (_netBattleData == null)
                {
                    return;
                }
                if (_netBattleData.ScoreWinCondition != value)
                {
                    _netBattleData.ScoreWinCondition = value;
                    NeedSave = true;
                }
            }
        }

        public bool InfiniteLife
        {
            get { return _netBattleData.InfiniteLife; }
            set
            {
                if (_netBattleData == null)
                {
                    return;
                }
                if (_netBattleData.InfiniteLife != value)
                {
                    _netBattleData.InfiniteLife = value;
                    NeedSave = true;
                }
            }
        }

        public bool IsMulti { get; private set; }

        public MapStatistics()
        {
            _winCondition = 1 << (int) EWinCondition.WC_Arrive | 1 << (int) EWinCondition.WC_TimeLimit;
            _timeLimit = 30;
            _lifeCount = 3;
        }

        public void SetWinCondition(EWinCondition eWinCondition, bool value)
        {
            if (HasWinCondition(eWinCondition) == value)
            {
                return;
            }
            NeedSave = true;
            if (value)
            {
                _winCondition |= 1 << (int) eWinCondition;
            }
            else
            {
                _winCondition &= ~(1 << (int) eWinCondition);
            }
        }

        public void InitWithMapData(GM2DMapData levelData)
        {
            _winCondition = levelData.WinCondition;
            _timeLimit = levelData.TimeLimit;
            _lifeCount = levelData.LifeCount;
            _levelFinishCount = levelData.FinishCount;
        }

        public void InitMultiBattleData(Project project)
        {
            _netBattleData = project.NetData;
            IsMulti = true;
        }

        public void CreateDefaltNetData(Project project)
        {
            IsMulti = true;
            _netBattleData = new NetBattleData();
            _netBattleData.ProjectId = project.ProjectId;
            SetHarmType(EHarmType.EnemyMonster, true, true);
            SetHarmType(EHarmType.EnemyPlayer, true, true);
            _netBattleData.TimeLimit = 120;
            _netBattleData.PlayerCount = 6;
            _netBattleData.LifeCount = 3;
            _netBattleData.ReviveTime = 0;
            _netBattleData.ReviveInvincibleTime = 0;
            _netBattleData.ReviveType = 0;
            _netBattleData.TimeWinCondition = 0;
            _netBattleData.WinScore = 100;
            _netBattleData.ArriveScore = 100;
            _netBattleData.CollectGemScore = 10;
            _netBattleData.KillMonsterScore = 10;
            _netBattleData.KillPlayerScore = 20;
            _netBattleData.ScoreWinCondition = false;
            _netBattleData.InfiniteLife = false;
        }

        public bool HasWinCondition(EWinCondition eWinCondition)
        {
            if (eWinCondition == EWinCondition.WC_TimeLimit)
            {
                return true;
            }
            return (_winCondition & (1 << (int) eWinCondition)) != 0;
        }

        public void AddFinishCount()
        {
            _levelFinishCount++;
        }

        public void ClearFinishCount()
        {
            _levelFinishCount = 0;
        }

        public void AddOrDeleteUnit(Table_Unit tableUnit, bool value, bool isInit = false)
        {
            if (!isInit)
            {
                NeedSave = true;
            }
            if (UnitDefine.IsSpawn(tableUnit.Id))
            {
                _spawnCount = value ? ++_spawnCount : --_spawnCount;
            }
            else if (UnitDefine.IsMonster(tableUnit.Id))
            {
                _monsterCount = value ? ++_monsterCount : --_monsterCount;
            }
            else if (tableUnit.Id == 5001)
            {
                _finalCount = value ? ++_finalCount : --_finalCount;
            }
            else if (tableUnit.Id == 5012)
            {
                _keyCount = value ? ++_keyCount : --_keyCount;
            }
            else if (tableUnit.Id == 6001)
            {
                _gemCount = value ? ++_gemCount : --_gemCount;
            }
        }

        public void AddOrDeleteConnection()
        {
            NeedSave = true;
        }

        public bool CanHarmType(EHarmType eHarmType)
        {
            if (_netBattleData == null)
            {
                return true;
            }
            return (1 << (int) eHarmType & _netBattleData.HarmType) != 0;
        }

        public void SetHarmType(EHarmType eHarmType, bool value, bool Init = false)
        {
            if (_netBattleData == null)
            {
                return;
            }
            if (CanHarmType(eHarmType) == value) return;
            if (!Init)
            {
                NeedSave = true;
            }
            if (value)
            {
                _netBattleData.HarmType |= 1 << (int) eHarmType;
            }
            else
            {
                _netBattleData.HarmType &= ~(1 << (int) eHarmType);
            }
        }
    }

    public enum EHarmType
    {
        EnemyMonster,
        EnemyPlayer,
        SelfMonster,
        SelfPlayer,
    }

    public enum EReviveType
    {
        Original,
        Nearest,
        Random
    }
}