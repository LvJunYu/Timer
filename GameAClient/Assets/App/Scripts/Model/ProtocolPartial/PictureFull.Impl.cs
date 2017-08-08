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
        public PuzzleState CurState;
        public int Quality;
        public PicturePart[] NeededFragments;
        public string Name;
        public string Desc;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="puzzle"></param>
        public PictureFull(Table_Puzzle puzzle)
        {
            _pictureId = puzzle.Id;
            _level = -1;
            _isUsing = false;
            _slot = -1;
            CurState = PuzzleState.CantActive;
            Quality = puzzle.Quality;
            Name = puzzle.Name;
            Desc = puzzle.Description;
            //合成所需碎片
            var FragmentIDs = puzzle.Fragments;
            NeededFragments = new PicturePart[FragmentIDs.Length];
            for (int i = 0; i < FragmentIDs.Length; i++)
            {
                var Fragment = TableManager.Instance.GetPuzzleFragment(FragmentIDs[i]);
                NeededFragments[i] = new PicturePart(Fragment);
            }
        }

        /// <summary>
        /// 激活拼图
        /// </summary>
        public void ActivatePuzzle()
        {
            CurState = PuzzleState.HasActived;
        }

        /// <summary>
        /// 装备拼图
        /// </summary>
        public void EquipPuzzle()
        {
            CurState = PuzzleState.HasEquiped;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public void UpdateState()
        {
            if (CurState == PuzzleState.CantActive && CheckActivatable())
                CurState = PuzzleState.CanActive;
            else if (CurState == PuzzleState.CanActive && !CheckActivatable())
                CurState = PuzzleState.CantActive;

        }

        private bool CheckActivatable()
        {
            for (int i = 0; i < NeededFragments.Length; i++)
            {
                if (!(NeededFragments[i].TotalCount > 0))
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
        HasEquiped//已装备
    }
}
