﻿using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlAdvLvlDetailRank : USCtrlBase<USViewAdvLvlDetailRank>
    {
        private EResScenary _resScenary;
        private List<Record> _recordList;

        private List<CardDataRendererWrapper<RecordRankHolder>> _contentList =
            new List<CardDataRendererWrapper<RecordRankHolder>>();

        public void Set(List<Record> recordList, EResScenary resScenary)
        {
            _resScenary = resScenary;
            _recordList = recordList;
            RefreshView();
        }

        public void Open()
        {
            _cachedView.gameObject.SetActive(true);
            RefreshView();
        }

        public void Close()
        {
            _cachedView.gameObject.SetActive(false);
        }

        private void RefreshView()
        {
            if (_recordList == null)
            {
                _cachedView.GridDataScroller.SetEmpty();
                return;
            }
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _recordList.Count);
            for (int i = 0; i < _recordList.Count; i++)
            {
                RecordRankHolder r = new RecordRankHolder(_recordList[i], i);
                CardDataRendererWrapper<RecordRankHolder> w =
                    new CardDataRendererWrapper<RecordRankHolder>(r, OnItemClick);
                _contentList.Add(w);
            }
            _cachedView.GridDataScroller.SetItemCount(_contentList.Count);
        }

        private void OnItemClick(CardDataRendererWrapper<RecordRankHolder> item)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求播放录像");
            item.Content.Record.RequestPlay(() =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                UICtrlAdvLvlDetail uictrlAdvLvlDetail = SocialGUIManager.Instance.GetUI<UICtrlAdvLvlDetail>();
                SituationAdventureParam param = new SituationAdventureParam();
                param.ProjectType = uictrlAdvLvlDetail.ProjectType;
                param.Section = uictrlAdvLvlDetail.ChapterIdx;
                param.Level = uictrlAdvLvlDetail.LevelIdx;
                param.Record = item.Content.Record;
                GameManager.Instance.RequestPlayAdvRecord(uictrlAdvLvlDetail.Project, param);
                SocialApp.Instance.ChangeToGame();
            }, (error) =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.ShowPopupDialog("进入录像失败");
            });
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _contentList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_contentList[inx]);
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlRank();
            item.Init(parent, _resScenary);
            return item;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }
    }
}