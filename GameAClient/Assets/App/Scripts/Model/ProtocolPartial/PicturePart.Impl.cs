using System.Collections;
using System;
using UnityEngine;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public partial class PicturePart : SyncronisticData
    {
        public bool Owned;
        public PictureFull ParentPuzzle;
        public int HaveNum;
        public string Name;
        //掉率

        public void Add()
        {
            HaveNum++;
            if (HaveNum == 1)
                ParentPuzzle.UpdateState();
        }

        public void Remove()
        {
            if (HaveNum > 0)
            {
                HaveNum--;
                if (HaveNum == 0)
                    ParentPuzzle.UpdateState();
            }
        }

        /// <summary>
        /// 赠予好友
        /// </summary>
        public void Grant()
        {

        }
    }
}
