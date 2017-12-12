using SoyEngine;

namespace GameA
{
    public class UPCtrlWorldFollowedUserProject : UPCtrlWorldNewestPanelBase
    {
        private WorldFollowedUserProjectList _data;

        public override void RequestData(bool append = false)
        {
            _data = AppData.Instance.WorldData.WorldFollowedUserProjectList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(startInx, _pageSize, () =>
            {
                _hasRequested = true;
                _projectList = _data.AllList;
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