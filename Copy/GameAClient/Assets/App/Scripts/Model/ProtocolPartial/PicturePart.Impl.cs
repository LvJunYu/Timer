using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public partial class PicturePart
    {
        private static Dictionary<int, PicturePart> _picPartsDic = new Dictionary<int, PicturePart>(); //所有拼图碎片字典
        private static bool _hasBuilded;
        private int _id;
        private bool _hasInited;

        public void AddFrag(int num)
        {
            TotalCount += num;
            Messenger.Broadcast(EMessengerType.OnPuzzleFragChanged);
        }

        public PicturePart(int pictureId, int index)
        {
            PictureId = pictureId;
            PictureInx = index;
            TotalCount = 0;
            InitData();
        }

        private void InitData()
        {
            if (_hasInited)
                return;
            _id = GetFragId((int) _pictureId, _pictureInx);
            _hasInited = true;
        }

        private static void BuildDic()
        {
            if (_hasBuilded) return;
//            LocalUser.Instance.UserPicturePart.Request(LocalUser.Instance.UserGuid, null,
//                code => { LogHelper.Error("Network error when get UserPicturePart, {0}", code); });
            var fragments = LocalUser.Instance.UserPicturePart.ItemDataList;
            for (int i = 0; i < fragments.Count; i++)
            {
                fragments[i].InitData();
                _picPartsDic.Add(fragments[i]._id, fragments[i]);
            }
            _hasBuilded = true;
        }

        public static PicturePart GetPicturePart(int picId, int index)
        {
            BuildDic();
            int fragId = GetFragId(picId, index);
            //查看字典
            if (_picPartsDic.ContainsKey(fragId))
            {
                //_picPartsDic[id].InitData();
                return _picPartsDic[fragId];
            }
            //没有则创建新的
            PicturePart fragment = new PicturePart(picId, index);
            _picPartsDic.Add(fragId, fragment);
            return fragment;
        }

        public static void AddPicturePart(int picId, int index, int num)
        {
            PicturePart picPart = GetPicturePart(picId, index);
            picPart.TotalCount += num;
        }

        public static int GetFragId(int picId, int index)
        {
            return picId * 100 + index;
        }
    }
}