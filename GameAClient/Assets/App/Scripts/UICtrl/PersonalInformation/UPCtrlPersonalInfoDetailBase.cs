using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlPersonalInfoDetailBase : UPCtrlPersonalInfoBase, IOnChangeHandler<long>
    {
        private const int PageSize = 10;
        private List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();

        private Dictionary<long, CardDataRendererWrapper<Project>> _dict =
            new Dictionary<long, CardDataRendererWrapper<Project>>();

        private UserFavoriteWorldProjectList _data;
        private bool _unload;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu - 1].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _unload = false;
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _unload = true;
            _cachedView.GridDataScrollers[(int) _menu - 1].RefreshCurrent();
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

        private void OnItemRefresh(IDataItemRenderer item, int inx)
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

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPersonalInfoProject();
            item.Init(parent, _resScenary);
            return item;
        }

        private void RequestData(bool append = false)
        {
            _data = AppData.Instance.WorldData.UserFavoriteProjectList;
            if (_mainCtrl.UserInfoDetail == null) return;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(_mainCtrl.UserInfoDetail.UserInfoSimple.UserId, startInx, PageSize,
                EFavoriteProjectOrderBy.FPOB_FavoriteTime, EOrderType.OT_Desc, () =>
                {
                    if (!_isOpen)
                    {
                        return;
                    }
                    RefreshView();
                }, code => { });
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
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
        }

        public void OnChangeHandler(long val)
        {
            CardDataRendererWrapper<Project> w;
            if (_dict.TryGetValue(val, out w))
            {
                w.BroadcastDataChanged();
            }
        }

        public override void OnDestroy()
        {
            _contentList = null;
            _dict = null;
            base.OnDestroy();
        }
    }
}