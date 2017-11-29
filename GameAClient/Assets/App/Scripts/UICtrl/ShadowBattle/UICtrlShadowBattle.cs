using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlShadowBattle : UICtrlResManagedBase<UIViewShadowBattle>
    {
        private Msg_SC_CMD_MatchShadowBattle _matchShadowBattle;
        private USCtrlGameFinishReward[] _rewardCtrl;
        private Reward _reward;

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
            _matchShadowBattle = parameter as Msg_SC_CMD_MatchShadowBattle;
            if (null == _matchShadowBattle)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlShadowBattle>();
                return;
            }
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
            var user = new UserInfoSimple(_matchShadowBattle.Project.UserInfo);
            _cachedView.NickName.text = user.NickName;
            _cachedView.MaleIcon.SetActiveEx(user.Sex == ESex.S_Male);
            _cachedView.FemaleIcon.SetActiveEx(user.Sex == ESex.S_Female);
            _cachedView.AdvLevel.text = user.LevelData.PlayerLevel.ToString();
            _cachedView.CreatorLevel.text = user.LevelData.CreatorLevel.ToString();
            _cachedView.Score.text = _matchShadowBattle.PlayProjectData.ShadowBattleData.Record.Score.ToString();
            _reward = new Reward(_matchShadowBattle.PlayProjectData.ShadowBattleData.Reward);
            UpdateReward(_reward);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHead, user.HeadImgUrl,
                _cachedView.DefaultHeadTexture);
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock,
                _cachedView.BlueImg, _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
        }

        private void OnCancelBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlShadowBattle>();
        }

        private void OnPlayBtn()
        {
            if (null == _matchShadowBattle) return;
            Project project = new Project(_matchShadowBattle.Project);
            long battleId = _matchShadowBattle.PlayProjectData.ShadowBattleData.Id;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求进入关卡");
            project.RequestPlayShadowBattle(battleId, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.Instance.CloseUI<UICtrlShadowBattle>();
                GameManager.Instance.RequestPlay(project);
                SocialApp.Instance.ChangeToGame();
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.ShowPopupDialog("进入关卡失败");
            });
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