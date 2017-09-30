using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldNewestProject : UPCtrlBase<UICtrlWorld, UIViewWorld>, IOnChangeHandler<long>
    {
        #region 常量与字段
        private const int PageSize = 10;
        private List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();
        private Dictionary<long, CardDataRendererWrapper<Project>> _dict = new Dictionary<long, CardDataRendererWrapper<Project>>();
        private WorldNewestProjectList _data;
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
            _cachedView.NewestPanel.SetActiveEx(true);
            _data = AppData.Instance.WorldData.NewestProjectList;
            RefreshView();
            RequestData();
        }

        public override void Close()
        {
            _unload = true;
            _cachedView.NewestGridScroller.RefreshCurrent();
            _cachedView.NewestPanel.SetActiveEx(false);
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
            return DateTimeUtil.GetServerSmartDateStringByTimestampMillis(p.CreateTime);
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
            for (int i = 0; i < list.Count; i++)
            {
                Project p = list[i];
                CardDataRendererWrapper<Project> w = new CardDataRendererWrapper<Project>(p, OnItemClick);
                _contentList.Add(w);
                _dict.Add(p.ProjectId, w);
            }
            _cachedView.NewestGridScroller.SetItemCount(_contentList.Count);
        }

        #endregion private

        #region 接口

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.NewestGridScroller.Set(OnItemRefresh, GetItemRenderer);
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
