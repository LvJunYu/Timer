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

        private int _id;
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
            _hasInited = true;
        }

        /// <summary>
        /// 同步本地数据
        /// </summary>
        private void Request()
        {
            PicturePart fragment = null;
            LocalUser.Instance.UserPicturePart.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UserPicturePart, {0}", code); });

            if (_sync) return;
            fragment = LocalUser.Instance.UserPicturePart.ItemDataList.Find(p => p._pictureId == this._pictureId && p._pictureInx == this._pictureInx);
            if (fragment != null)
            {
                fragment.InitData();
                fragment._sync = true;
                if (_picPartsDic.ContainsKey(_id))
                    _picPartsDic[_id] = fragment;
                else
                    _picPartsDic.Add(_id, fragment);
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
            int id = GetFragId(picId, index);
            PicturePart picPart = null;
            //修改本地数据
            if (_picPartsDic.ContainsKey(id))
            {
                picPart = _picPartsDic[id];
                picPart.TotalCount += num;
            }
            else
            {
                picPart = new PicturePart(picId, index);
                picPart.TotalCount += num;
                _picPartsDic.Add(id, picPart);
            }
            //请求同步服务器数据
            picPart.Request();
        }
    }
}
