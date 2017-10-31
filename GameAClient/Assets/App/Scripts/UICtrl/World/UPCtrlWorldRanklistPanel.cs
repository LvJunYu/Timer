using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldRanklistPanel : UPCtrlWorldPanelBase
    {
        private WorldRankList _data;
        protected const int _pageSize = 20;
        protected List<CardDataRendererWrapper<WorldRankItem.WorldRankHolder>> _contentList = 
            new List<CardDataRendererWrapper<WorldRankItem.WorldRankHolder>>();
        protected List<WorldRankItem.WorldRankHolder> _projectList;
        private EWorldRankType _type;
        private ERankTimeBucket _bucket;
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
            InitBtn();
        }
     
        protected override void RefreshView()
        {
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
            if (_projectList == null)
            {
                _contentList.Clear();
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _projectList.Count);
            for (int i = 0; i < _projectList.Count; i++)
            {
                CardDataRendererWrapper<WorldRankItem.WorldRankHolder> w = new CardDataRendererWrapper<WorldRankItem.WorldRankHolder>(_projectList[i], null);
                _contentList.Add(w);
            }
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
        }
        
        protected IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldRank();
            item.Init(parent, _resScenary);
            return item;
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

        protected override void RequestData(bool append = false)
        {
            _data = AppData.Instance.WorldData.RankList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _data.Request(_type,_bucket ,startInx, _pageSize,
               () =>
                {
                    Debug.Log("rquestsucess");
                    _projectList = _data.CurList;
                    if (_isOpen)
                    {
                        RefreshView();
                    }
                }, code => { Debug.Log("requestdatafails"); });
        }

        public override void OnChangeHandler(long val)
        {
//            CardDataRendererWrapper<Project> w;
//            RequestData();
        }
        public override void Clear()
        {
            _unload = true;
            _contentList.Clear();
            _projectList = null;
        }

        private void InitBtn()
        {
            int BtnNum = _cachedView.RankListBtnAry.Length-1 ;
            _type = EWorldRankType.WRT_Player;
            _bucket = ERankTimeBucket.RTB_Total;
            for (int i = 0; i <  _cachedView.RankListBtnAry.Length; i++)
            {
                _cachedView.RankListBtnAry[i].SetActiveEx(true);
            }
            for (int i = 0; i < _cachedView.RankListBtnSelectAry.Length; i++)
            {
                _cachedView.RankListBtnSelectAry[i].SetActiveEx(false);  
            }
            _cachedView.RankListBtnAry[ BtnNum-(int)_bucket].SetActiveEx(false);
            _cachedView.RankListBtnSelectAry[ BtnNum- (int)_bucket].SetActiveEx(true);
            for (int i = 0; i < _cachedView.RankListBtnAry.Length; i++)
            {
                var i1 = i;
                _cachedView.RankListBtnAry[i].onClick.AddListener(() =>
                {
                    _cachedView.RankListBtnAry[i1].gameObject.SetActive(false);
                    _cachedView.RankListBtnAry[BtnNum-(int)_bucket].gameObject.SetActive(true);
                    _cachedView.RankListBtnSelectAry[BtnNum-(int)_bucket].gameObject.SetActive(false);
                    _cachedView.RankListBtnSelectAry[i1].gameObject.SetActive(true);
                    _bucket = (ERankTimeBucket) (BtnNum-i1);
                    RequestData();
                    RefreshView();
                });
            }
            _cachedView.AdventureBtn.onClick.AddListener(() =>
            {
                _type = EWorldRankType.WRT_Player;
                RequestData();
                RefreshView();
            });
            _cachedView.CraftsmanBtn.onClick.AddListener(() =>
                {
                    _type = EWorldRankType.WRT_Creator;
                    RequestData();
                    RefreshView();
                }
            );
        }
    }
}