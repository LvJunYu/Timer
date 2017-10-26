﻿/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlModifyMatchMain : UICtrlGenericBase<UIViewModifyMatchMain>
    {
        #region 常量与字段
        //private const long _publishedProjectValidTimeLength = 3 * GameTimer.Day2Ms;


        private const long _autoRefershUIInterval = 200;
        private const long _autoRequstDataInterval = 1000;
        private long _lastAutoRefreshView;
        private long _lastRequstDataTime;
        private bool _pushGoldEnergyStyle;
        #endregion

        #region 属性


        #endregion

        #region 方法

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            if (LocalUser.Instance.MatchUserData.IsInited) {
                RefreshViewAll ();
                LocalUser.Instance.MatchUserData.Request (
                    LocalUser.Instance.UserGuid,
                    () => {
                        RefreshViewAll ();
                    },
                    code => {
                        // todo request data failed
                    }
                );
            } else {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
                LocalUser.Instance.MatchUserData.Request (
                    LocalUser.Instance.UserGuid,
                    () => {
                        RefreshViewAll ();
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    },
                    code => {
                        // todo request data failed
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        //SocialGUIManager.Instance.CloseUI<UICtrlModifyMatchMain> ();
                    }
                );
            }
            if (LocalUser.Instance.MatchUserData.IsInited) {
                LocalUser.Instance.MatchUserData.LocalRefresh ();
            }
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.None);
                _pushGoldEnergyStyle = true;
            }
            //RefreshViewAll ();
            _lastAutoRefreshView = DateTimeUtil.GetServerTimeNowTimestampMillis ();
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
//            Messenger<Project>.AddListener (EMessengerType.OnReformProjectPublished, OnReformProjectPublished);
			RegisterEvent(EMessengerType.OnReformProjectPublished, OnReformProjectPublished);
