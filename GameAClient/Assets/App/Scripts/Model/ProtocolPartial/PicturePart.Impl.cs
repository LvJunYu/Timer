using System.Collections;
using System;
using UnityEngine;
using GameA.Game;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public partial class PicturePart : SyncronisticData
    {
        public string Name;
       
        public PicturePart(Table_PuzzleFragment fragement)
        {
            _pictureId = fragement.Id;
            _pictureInx = 0;
            _totalCount = 0;
            Name = fragement.Name;
        }

        //public 

    }
}
