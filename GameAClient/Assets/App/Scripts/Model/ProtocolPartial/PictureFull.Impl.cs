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
        private bool _Inited;
        private PuzzleState _curState;
        private Table_Puzzle _puzzleTable;
        private PicturePart[] _neededFragments;
        private Dictionary<int, Table_PuzzleUpgrade> _lvTableDic;//键是等级，值是当前等级对应的信息

        //属性
        public PuzzleState CurState { get { return _curState; } }
        public int Quality { get { return _puzzleTable.Quality; } }
        public string Name { get { return _puzzleTable.Name; } }
        public string Desc
        {
            get
            {
                if (_lvTableDic.ContainsKey(_level))
                    return _lvTableDic[_level].Description;
                else
                    return _puzzleTable.Description;
            }
        }
        public int CostMoeny { get
            {
                if (_lvTableDic.ContainsKey(_level))
                    return _lvTableDic[_level].UpgradeCost;
                else
                    return _puzzleTable.MergeCost;
            } }
        public Dictionary<int, Table_PuzzleUpgrade> LvTableDic { get { return _lvTableDic; } }
        public PicturePart[] NeededFragments { get { return _neededFragments; } }

        //方法
        public PictureFull(Table_Puzzle puzzleTable)
        {
            //初始化信息
            _pictureId = puzzleTable.Id;
            _level = 0;
            _isUsing = false;
            _slot = -1;

            InitData(puzzleTable);
        }

        public void InitData()
        {
            if (_inited == true)
                return;
            var puzzleTable = TableManager.Instance.GetPuzzle((int)_pictureId);
            InitData(puzzleTable);
        }

        public void InitData(Table_Puzzle puzzleTable)
        {
            _puzzleTable = puzzleTable;
            _curState = PuzzleState.CantActive;

            //合成所需碎片
            var FragmentIDs = _puzzleTable.Fragments;
            _neededFragments = new PicturePart[FragmentIDs.Length];
            for (int i = 0; i < FragmentIDs.Length; i++)
            {
                var Fragment = TableManager.Instance.GetPuzzleFragment(FragmentIDs[i]);
                _neededFragments[i] = new PicturePart(Fragment);
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
            _Inited = true;
        }

        private void InitLvDic()
        {

        }

        private void UpdateDesc()
        {
            //Desc = puzzle.Description;

        }

        public void ActivatePuzzle()
        {
            _level++;
            _curState = PuzzleState.HasActived;
        }

        public void EquipPuzzle()
        {
            _isUsing = true;
            //_slot = 1;
        }

        public void Unload()
        {
            _isUsing = false;
            _slot = -1;
        }

        public void UpdateState()
        {
            if (_curState == PuzzleState.CantActive && CheckActivatable())
                _curState = PuzzleState.CanActive;
            else if (_curState == PuzzleState.CanActive && !CheckActivatable())
                _curState = PuzzleState.CantActive;

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
    public enum PuzzleState
    {
        None,
        CantActive,//不可激活
        CanActive,//可激活
        HasActived,//已激活
        //HasEquiped//已装备
    }
}
