using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopProjectPublished : UPCtrlWorkShopProjectBase
    {
        private UserPublishedWorldProjectList _data;

        public override void RequestData(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = LocalUser.Instance.UserPublishedWorldProjectList.AllList.Count;
            }
            _data = LocalUser.Instance.UserPublishedWorldProjectList;
            _data.Requset(startInx, _pageSize, () =>
            {
                _projectList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, null);
        }

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (!_isOpen)
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