﻿using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameA
{
    /// <summary>
    /// 拼图
    /// </summary>
    public partial class PuzzleData
    {
        public PuzzleState CurState;
        public int Quality;
        public List<PuzzleFragmentData> PuzzleFragments;
        //拼图的属性

        public void Init()
        {
            CurState = PuzzleState.CantActive;
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
            for (int i = 0; i < PuzzleFragments.Count; i++)
            {
                if (!PuzzleFragments[i].Owned)
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
