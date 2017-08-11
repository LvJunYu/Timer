using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
using GameA.Game;

namespace GameA
{
    /// <summary>
    /// 拼图
    /// </summary>
    public partial class PictureFull : SyncronisticData
    {
        //字段
        private bool _hasInited;
        private EPuzzleState _curState;
        private Table_Puzzle _puzzleTable;
        private PicturePart[] _neededFragments;
        private Dictionary<int, Table_PuzzleUpgrade> _lvTableDic;//键是等级，值是当前等级对应的信息

        //属性
        public EPuzzleState CurState { get { return _curState; } }
        public int Quality { get { return _puzzleTable.Quality; } }
        public string Name { get { return _puzzleTable.Name; } }
        public string Desc
        {
            get
            {
                if (_curState == EPuzzleState.HasActived)
                    return _lvTableDic[_level].Description;
                else
                    return _puzzleTable.Description;
            }
        }
        public int CostMoeny
        {
            get
            {
                if (_curState == EPuzzleState.HasActived)
                    return _lvTableDic[_level].UpgradeCost;
                else
                    return _puzzleTable.MergeCost;
            }
        }
        public int AttriBonus
        {
            get
            {
                if (_curState == EPuzzleState.HasActived)
                    return _lvTableDic[_level].AttriBonus;
                else
                    return _puzzleTable.AttriBonus;
            }
        }
        public Dictionary<int, Table_PuzzleUpgrade> LvTableDic { get { return _lvTableDic; } }
        public PicturePart[] NeededFragments { get { return _neededFragments; } }

        //方法
        public PictureFull(int puzzleID)
        {
            //初始化信息
            _pictureId = puzzleID;
            _level = 0;
            _isUsing = false;
            _slot = -1;

            InitData(puzzleID);
        }

        public void InitData()
        {
            if (_hasInited == true)
                return;
            InitData((int)_pictureId);
        }

        public void InitData(int puzzleID)
        {
            _puzzleTable = TableManager.Instance.GetPuzzle(puzzleID);
            _curState = EPuzzleState.CantActive;

            //合成所需碎片
            _neededFragments = new PicturePart[_puzzleTable.FragNum];
            for (int i = 0; i < _neededFragments.Length; i++)
            {
                _neededFragments[i] = PicturePart.GetPicturePart((int)this._pictureId, i + 1);
            }

            //等级信息字典
            _lvTableDic = new Dictionary<int, Table_PuzzleUpgrade>();
            var table = TableManager.Instance.Table_PuzzleUpgradeDic;
            foreach (int key in table.Keys)
            {
                Table_PuzzleUpgrade value = table[key];
                if (value.PuzzleID == this._pictureId)
                    _lvTableDic.Add(value.Level, value);
            }
            _hasInited = true;
        }

        public void ActivatePuzzle()
        {
            _level++;
            _curState = EPuzzleState.HasActived;
        }

        public void EquipPuzzle(int slotID)
        {
            _isUsing = true;
            _slot = slotID;
        }

        public void Unload()
        {
            _isUsing = false;
            _slot = -1;
        }

        public void UpdateState()
        {
            if (_curState == EPuzzleState.CantActive && CheckActivatable())
                _curState = EPuzzleState.CanActive;
            else if (_curState == EPuzzleState.CanActive && !CheckActivatable())
                _curState = EPuzzleState.CantActive;
        }

        private bool CheckActivatable()
        {
            if (_level == _puzzleTable.MaxLevel)
                return false;
            for (int i = 0; i < _neededFragments.Length; i++)
            {
                if (!(_neededFragments[i].TotalCount > 0))
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 拼图状态
    /// </summary>
    public enum EPuzzleState
    {
        None,
        CantActive,//不可激活
        CanActive,//可激活
        HasActived,//已激活
        //HasEquiped//已装备
    }
}
