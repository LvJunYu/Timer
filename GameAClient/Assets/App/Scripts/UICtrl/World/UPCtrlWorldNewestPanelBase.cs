//using System.Collections.Generic;
//using SoyEngine;
//using UnityEngine;
//
//namespace GameA
//{
//    public abstract class UPCtrlWorldNewestPanelBase : UPCtrlBase<UICtrlWorld, UIViewWorld>, IOnChangeHandler<long>
//    {
//        protected EResScenary _resScenary;
//        protected UPCtrlWorldNewest.EMenu _menu;
//        protected bool _unload;
//        protected const int _pageSize = 21;
//        protected List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();
//
//        protected Dictionary<long, CardDataRendererWrapper<Project>> _dict =
//            new Dictionary<long, CardDataRendererWrapper<Project>>();
//
//        protected UMCtrlProject.ECurUI _eCurUi;
//        protected List<Project> _userselfRecommedprojectList;
//
//        protected override void OnViewCreated()
//        {
//            base.OnViewCreated();
//            _eCurUi = GetUMCurUI(_menu);
//            _cachedView.NewestGridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
//        }
//
//        public override void Open()
//        {
//            base.Open();
//            _unload = false;
//            _cachedView.NewestPannels[(int) _menu].SetActiveEx(true);
//            _cachedView.NewestGridDataScrollers[(int) _menu].SetActiveEx(true);
//            RequestData();
//            RefreshView();
//        }
//
//        public override void Close()
//        {
//            _cachedView.NewestGridDataScrollers[(int) _menu].RefreshCurrent();
//            _cachedView.NewestPannels[(int) _menu].SetActiveEx(false);
//            base.Close();
//        }
//
//        public virtual void RequestData(bool append = false)
//        {
//        }
//
//        protected virtual void RefreshView()
//        {
//            _cachedView.EmptyObj.SetActiveEx(_userselfRecommedprojectList == null || _userselfRecommedprojectList.Count == 0);
//            _contentList.Clear();
//            _dict.Clear();
//            if (_userselfRecommedprojectList == null)
//            {
//                _cachedView.NewestGridDataScrollers[(int) _menu].SetEmpty();
//                return;
//            }
//
//            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _userselfRecommedprojectList.Count);
//            for (int i = 0; i < _userselfRecommedprojectList.Count; i++)
//            {
//                if (!_dict.ContainsKey(_userselfRecommedprojectList[i].ProjectId))
//                {
//                    CardDataRendererWrapper<Project> w =
//                        new CardDataRendererWrapper<Project>(_userselfRecommedprojectList[i], OnItemClick);
//                    _contentList.Add(w);
//                    _dict.Add(_userselfRecommedprojectList[i].ProjectId, w);
//                }
//            }
//
//            _cachedView.NewestGridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
//        }
//
//        protected void OnItemClick(CardDataRendererWrapper<Project> item)
//        {
//            if (item == null || item.Content == null)
//            {
//                return;
//            }
//
//            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content);
//        }
//
//        protected IDataItemRenderer GetItemRenderer(RectTransform parent)
//        {
//            var item = new UMCtrlProject();
//            item.SetCurUI(_eCurUi);
//            item.Init(parent, _resScenary);
//            return item;
//        }
//
//        protected virtual void OnItemRefresh(IDataItemRenderer item, int inx)
//        {
//            if (_unload)
//            {
//                item.Set(null);
//            }
//            else
//            {
//                if (inx >= _contentList.Count)
//                {
//                    LogHelper.Error("OnItemRefresh Error Inx > count");
//                    return;
//                }
//
//                item.Set(_contentList[inx]);
//            }
//        }
//
//        public virtual void OnChangeHandler(long val)
//        {
//            CardDataRendererWrapper<Project> w;
//            if (_dict.TryGetValue(val, out w))
//            {
//                w.BroadcastDataChanged();
//            }
//        }
//
//        public void Set(EResScenary resScenary)
//        {
//            _resScenary = resScenary;
//        }
//
//        public void SetMenu(UPCtrlWorldNewest.EMenu menu)
//        {
//            _menu = menu;
//        }
//
//        public virtual void Clear()
//        {
//            _unload = true;
//            _contentList.Clear();
//            _dict.Clear();
//            _userselfRecommedprojectList = null;
//            _cachedView.NewestGridDataScrollers[(int) _menu].ContentPosition = Vector2.zero;
//        }
//
//        private UMCtrlProject.ECurUI GetUMCurUI(UPCtrlWorldNewest.EMenu menu)
//        {
//            switch (menu)
//            {
//                case UPCtrlWorldNewest.EMenu.All:
//                    return UMCtrlProject.ECurUI.AllNewestProject;
//                case UPCtrlWorldNewest.EMenu.Follow:
//                    return UMCtrlProject.ECurUI.Follows;
//            }
//
//            return UMCtrlProject.ECurUI.None;
//        }
//    }
//}