/********************************************************************
** Filename : UPCtrlWorldProjectRecentRecord.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlWorldProjectRecentRecord.cs
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace GameA
{
    public class UPCtrlWorldProjectRecentRecord : UPCtrlBase<UICtrlWorld, UIViewWorld>
    {
        #region 常量与字段
        private const int PageSize = 10;
        private Project _content;
        private List<CardDataRendererWrapper<Record>> _contentList = new List<CardDataRendererWrapper<Record>>();
        private WorldProjectRecentRecordList _data;
        #endregion

        #region 属性

        #endregion

        #region 方法
        public void SetData(Project project)
        {
            _content = project;
            if (_content == null)
            {
                _data = null;
            }
            else
            {
                _data = _content.ProjectRecentRecordList;
            }
        }

        public override void Open()
        {
            base.Open();
            _cachedView.RecentRecordDock.SetActive(true);
            RefreshView();
            RequestData();
        }

        public override void Close()
        {
            _cachedView.RecentRecordDock.SetActive(false);
            base.Close();
        }
        #region private
        #endregion private

        private void RefreshView()
        {
            if (_content == null)
            {
                _cachedView.RecentRecordGridScroller.SetEmpty();
                return;
            }
            List<Record> list = _data.AllList;
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Record r = list[i];
                CardDataRendererWrapper<Record> w = new CardDataRendererWrapper<Record>(r, OnItemClick);
                _contentList.Add(w);
            }
            _cachedView.RecentRecordGridScroller.SetItemCount(_contentList.Count);
        }


        private void RequestData(bool append = false)
        {
            if (_content == null)
            {
                return;
            }
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            Project requestP = _content;
            _content.RequestRecentRecordList(startInx, PageSize, ()=>{
                if (!_isOpen) {
                    return;
                }
                if (_content != null && _content.ProjectId == requestP.ProjectId) {
                    RefreshView();
                }
            }, code=>{
            });
        }

        private void OnItemClick(CardDataRendererWrapper<Record> item)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "请求播放录像");
            item.Content.RequestPlay (() => {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                GameManager.Instance.RequestPlayRecord (_content, item.Content);
                SocialGUIManager.Instance.ChangeToGameMode ();
            }, (error) => {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                SocialGUIManager.ShowPopupDialog("进入录像失败");
            });
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if(inx >= _contentList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_contentList[inx]);
            if (!_data.IsEnd)
            {
                if(inx > _contentList.Count - 2)
                {
                    RequestData(true);
                }
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldRecentRecord();
            item.Init(parent, Vector3.zero);
            return item;
        }
        #region 接口
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.RecentRecordGridScroller.SetCallback(OnItemRefresh, GetItemRenderer);
        }

        #endregion 接口

        #endregion

    }
}
