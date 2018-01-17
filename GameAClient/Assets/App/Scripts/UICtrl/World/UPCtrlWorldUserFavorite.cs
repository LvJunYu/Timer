using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlWorldUserFavorite : UPCtrlWorldProjectBase
    {
        private UserFavoriteWorldProjectList _data = AppData.Instance.WorldData.UserFavoriteProjectList;

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
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }

            _data.Request(LocalUser.Instance.Account.UserGuid, startInx, _pageSize,
                EFavoriteProjectOrderBy.FPOB_FavoriteTime, EOrderType.OT_Desc, () =>
                {
                    _projectList = _data.AllList;
                    if (_isOpen)
                    {
                        RefreshView();
                    }
                }, code => { });
        }

        public void OnProjectMyFavoriteChanged(Project project, bool favorite)
        {
            if (_isOpen)
            {
                if (!favorite && _data.AllList.Contains(project))
                {
                    _data.AllList.Remove(project);
                }
                if (favorite && !_data.AllList.Contains(project))
                {
                    _data.AllList.Add(project);
                }
                _projectList = _data.AllList;
                RefreshView();
            }
        }
    }
}