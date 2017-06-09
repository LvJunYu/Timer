/********************************************************************
** Filename : UPCtrlWorldNewestProject.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlWorldNewestProject.cs
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
    public class UPCtrlWorldNewestProject : UPCtrlBase<UICtrlWorld, UIViewWorld>
    {
        #region 常量与字段
        private const int PageSize = 10;
        private List<CardDataRendererWrapper<Project>> _content = new List<CardDataRendererWrapper<Project>>();
        private WorldNewestProjectList _data;
        private bool _isOpen = false;
        private Vector2 _pagePosition = Vector2.zero;
        private CardDataRendererWrapper<Project> _curSelectedProject;
        #endregion

        #region 属性

        #endregion

        #region 方法
        public void Open()
        {
            _isOpen = true;
            _data = AppData.Instance.WorldData.NewestProjectList;
            RefreshView();
            RequestData();
            _cachedView.GridScroller.ContentPosition = _pagePosition;
        }

        public void Close()
        {
            _isOpen = false;
            _pagePosition = _cachedView.GridScroller.ContentPosition;
        }

        public void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if (_curSelectedProject != null)
            {
                _curSelectedProject.IsSelected = false;
                _curSelectedProject.BroadcastDataChanged();
            }
            _curSelectedProject = item;
            if (_curSelectedProject != null)
            {
                _curSelectedProject.IsSelected = true;
                _curSelectedProject.BroadcastDataChanged();
            }
        }

        public void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if(inx >= _content.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_content[inx]);
            if (!_data.IsEnd)
            {
                if(inx > _content.Count - 2)
                {
                    RequestData(true);
                }
            }
        }
        #region private
        private void RequestData(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = _content.Count;
            }
            _data.Request(startInx, PageSize, ()=>{
                if (!_isOpen) {
                    return;
                }
                RefreshView();
            }, code=>{
            });
        }

        private void RefreshView()
        {
            List<Project> list = _data.AllList;
            _content.Clear();
            _content.Capacity = Mathf.Max(_content.Capacity, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                if (_curSelectedProject != null)
                {
                    
                }
            }
            _cachedView.GridScroller.SetItemCount(_content.Count);
        }
        #endregion private

        #region 接口

        #endregion 接口

        #endregion

    }
}
