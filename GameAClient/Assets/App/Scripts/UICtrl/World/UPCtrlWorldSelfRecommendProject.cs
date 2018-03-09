﻿using SoyEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public class UPCtrlWorldSelfRecommendProject : UPCtrlWorldProjectBase
    {
        private WorldSelfRecommendProjectList _data;

        public override void RequestData(bool append = false)
        {
            _data = AppData.Instance.WorldData.WorldSelfRecommendProjectList;
            if (_isRequesting)
            {
                return;
            }

            _isRequesting = true;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }

            _data.Request(startInx, _pageSize, () =>
            {
                _isRequesting = false;
                _projectList = _data.ProjectList;
                if (_isOpen)
                {
                    RefreshView();
                }
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