using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlShadowBattle : UICtrlResManagedBase<UIViewShadowBattle>
    {
        private USCtrlGameFinishReward[] _rewardCtrl;
        private MatchShadowBattleData _data;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CancelBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.PlayBtn.onClick.AddListener(OnPlayBtn);
            _rewardCtrl = new USCtrlGameFinishReward [_cachedView.Rewards.Length];
            for (int i = 0; i < _cachedView.Rewards.Length; i++)
            {
                _rewardCtrl[i] = new USCtrlGameFinishReward();
                _rewardCtrl[i].Init(_cachedView.Rewards[i]);
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _data = LocalUser.Instance.MatchShadowBattleData;
            RefreshView();
        }

        protected override void OnClose()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserHead, _cachedView.DefaultHeadTexture);
            base.OnClose();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainPopUpUI;
        }

        private void RefreshView()
        {
            if (_data == null) return;
//            UpdateReward(_data);
        }

        private void OnCancelBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlShadowBattle>();
        }

        private void OnPlayBtn()
        {
        }

        private void UpdateReward(Reward reward)
        {
            if (null != reward && reward.IsInited)
            {
                int i = 0;
                for (; i < _rewardCtrl.Length && i < reward.ItemList.Count; i++)
                {
                    _rewardCtrl[i].Set(reward.ItemList[i].GetSprite(), reward.ItemList[i].Count.ToString());
                }
                for (; i < _rewardCtrl.Length; i++)
                {
                    _rewardCtrl[i].Hide();
                }
            }
            else
            {
                for (int i = 0; i < _rewardCtrl.Length; i++)
                {
                    _rewardCtrl[i].Hide();
                }
            }
        }
    }
}