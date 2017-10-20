using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlPersonalInfoDetailCollect : UPCtrlPersonalInfoDetailBase
    {
        private UserFavoriteWorldProjectList _data;

        protected override void RequestData(bool append = false)
        {
            _data = AppData.Instance.WorldData.UserFavoriteProjectList;
            _pojectList = _data.AllList;
            if (_mainCtrl.UserInfoDetail == null) return;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(_mainCtrl.UserInfoDetail.UserInfoSimple.UserId, startInx, PageSize,
                EFavoriteProjectOrderBy.FPOB_FavoriteTime, EOrderType.OT_Desc, () =>
                {
                    if (_isOpen)
                    {
                        RefreshView();
                    }
                }, code => { });
        }
        
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
    }
}