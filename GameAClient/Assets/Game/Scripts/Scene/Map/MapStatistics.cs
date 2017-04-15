﻿/********************************************************************
** Filename : MapStatistics
** Author : Dong
** Date : 2016/4/28 星期四 下午 4:08:50
** Summary : MapStatistics
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using IntRect = SoyEngine.IntRect;

namespace GameA.Game
{
    [Serializable]
    public class MapStatistics
    {
        private bool _needSave;
        [SerializeField] private int _gemCount;
        [SerializeField] private int _finalCount;
        [SerializeField] private int _heroCageCount;
        [SerializeField] private int _mainPlayerCount;
        [SerializeField] private int _monsterCount;
        [SerializeField] private int _keyCount;

	    [SerializeField] private int _timeLimit;
	    [SerializeField] private int _lifeCount;
	    [SerializeField] private int _winCondition; 

	    private int _levelFinishCount;

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

        public int MainPlayerCount
        {
            get { return _mainPlayerCount; }
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
            get { return (byte)_winCondition; }
            set { _winCondition = value; }
        }

	    public int LevelFinishCount
	    {
		    get { return _levelFinishCount; }
	    }

        public MapStatistics()
        {
            _winCondition = 1 << (int) EWinCondition.Arrived | 1 << (int) EWinCondition.TimeLimit;
            _timeLimit = 60;
            _lifeCount = 5;
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

        public bool HasWinCondition(EWinCondition eWinCondition)
        {
            return (_winCondition & (1 << (int) eWinCondition)) != 0;
        }

        public void AddFinishCount()
        {
            _levelFinishCount ++;
        }

        public void ClearFinishCount()
        {
            _levelFinishCount = 0;
        }

        public void AddOrDelete(Table_Unit tableUnit, bool value, bool isInit = false)
        {
            if (!isInit)
            {
                NeedSave = true;
            }
            switch (tableUnit.EUnitType)
            {
                case EUnitType.MainPlayer:
                    _mainPlayerCount = value ? ++_mainPlayerCount : --_mainPlayerCount;
                    break;
                case EUnitType.Monster:
                    _monsterCount = value ? ++_monsterCount : --_monsterCount;
                    break;
                case EUnitType.Mechanism:
                    if (tableUnit.Id == 5001)
                    {
                        _finalCount = value ? ++_finalCount : --_finalCount;
                    }
                    else if (tableUnit.Id == 5012)
                    {
                        _keyCount = value ? ++_keyCount : --_keyCount;
                    }
                    break;
                case EUnitType.Collection:
                    if (tableUnit.Id == 6001)
                    {
                        _gemCount = value ? ++_gemCount : --_gemCount;
                    }
                    break;
            }
        }
    }
}