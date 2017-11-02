using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlModifyMatchMain : UICtrlGenericBase<UIViewModifyMatchMain>
    {
        //private const long _publishedProjectValidTimeLength = 3 * GameTimer.Day2Ms;
        private const long _autoRefershUIInterval = 1000;

        private const long _autoRequstDataInterval = 5000;
        private long _lastAutoRefreshView;
        private long _lastRequstDataTime;
        private bool _pushGoldEnergyStyle;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ModifyBtn.onClick.AddListener(OnModifyBtn);
            _cachedView.MatchBtn.onClick.AddListener(OnMatchBtn);
            _cachedView.ClaimBtn.onClick.AddListener(OnClaimBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (LocalUser.Instance.MatchUserData.IsInited)
            {
                LocalUser.Instance.MatchUserData.LocalRefresh();
                RefreshViewAll();
                LocalUser.Instance.MatchUserData.Request(LocalUser.Instance.UserGuid, RefreshViewAll, code => { });
            }
            else
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
                LocalUser.Instance.MatchUserData.Request(LocalUser.Instance.UserGuid, () =>
                    {
                        RefreshViewAll();
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    },
                    code =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        SocialGUIManager.ShowPopupDialog("请求匹配数据失败。");
                    }
                );
            }
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.None);
                _pushGoldEnergyStyle = true;
            }
            _lastAutoRefreshView = DateTimeUtil.GetServerTimeNowTimestampMillis();
            _lastRequstDataTime = _lastAutoRefreshView;
        }

        protected override void OnClose()
        {
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
//            RegisterEvent<Project>(EMessengerType.OnReformProjectPublished, OnReformProjectPublished);
            RegisterEvent(EMessengerType.OnReformProjectPublished, OnReformProjectPublished);
//            RegisterEvent(EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainPopUpUI;
        }

        public override void OnUpdate()
        {
            if (!_isOpen) return;
            base.OnUpdate();
            long now = DateTimeUtil.GetServerTimeNowTimestampMillis();
            if (now - _lastAutoRefreshView > _autoRefershUIInterval)
            {
                _lastAutoRefreshView = now;
                LocalUser.Instance.MatchUserData.LocalRefresh();
                RefreshViewAll();
            }
            if (CheckPublishedProjectValid() && now - _lastRequstDataTime > _autoRequstDataInterval)
            {
                _lastRequstDataTime = now;
                LocalUser.Instance.MatchUserData.Request(LocalUser.Instance.UserGuid, RefreshViewAll, code => { });
            }
        }

        private void RefreshViewAll()
        {
            RefreshUserInfo();
            RefreshModify();
            RefreshMatch();
            RefreshPublishedProject();
        }

        private void RefreshUserInfo()
        {
            _cachedView.MakerLevel.text = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel.ToString();
            _cachedView.CanModifyNum.text = LocalUser.Instance.MatchUserData.ReformModifyUnitCapacity.ToString();
            _cachedView.CanDeleteNum.text = LocalUser.Instance.MatchUserData.ReformDeleteUnitCapacity.ToString();
            _cachedView.CanAddNum.text = LocalUser.Instance.MatchUserData.ReformAddUnitCapacity.ToString();
        }

        private void RefreshMatch()
        {
            int matchPoint = LocalUser.Instance.MatchUserData.LeftChallengeCount;
            MatchUserData.EChallengeState challengeState = LocalUser.Instance.MatchUserData.CurrentChallengeState();
            if (MatchUserData.EChallengeState.Selecting == challengeState ||
                MatchUserData.EChallengeState.Challenging == challengeState)
            {
                matchPoint += 1;
            }
            bool canMatch = matchPoint != 0;
            _cachedView.MatchPoint.SetActiveEx(canMatch);
            _cachedView.matchCDText.SetActiveEx(!canMatch);
            _cachedView.MatchBtn.SetActiveEx(canMatch);
            _cachedView.MatchRedPoint.SetActiveEx(canMatch);
            _cachedView.MatchDoneText.SetActiveEx(canMatch);
            if (canMatch)
            {
                int matchPointMax = LocalUser.Instance.MatchUserData.ChallengeCapacity;
                _cachedView.MatchPoint.text = string.Format("{0} / {1}", matchPoint, matchPointMax);
                SetLightImageProgress(1, _cachedView.MatchLightSmall, _cachedView.MatchLightBig);
            }
            else
            {
                long now = DateTimeUtil.GetServerTimeNowTimestampMillis();
                var leftTimeInSecond = LocalUser.Instance.MatchUserData.ChallengeIntervalSecond - 0.001f *
                                       (now - LocalUser.Instance.MatchUserData.LeftChallengeCountRefreshTime);
                var leftTimeInPercentage = leftTimeInSecond / LocalUser.Instance.MatchUserData.ChallengeIntervalSecond;
                _cachedView.matchCDText.text = GameATools.GetTimeStringBySecond((int) leftTimeInSecond);
                SetLightImageProgress(1f - leftTimeInPercentage, _cachedView.MatchLightSmall,
                    _cachedView.MatchLightBig);
            }
        }

        private void RefreshModify()
        {
            bool canModify = LocalUser.Instance.MatchUserData.CurReformState != (int) EReformState.RS_WaitForChance;
            _cachedView.ModifyCDText.SetActiveEx(!canModify);
            _cachedView.ModifyRedPoint.SetActiveEx(canModify);
            _cachedView.ModifyBtn.SetActiveEx(canModify);
            _cachedView.ModifyChanceReady.SetActiveEx(canModify);
            _cachedView.ModifyDoneText.SetActiveEx(canModify);
            if (canModify)
            {
                SetLightImageProgress(1, _cachedView.ModifyLightSmall, _cachedView.ModifyLightBig);
            }
            else
            {
                long now = DateTimeUtil.GetServerTimeNowTimestampMillis();
                int modifyCDInSecond = LocalUser.Instance.MatchUserData.ReformIntervalSeconds
                                       - (int) (0.001f * (now - LocalUser.Instance.MatchUserData.CurPublishTime));
                _cachedView.ModifyCDText.text = GameATools.GetTimeStringBySecond(modifyCDInSecond);
                SetLightImageProgress(
                    (float) (now - LocalUser.Instance.MatchUserData.CurPublishTime) /
                    LocalUser.Instance.MatchUserData.ReformIntervalSeconds * 0.001f,
                    _cachedView.ModifyLightSmall, _cachedView.ModifyLightBig);
            }
        }

        private void RefreshPublishedProject()
        {
            bool hasValidPublishProject = CheckPublishedProjectValid();
            _cachedView.PublishedProjectSnapShoot.SetActiveEx(hasValidPublishProject);
            if (hasValidPublishProject)
            {
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.PublishedProjectSnapShoot,
                    LocalUser.Instance.MatchUserData.GetProjectIconPath(
                        LocalUser.Instance.MatchUserData.CurPublishProject.TargetSection - 1,
                        LocalUser.Instance.MatchUserData.CurPublishProject.TargetLevel - 1),
                    _cachedView.DefaultProjectCoverTex);
                int passRate = 0;
                if (LocalUser.Instance.MatchUserData.CurPublishProject.PlayCount > 0)
                {
                    passRate = (int) (LocalUser.Instance.MatchUserData.CurPublishProject.ExtendData.CompleteCount /
                                      (float) LocalUser.Instance.MatchUserData.CurPublishProject.PlayCount * 100);
                }
                _cachedView.PassingRate.text = string.Format("{0}%", passRate);
                int validSecond = (int) ((MatchUserData.PublishedProjectValidTimeLength -
                                          (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                                           LocalUser.Instance.MatchUserData.CurPublishTime))
                                         / GameTimer.Second2Ms);
                _cachedView.ValidTime.text = GameATools.GetTimeStringBySecond(validSecond);
            }
            else
            {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.PublishedProjectSnapShoot,
                    _cachedView.DefaultProjectCoverTex);
                _cachedView.PassingRate.text = "--";
                _cachedView.ValidTime.text = "--:--:--";
                _cachedView.ChallengeUserCnt.text = "- / -";
            }
            _cachedView.ChallengeUserCnt.text = string.Format("{0} / {1}",
                LocalUser.Instance.MatchUserData.PlayCountForReward,
                LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity);
            float percentage = _cachedView.ChallengeUserCntBar.fillAmount =
                LocalUser.Instance.MatchUserData.PlayCountForReward /
                (float) LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity;
            _cachedView.ChallengeUserCntBar.fillAmount = percentage;
            _cachedView.ClaimBtn.interactable = percentage >= 0.25f;
            _cachedView.ChallengeMark1.SetActiveEx(percentage >= 0.25f);
            _cachedView.ChallengeMark2.SetActiveEx(percentage >= 0.5f);
            _cachedView.ChallengeMark3.SetActiveEx(percentage >= 0.75f);
        }

        private bool CheckPublishedProjectValid()
        {
            bool hasValidPublishProject = LocalUser.Instance.MatchUserData.CurPublishProject != null;
            long now = DateTimeUtil.GetServerTimeNowTimestampMillis();
            if (now - LocalUser.Instance.MatchUserData.CurPublishTime > MatchUserData.PublishedProjectValidTimeLength)
            {
                hasValidPublishProject = false;
            }
            return hasValidPublishProject;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlModifyMatchMain>();
        }

        private void OnModifyBtn()
        {
            if (LocalUser.Instance.MatchUserData.CurReformState > (int) EReformState.RS_WaitForChance)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlModify>();
            }
        }

        private void OnMatchBtn()
        {
            if (LocalUser.Instance.MatchUserData.CurrentChallengeState() > MatchUserData.EChallengeState.WaitForChance)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlChallengeMatch>();
            }
        }

        private void OnClaimBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlMatchGetReward>();
