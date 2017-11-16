using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlPersonalInfoAchieve : UPCtrlPersonalInfoBase
    {
        private Achievement _data = new Achievement();
        private List<AchievementItem> _dataList;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu - 1].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void OnDestroy()
        {
            _dataList = null;
            base.OnDestroy();
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        private void RequestData()
        {
            if (_mainCtrl.UserInfoDetail == null) return;
            _data.Request(_mainCtrl.UserInfoDetail.UserInfoSimple.UserId, () =>
            {
                _dataList = LocalUser.Instance.Achievement.AchievementList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("成就功能暂未开放。");
            });
        }

        public override void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.EmptyObj.SetActive(true);
                _cachedView.GridDataScrollers[(int) _menu - 1].SetEmpty();
                return;
            }
            _cachedView.EmptyObj.SetActive(_dataList.Count == 0);
            _cachedView.GridDataScrollers[(int) _menu - 1].SetItemCount(_dataList.Count);
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _dataList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_dataList[inx]);
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPersonalInfoAchievement();
            item.Init(parent, _resScenary);
            return item;
        }
    }
}