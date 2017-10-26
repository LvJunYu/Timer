using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlAchievement : UICtrlAnimationBase<UIViewAchievement>
    {
        private List<AchiveItemData> _dataList;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.GridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshData();
            RefreshView();
        }

        private void RefreshData()
        {
            LocalUser.Instance.Achievement.Request(LocalUser.Instance.UserGuid, () =>
            {
                _dataList = LocalUser.Instance.Achievement.AllAchiveData;
                RefreshView();
            }, code =>
            {
                //Todo 测试数据
                LocalUser.Instance.Achievement.BuildData();
                _dataList = LocalUser.Instance.Achievement.AllAchiveData;
                RefreshView();
//                SocialGUIManager.ShowPopupDialog("请求成就数据失败。");
            });
        }

        private void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.GridDataScroller.SetEmpty();
            }
            else
            {
                _cachedView.GridDataScroller.SetItemCount(_dataList.Count);
            }
        }

        protected void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _dataList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_dataList[inx]);
        }

        protected virtual IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlAchievementItem();
            item.Init(parent, ResScenary);
            return item;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlAchievement>();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnAddAchievementCount, OnAddAchievementCount);
        }

        private void OnAddAchievementCount()
        {
            if (_isOpen)
            {
                RefreshView();
            }
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainPopUpUI;
        }

        protected override void OnOpenAnimationUpdate()
        {
            base.OnOpenAnimationUpdate();
            _cachedView.Scrollbar.value = 1;
        }
    }
}