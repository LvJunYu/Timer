using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlWorldUserFavorite : UPCtrlWorldProjectBase
    {
        private UserFavoriteWorldProjectList _data;

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (_unload)
            {
                item.Set(null);
            }
            else
            {
                if (inx >= _contentList.Count)
                {
                    LogHelper.Error("OnItemRefresh Error Inx > count");
                    return;
                }
                item.Set(_contentList[inx]);
                if (!_data.IsEnd)
                {
                    if (inx > _contentList.Count - 2)
                    {
                        RequestData(true);
                    }
                }
            }
        }

        public override void RequestData(bool append = false)
        {
            _data = AppData.Instance.WorldData.UserFavoriteProjectList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(LocalUser.Instance.Account.UserGuid, startInx, _pageSize,
                EFavoriteProjectOrderBy.FPOB_FavoriteTime, EOrderType.OT_Desc, () =>
                {
                    _hasRequested = true;
                    _projectList = _data.AllList;
                    if (_isOpen)
                    {
                        RefreshView();
                    }
                }, code => { });
        }
    }
}