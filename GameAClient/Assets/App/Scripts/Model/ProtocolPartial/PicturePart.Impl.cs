using System.Collections;
using System;
using UnityEngine;
using GameA.Game;
using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public partial class PicturePart : SyncronisticData
    {
        private static Dictionary<int, PicturePart> _picPartsDic = new Dictionary<int, PicturePart>();//所有拼图碎片字典

        private long _id;
        private bool _hasInited;
        private bool _sync;//当前对象是否是服务器同步的对象

        public void AddFrag(int num)
        {
            TotalCount += num;
            Messenger.Broadcast(EMessengerType.OnPuzzleFragChanged);
        }

        public PicturePart(int pictureId, int index)
        {
            _pictureId = pictureId;
            _pictureInx = index;
            _totalCount = 0;
            _sync = false;
            InitData();
        }

        private void InitData()
        {
            if (_hasInited)
                return;
            _id = GetFragId((int)_pictureId, _pictureInx);
            //_fragmentTable = TableManager.Instance.GetPuzzleFragment((int)_pictureId);
            //_name = _fragmentTable.Name;
            _hasInited = true;
        }

        /// <summary>
        /// 同步本地引用到服务器数据引用
        /// </summary>
        private void Sync()
        {
            PicturePart fragment = null;
            fragment = LocalUser.Instance.UserPicturePart.ItemDataList.Find(p => p._pictureId == this._pictureId && p._pictureInx == this._pictureInx);
            if (fragment != null)
            {
                fragment._sync = true;
                _picPartsDic[GetFragId((int)_pictureId, _pictureInx)] = fragment;
            }
        }

        public static PicturePart GetPicturePart(int picId, int index)
        {
            int fragId = GetFragId(picId, index);
            //查看字典
            if (_picPartsDic.ContainsKey(fragId))
            {
                //_picPartsDic[id].InitData();
                return _picPartsDic[fragId];
            }
            //查看是否已拥有碎片
            PicturePart fragment = null;
            fragment = LocalUser.Instance.UserPicturePart.ItemDataList.Find(p => p._pictureId == picId && p._pictureInx == index);
            if (fragment != null)
            {
                fragment.InitData();
                fragment._sync = true;
            }
            else
            {
                //没有则创建新的
                fragment = new PicturePart(picId, index);
            }
            _picPartsDic.Add(fragId, fragment);
            return fragment;
        }

        public static int GetFragId(int picId, int index)
        {
            return picId * 100 + index;
        }

        public static void AddPicturePart(int picId, int index, int num)
        {
            PicturePart fragment = GetPicturePart(picId, index);
            if (!fragment._sync)
                fragment.Sync();
            if (!fragment._sync)
                fragment.TotalCount += num;
        }
    }
}
