/********************************************************************
** Filename : UPCtrlWorldUserFavorite.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlWorldUserFavorite.cs
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldUserFavorite : UPCtrlBase<UICtrlWorld, UIViewWorld>, IOnChangeHandler<long>
    {
        #region 常量与字段
        private const int PageSize = 10;
        private List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();
        private Dictionary<long, CardDataRendererWrapper<Project>> _dict = new Dictionary<long, CardDataRendererWrapper<Project>>();
        private UserFavoriteWorldProjectList _data;
        private EResScenary _resScenary;
        private bool _unload;
        #endregion

        #region 属性

        #endregion

        #region 方法
        public void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public override void Open()
        {
            base.Open();
            _unload = false;
            _cachedView.FavoritePanel.SetActiveEx(true);
            _data = AppData.Instance.WorldData.UserFavoriteProjectList;
            RefreshView();
            RequestData();
        }

        public override void Close()
        {
            _unload = true;
            _cachedView.FavoriteGridScroller.RefreshCurrent();
            _cachedView.FavoritePanel.SetActiveEx(false);
            base.Close();
        }

        private void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content);
        }
        
        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldProject();
            item.Init(parent, _resScenary);
            item.GetTimeFunc = GetTime;
            return item;
        }

        public void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (_unload)
            {
                item.Set(null);
            }
            else
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
        }
        
        private string GetTime(Project p)
        {
            return DateTimeUtil.GetServerSmartDateStringByTimestampMillis(p.ProjectUserData.LastFavoriteTime);
        }
        #region private
        private void RequestData(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(LocalUser.Instance.Account.UserGuid, startInx, PageSize,
                EFavoriteProjectOrderBy.FPOB_FavoriteTime, EOrderType.OT_Desc, ()=>{
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
            for (int i = 0; i < list.Count; i++)
            {
                Project p = list[i];
                CardDataRendererWrapper<Project> w = new CardDataRendererWrapper<Project>(p, OnItemClick);
                _contentList.Add(w);
                _dict.Add(p.ProjectId, w);
            }
            _cachedView.FavoriteGridScroller.SetItemCount(_contentList.Count);
        }

        #endregion private

        #region 接口
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.FavoriteGridScroller.Set(OnItemRefresh, GetItemRenderer);
        }
        
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
