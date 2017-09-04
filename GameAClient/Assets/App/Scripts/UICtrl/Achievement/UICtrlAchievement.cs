using SoyEngine;
using System.Collections.Generic;

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
        private List<UMCtrlAchievementItem> _umCtrlAchievementItemCache;
        private Dictionary<int, AchievementStatisticItem> _allAchievements;

        private void RefreshView()
        {
            if (!_isOpen) return;
            _allAchievements = LocalUser.Instance.Achievement.AllAchievements;
            if (null == _unFinishUMs)
                _unFinishUMs = new List<UMCtrlAchievementItem>(_maxAchievementNum);
            if (null == _finishUMs)
                _finishUMs = new List<UMCtrlAchievementItem>(_maxAchievementNum);
            _unFinishUMs.Clear();
            _finishUMs.Clear();
            CollectAllUMItems();
            //生成未完成成就
            foreach (AchievementStatisticItem achievementStatisticItem in _allAchievements.Values)
            {
                if (achievementStatisticItem.NextLevel != null)
                {
                    UMCtrlAchievementItem umCtrlAchievementItem = createAchievementItem();
                    umCtrlAchievementItem.SetDate(achievementStatisticItem, false);
                    _unFinishUMs.Add(umCtrlAchievementItem);
                }
            }
            //生成已完成成就
            foreach (AchievementStatisticItem achievementStatisticItem in _allAchievements.Values)
            {
                if (achievementStatisticItem.FinishLevel != 0)
                {
                    UMCtrlAchievementItem umCtrlAchievementItem = createAchievementItem();
                    umCtrlAchievementItem.SetDate(achievementStatisticItem, true);
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

        private void CollectAllUMItems()
        {
            if (null == _umCtrlAchievementItemCache) return;
            for (int i = 0; i < _umCtrlAchievementItemCache.Count; i++)
            {
                _umCtrlAchievementItemCache[i].Collect();
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlAchievement>();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshView();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnAddAchievementCount, OnAddAchievementCount);
        }

        private void OnAddAchievementCount()
        {
            RefreshView();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpUI;
        }

        protected override void OnOpenAnimationUpdate()
        {
            base.OnOpenAnimationUpdate();
            _cachedView.Scrollbar.value = 1;
        }
    }
}