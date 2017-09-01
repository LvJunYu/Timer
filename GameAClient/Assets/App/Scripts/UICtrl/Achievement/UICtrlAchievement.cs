using SoyEngine;
using System.Collections.Generic;
using GameA.Game;

namespace GameA
{
    /// <summary>
    /// 成就页面
    /// </summary>
    [UIAutoSetup]
    public class UICtrlAchievement : UICtrlAnimationBase<UIViewAchievement>
    {
        private const int _maxAchievementNum = 40;
        private List<UMCtrlAchievementItem> _unFinishUMs;
        private List<UMCtrlAchievementItem> _finishUMs;
        private List<AchievementItem> _achievementItems;
        private List<UMCtrlAchievementItem> _umCtrlAchievementItemCache;

        private void InitView()
        {
            _achievementItems = new List<AchievementItem>(_maxAchievementNum);
            var achievements = TableManager.Instance.Table_AchievementDic;
            int achievementType;
            foreach (Table_Achievement value in achievements.Values)
            {
                AchievementItem achievementItem = _achievementItems.Find(p => p.Type == value.Type);
                if (null == achievementItem)
                {
                    achievementItem = new AchievementItem(value.Type, 1);
                    _achievementItems.Add(achievementItem);
                }
                achievementItem.AddLvDic(value.Level, value);
            }
        }

        private void RefreshView()
        {
            if(null == _unFinishUMs)
                _unFinishUMs = new List<UMCtrlAchievementItem>(_maxAchievementNum);
            if(null == _finishUMs)
                _finishUMs = new List<UMCtrlAchievementItem>(_maxAchievementNum);
            _unFinishUMs.Clear();
            _finishUMs.Clear();
            //生成未完成成就
            for (int i = 0; i < _achievementItems.Count; i++)
            {
                if (_achievementItems[i].NextLevel != null)
                {
                    UMCtrlAchievementItem umCtrlAchievementItem = createAchievementItem();
                    umCtrlAchievementItem.SetDate(_achievementItems[i],false);
                    _unFinishUMs.Add(umCtrlAchievementItem);
                }
            }
            //生成已完成成就
            for (int i = 0; i < _achievementItems.Count; i++)
            {
                if (_achievementItems[i].FinishLevel != 0)
                {
                    UMCtrlAchievementItem umCtrlAchievementItem = createAchievementItem();
                    umCtrlAchievementItem.SetDate(_achievementItems[i],true);
                    _finishUMs.Add(umCtrlAchievementItem);
                }
            }
        }

        private UMCtrlAchievementItem createAchievementItem()
        {
            if (null == _umCtrlAchievementItemCache)
                _umCtrlAchievementItemCache = new List<UMCtrlAchievementItem>(_maxAchievementNum * 2);
            var umCtrlAchievementItem = _umCtrlAchievementItemCache.Find(p => !p.IsShow);
            if (umCtrlAchievementItem != null)
            {
                umCtrlAchievementItem.Show();
                return umCtrlAchievementItem;
            }
            umCtrlAchievementItem = new UMCtrlAchievementItem();
            umCtrlAchievementItem.Init(_cachedView.UMItemRTF);
            _umCtrlAchievementItemCache.Add(umCtrlAchievementItem);
            return umCtrlAchievementItem;
        }

        private void OnAchieve()
        {
            RefreshView();
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlAchievement>();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            InitView();
        }

        protected override void OnOpen(object parameter)
        {
            RefreshView();
            base.OnOpen(parameter);
        }

        protected override void OnClose()
        {
            base.OnClose();
            _cachedView.AchievementScrollRect.vertical = false;
            _cachedView.Scrollbar.value = 1;
            for (int i = 0; i < _umCtrlAchievementItemCache.Count; i++)
            {
                _umCtrlAchievementItemCache[i].Collect();
            }
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnAchieve, OnAchieve);
        }

        protected override void SetAnimationType()
        {
            base.SetAnimationType();
            _animationType = EAnimationType.PopupFromDown;
        }

        protected override void OnOpenAnimationComplete()
        {
            base.OnOpenAnimationComplete();
            _cachedView.AchievementScrollRect.vertical = true;
            _cachedView.Scrollbar.value = 1;
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpUI;
        }
    }
}