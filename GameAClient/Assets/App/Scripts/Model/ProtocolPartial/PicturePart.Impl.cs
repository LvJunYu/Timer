using System.Collections;
using System;
using UnityEngine;
using GameA.Game;
using System.Collections.Generic;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public partial class PicturePart : SyncronisticData
    {
        private string _name;
        private PictureFull _parent;
        private Table_PuzzleFragment _fragmentTable;
        //private bool _inited;

        public PicturePart(int fragmentID, PictureFull parent)
        {
            _pictureId = fragmentID;
            _totalCount = 0;
            InitData(parent);
        }

        public void InitData(PictureFull parent)
        {
            //if (_inited)
            //    return;
            _fragmentTable = TableManager.Instance.GetPuzzleFragment((int)_pictureId);
            _name = _fragmentTable.Name;
            _parent = parent;
            _pictureInx = Array.IndexOf(_parent.FragmentIDs, (int)_pictureId) + 1;
            //_inited = true;
        }
    }
}
