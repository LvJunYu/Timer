using SoyEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public class UPCtrlWorldSelfRecommendProject : UPCtrlWorldProjectBase
    {
        private WorldSelfRecommendProjectList _data;

        public override void RequestData(bool append = false)
        {
            base.RequestData(append);
            _data = AppData.Instance.WorldData.WorldSelfRecommendProjectList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }

            _isRequesting = true;
            _data.Request(startInx, _pageSize, Mask, () =>
            {
                _projectList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }

                _isRequesting = false;
            }, code =>
            {
                _isRequesting = false;
                LogHelper.Error("WorldNewestProjectList Request fail, code = {0}", code);
            });
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

        protected override void OnEndDragEvent(PointerEventData arg0)
        {
        }
    }
}