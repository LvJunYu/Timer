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
    public class UPCtrlWorldNewestProject : UPCtrlBase<UICtrlWorld, UIViewWorld>, IOnChangeHandler<long>
    {
        #region 常量与字段
        private const int PageSize = 10;
        private List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();
        private Dictionary<long, CardDataRendererWrapper<Project>> _dict = new Dictionary<long, CardDataRendererWrapper<Project>>();
        private WorldNewestProjectList _data;
        private Vector2 _pagePosition = Vector2.zero;
        private CardDataRendererWrapper<Project> _curSelectedProject;
        #endregion

        #region 属性

        #endregion

        #region 方法
        public override void Open()
        {
            base.Open();
            _data = AppData.Instance.WorldData.NewestProjectList;
            RefreshView();
            RequestData();
            _cachedView.GridScroller.ContentPosition = _pagePosition;
        }

        public override void Close()
        {
            _pagePosition = _cachedView.GridScroller.ContentPosition;
            base.Close();
        }

        private void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            SelectItem(item);
        }

        public void OnItemRefresh(IDataItemRenderer item, int inx)
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
        #region private
        private void RequestData(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
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
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, list.Count);
            _dict.Clear();
            bool findFlag = false;
            for (int i = 0; i < list.Count; i++)
            {
                Project p = list[i];
                CardDataRendererWrapper<Project> w = new CardDataRendererWrapper<Project>(p, OnItemClick);
                if (_curSelectedProject != null
                    && _curSelectedProject.Content.ProjectId == p.ProjectId)
                {
                    SelectItem(w);
                    findFlag = true;
                }
                w.IsSelected = false;
                _contentList.Add(w);
                _dict.Add(p.ProjectId, w);
            }
            if (!findFlag)
            {
                if (_contentList.Count > 0)
                {
                    SelectItem(_contentList[0]);
                }
                else
                {
                    SelectItem(null);
                }
            }
            _cachedView.GridScroller.SetItemCount(_contentList.Count);
        }

        private void SelectItem(CardDataRendererWrapper<Project> item)
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
            _mainCtrl.SetProject(item == null ? null : item.Content);
        }
        #endregion private

        #region 接口
        public void OnChangeHandler(long val)
        {
            CardDataRendererWrapper<Project> w;
            if (_dict.TryGetValue(val, out w))
            {
                w.BroadcastDataChanged();
            }
        }
        #endregion 接口

        #endregion

    }
}