//            //if (!CheckPublishedProjectValid ()) {
//            //    return;
//            //}
//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在收取奖励");
//            int rewardLevel = (int)((float)LocalUser.Instance.MatchUserData.PlayCountForReward / LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity / 0.25f);
//            if (rewardLevel < 1) return;
//            rewardLevel = Mathf.Clamp (rewardLevel, 1, 3);
//            Debug.Log ("________________________________________ rewardlevel: " + rewardLevel);
//            RemoteCommands.GetReformReward (
//                rewardLevel,
//                msg => {
//                    if ((int)EGetReformRewardCode.GRRC_Success == msg.ResultCode) {
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
//                        var reward = new Reward (msg.Reward);
//                        for (int i = 0; i < reward.ItemList.Count; i++)
//                        {
//                            reward.ItemList[i].AddToLocal();
//                        }
//                        SocialGUIManager.ShowReward (reward);
//                        LocalUser.Instance.MatchUserData.PlayCountForReward = 0;
//                        RefreshPublishedProject ();
//
//                    } else
//                    {
//                        // TODO error handle    
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
//                    }
//                },
//                code => {
//                    // TODO error handle
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
//                }
//            );
        }

        private void OnReformProjectPublished()
        {
            if (_isOpen)
            {
                LocalUser.Instance.MatchUserData.Request(LocalUser.Instance.UserGuid, RefreshViewAll, code => { });
            }
        }

        private void SetLightImageProgress(float p, GameObject[] small, GameObject[] big)
        {
            p = Mathf.Clamp01(p);
            int lightCount = small.Length;
            int intProgress = (int) (p * lightCount);
            int i = 0;
            for (; i < intProgress; i++)
            {
                small[i].SetActive(true);
                big[i].SetActive(false);
            }
            for (; i < lightCount; i++)
            {
                small[i].SetActive(false);
                big[i].SetActive(false);
            }
            if (intProgress < lightCount)
            {
                big[intProgress].SetActive(true);
            }
        }
    }
}