//            RegisterEvent(EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

			_cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);

			_cachedView.ModifyBtn.onClick.AddListener (OnModifyBtn);
            _cachedView.MatchBtn.onClick.AddListener (OnMatchBtn);
            _cachedView.ClaimBtn.onClick.AddListener (OnClaimBtn);
        }

        public override void OnUpdate ()
        {
            if (!_isOpen) return;
            base.OnUpdate ();
            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
            if (now - _lastAutoRefreshView > _autoRefershUIInterval) {
                _lastAutoRefreshView = now;
                LocalUser.Instance.MatchUserData.LocalRefresh ();
                RefreshViewAll ();
            }
            //
            if (CheckPublishedProjectValid () && 
                now - _lastRequstDataTime > _autoRequstDataInterval) {
                _lastRequstDataTime = now;
                LocalUser.Instance.MatchUserData.Request (
                    LocalUser.Instance.UserGuid,
                    () => {
                        RefreshViewAll ();
                    },
                    code => {
                        // todo request data failed
                    }
                );
                _lastRequstDataTime = now;
            }
        }
			

		private void RefreshViewAll () {
            RefreshUserInfo ();
            RefreshModify ();
            RefreshMatch ();
            RefreshPublishedProject ();
		}

        private void RefreshUserInfo () {
            _cachedView.MakerLevel.text = LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel.ToString ();
            _cachedView.CanModifyNum.text = LocalUser.Instance.MatchUserData.ReformModifyUnitCapacity.ToString ();
            _cachedView.CanDeleteNum.text = LocalUser.Instance.MatchUserData.ReformDeleteUnitCapacity.ToString ();
            _cachedView.CanAddNum.text = LocalUser.Instance.MatchUserData.ReformAddUnitCapacity.ToString ();
        }

        private void RefreshMatch () {
            int matchPoint = LocalUser.Instance.MatchUserData.LeftChallengeCount;
            MatchUserData.EChallengeState challengeState = LocalUser.Instance.MatchUserData.CurrentChallengeState ();
            if (MatchUserData.EChallengeState.Selecting == challengeState || 
                MatchUserData.EChallengeState.Challenging == challengeState) {
                matchPoint += 1;
            }
            int matchPointMax = LocalUser.Instance.MatchUserData.ChallengeCapacity;
            _cachedView.MatchPoint.text = string.Format ("{0} / {1}", matchPoint, matchPointMax);

            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
            if (0 == matchPoint) {
                var getMatchPointLeftTimeInSecond = LocalUser.Instance.MatchUserData.ChallengeIntervalSecond - 
                                                      0.001f * (now - LocalUser.Instance.MatchUserData.LeftChallengeCountRefreshTime);
                var getMatchPointLeftTimeInPercentage = getMatchPointLeftTimeInSecond /
                                                          LocalUser.Instance.MatchUserData.ChallengeIntervalSecond;
                _cachedView.MatchPoint.gameObject.SetActive (false);
                int hour = (int)getMatchPointLeftTimeInSecond / 60 / 60;
                int minute = (int)getMatchPointLeftTimeInSecond / 60 - hour * 60;
                int second = (int)getMatchPointLeftTimeInSecond - hour * 60 * 60 - minute * 60;
                _cachedView.matchCDText.text = string.Format ("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
                _cachedView.matchCDText.gameObject.SetActive (true);
                SetLightImageProgress (1f - getMatchPointLeftTimeInPercentage, _cachedView.MatchLightSmall, _cachedView.MatchLightBig);
                _cachedView.MatchBtn.gameObject.SetActive (false);
                _cachedView.MatchRedPoint.gameObject.SetActive(false);
                _cachedView.MatchDoneText.gameObject.SetActive(false);
            } else {
                _cachedView.MatchPoint.gameObject.SetActive (true);
                _cachedView.matchCDText.gameObject.SetActive (false);
                SetLightImageProgress (1, _cachedView.MatchLightSmall, _cachedView.MatchLightBig);
                _cachedView.MatchBtn.gameObject.SetActive (true);
                _cachedView.MatchRedPoint.gameObject.SetActive(true);
                _cachedView.MatchDoneText.gameObject.SetActive(true);
            }

        }

        private void RefreshModify () {
            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();

            if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_WaitForChance) {
                int modifyCDInSecond = LocalUser.Instance.MatchUserData.ReformIntervalSeconds
                    - (int)(0.001f * (now - LocalUser.Instance.MatchUserData.CurPublishTime));
                int hour = modifyCDInSecond / 60 / 60;
                int minute = modifyCDInSecond / 60 - hour * 60;
                int second = modifyCDInSecond - hour * 60 * 60 - minute * 60;
                _cachedView.ModifyCDText.text = string.Format ("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
                _cachedView.ModifyCDText.gameObject.SetActive (true);
                SetLightImageProgress (
                    (float)((now - LocalUser.Instance.MatchUserData.CurPublishTime)) / LocalUser.Instance.MatchUserData.ReformIntervalSeconds * 0.001f,
                    _cachedView.ModifyLightSmall, _cachedView.ModifyLightBig);
                _cachedView.ModifyBtn.gameObject.SetActive (false);
                _cachedView.ModifyChanceReady.gameObject.SetActive (false);
                _cachedView.ModifyRedPoint.gameObject.SetActive(false);
                //_cachedView.ModifyCDImage.fillAmount = (float)modifyCDInSecond / LocalUser.Instance.MatchUserData.ReformIntervalSeconds;
            } else if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_ChanceReady) {
                //_cachedView.ModifyCDImage.fillAmount = 0;
                _cachedView.ModifyCDText.text = string.Empty;
                _cachedView.ModifyCDText.gameObject.SetActive (false);
                SetLightImageProgress (1, _cachedView.ModifyLightSmall, _cachedView.ModifyLightBig);
                _cachedView.ModifyBtn.gameObject.SetActive (true);
                _cachedView.ModifyChanceReady.gameObject.SetActive (true);
                _cachedView.ModifyRedPoint.gameObject.SetActive(true);
                _cachedView.ModifyDoneText.gameObject.SetActive(true);
            } else if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_Editing) {
                //_cachedView.ModifyCDImage.fillAmount = 0;
                _cachedView.ModifyCDText.text = string.Empty;
                _cachedView.ModifyCDText.gameObject.SetActive (false);
                SetLightImageProgress (1, _cachedView.ModifyLightSmall, _cachedView.ModifyLightBig);
                _cachedView.ModifyBtn.gameObject.SetActive (true);
                _cachedView.ModifyChanceReady.gameObject.SetActive (true);
//                _cachedView.ModifyRedPoint.gameObject.SetActive(true);
                _cachedView.ModifyDoneText.gameObject.SetActive(true);
            }
        }

        private void RefreshPublishedProject () {
            bool hasValidPublishProject = CheckPublishedProjectValid ();

            if (hasValidPublishProject) {
                _cachedView.PublishedProjectSnapShoot.gameObject.SetActive (true);
                ImageResourceManager.Instance.SetDynamicImage(
                    _cachedView.PublishedProjectSnapShoot, 
                    LocalUser.Instance.MatchUserData.GetProjectIconPath (
                        LocalUser.Instance.MatchUserData.CurPublishProject.TargetSection - 1,
                        LocalUser.Instance.MatchUserData.CurPublishProject.TargetLevel - 1
                    ),
                    _cachedView.DefaultProjectCoverTex);
                int passRate = 0;
                if (LocalUser.Instance.MatchUserData.CurPublishProject.PlayCount > 0) {
                    passRate = (int)(LocalUser.Instance.MatchUserData.CurPublishProject.ExtendData.CompleteCount /
                    (float)LocalUser.Instance.MatchUserData.CurPublishProject.PlayCount * 100);
                }
                _cachedView.PassingRate.text = string.Format ("{0}%", passRate);
                int validSecond = (int)((MatchUserData.PublishedProjectValidTimeLength - 
                    (DateTimeUtil.GetServerTimeNowTimestampMillis () - LocalUser.Instance.MatchUserData.CurPublishTime)) 
                    / GameTimer.Second2Ms);
                int hour = validSecond / 60 / 60;
                int minute = validSecond / 60 - hour * 60;
                int second = validSecond - hour * 60 * 60 - minute * 60;
                _cachedView.ValidTime.text = string.Format ("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
            } else {
                _cachedView.PublishedProjectSnapShoot.gameObject.SetActive (false);
                //ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.PublishedProjectSnapShoot, _cachedView.DefaultProjectCoverTex);
                _cachedView.PassingRate.text = "--";
                _cachedView.ValidTime.text = "--:--:--";
                _cachedView.ChallengeUserCnt.text = "- / -";
            }
            // claim reward
            _cachedView.ChallengeUserCnt.text = string.Format (
                    "{0} / {1}",
                    LocalUser.Instance.MatchUserData.PlayCountForReward,
                    LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity
                );
            float percentage = _cachedView.ChallengeUserCntBar.fillAmount = LocalUser.Instance.MatchUserData.PlayCountForReward / (float)LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity;
            _cachedView.ChallengeUserCntBar.fillAmount = percentage;
            if (percentage > 0.25f) {
                _cachedView.ClaimBtn.interactable = true;
                _cachedView.ChallengeMark1.gameObject.SetActive (true);
            } else {
                _cachedView.ChallengeMark1.gameObject.SetActive (false);
                _cachedView.ClaimBtn.interactable = false;
            }
            if (percentage > 0.5f) {
                _cachedView.ChallengeMark2.gameObject.SetActive (true);
            } else {
                _cachedView.ChallengeMark2.gameObject.SetActive (false);
            }
            if (percentage > 0.75f) {
                _cachedView.ChallengeMark3.gameObject.SetActive (true);
            } else {
                _cachedView.ChallengeMark3.gameObject.SetActive (false);
            }
        }

        private bool CheckPublishedProjectValid () {
            bool hasValidPublishProject = LocalUser.Instance.MatchUserData.CurPublishProject != null;

            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
            if ((now - LocalUser.Instance.MatchUserData.CurPublishTime) > MatchUserData.PublishedProjectValidTimeLength) {
                hasValidPublishProject = false;
            }
            return hasValidPublishProject;
        }


        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.MainPopUpUI;
        }
		private void OnCloseBtn () {
			SocialGUIManager.Instance.CloseUI<UICtrlModifyMatchMain>();
		}

		private void OnModifyBtn () {
//			List<Project> projectList =	AppData.Instance.AdventureData.ProjectList.SectionList [0].NormalProjectList;
//			projectList[0].PrepareRes(
//				() => {
//					GameManager.Instance.RequestModify(projectList[0]);
//					SocialGUIManager.Instance.ChangeToGameMode();
//				}
//			);
            if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_WaitForChance) {
            } else {
                SocialGUIManager.Instance.OpenUI<UICtrlModify> ();
            }
		}

        private void OnMatchBtn () {
            if (MatchUserData.EChallengeState.WaitForChance == LocalUser.Instance.MatchUserData.CurrentChallengeState()) {
                
            } else {
                SocialGUIManager.Instance.OpenUI<UICtrlChallengeMatch> ();
            }
        }

        private void OnClaimBtn ()
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

        #endregion 接口
        private void OnReformProjectPublished () {
            if (!_isViewCreated)
            {
                return;
            }
            LocalUser.Instance.MatchUserData.Request(
                LocalUser.Instance.UserGuid,
                () => {
                    RefreshViewAll();
                },
                code => {
                    // error handle
                }
            );
        }
        #endregion

        private void SetLightImageProgress (float p, GameObject[] small, GameObject[] big)
        {
            p = Mathf.Clamp01 (p);
            int lightCount = small.Length;
            int intProgress = (int)(p * lightCount);
            int i = 0;
            for (; i < intProgress; i++)
            {
                small [i].SetActive (true);
                big [i].SetActive (false);
            }
            for (; i < lightCount; i++)
            {
                small [i].SetActive (false);
                big [i].SetActive (false);
            }
            if (intProgress < lightCount)
            {
                big [intProgress].SetActive (true);
            }
        }
    }
}
