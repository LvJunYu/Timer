using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopOfficialProjectPublished : UPCtrlWorkShopOfficialProjectBase
    {
        private GmMasterProjectList _data = OfficalData.Instance.GmProjectList;

        public override void RequestData(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = OfficalData.Instance.GmProjectList.AllList.Count;
            }

            _data.Request(startInx, _pageSize, Project.ProjectTypeAllMask, () =>
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

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProject();
            item.SetCurUI(UMCtrlProject.ECurUI.Published);
            item.Init(parent, _resScenary);
            return item;
        }
    }
}