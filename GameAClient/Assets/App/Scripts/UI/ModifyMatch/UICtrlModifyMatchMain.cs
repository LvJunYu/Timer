/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using SoyEngine;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlModifyMatchMain : UISocialCtrlBase<UIViewModifyMatchMain>
    {
        #region 常量与字段
        private const long _publishedProjectValidTimeLength = 3 * GameTimer.Day2Ms;

        #endregion

        #region 属性


        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            if (LocalUser.Instance.MatchUserData.IsInited) {
                RefreshAll ();
            }
            LocalUser.Instance.MatchUserData.Request (
                LocalUser.Instance.UserGuid,
                () => {
                    RefreshAll();
                },
                code => {
                    // todo request data failed
                }
            );
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
//			RegisterEvent(EMessengerType.OnChangeToAppMode, OnReturnToApp);
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
            base.OnUpdate ();

            //
        }
			

		private void RefreshAll () {
            RefreshUserInfo ();
            RefreshModifyMatch ();
            RefreshPublishedProject ();
		}

        private void RefreshUserInfo () {
            _cachedView.MakerLevel.text = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel.ToString ();
            _cachedView.CanModifyNum.text = LocalUser.Instance.MatchUserData.ReformModifyUnitCapacity.ToString ();
            _cachedView.CanDeleteNum.text = LocalUser.Instance.MatchUserData.ReformDeleteUnitCapacity.ToString ();
            _cachedView.CanAddNum.text = LocalUser.Instance.MatchUserData.ReformAddUnitCapacity.ToString ();
        }

        private void RefreshModifyMatch () {
            int matchPoint = LocalUser.Instance.MatchUserData.LeftChallengeCount;
            int matchPointMax = LocalUser.Instance.MatchUserData.ChallengeCapacity;
            _cachedView.MatchPoint.text = string.Format ("{0} / {1}", matchPoint, matchPointMax);

            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
            float getMatchPointCDInSecond = 0f;
            float getMatchPointLeftTimeInPercentage = 0f;
            if (0 == matchPoint) {
                getMatchPointCDInSecond = 0.001f * (now - LocalUser.Instance.MatchUserData.LeftChallengeCountRefreshTime);
                getMatchPointLeftTimeInPercentage = 1f - getMatchPointCDInSecond / 
                    (float)LocalUser.Instance.MatchUserData.ChallengeIntervalSecond;
            }
            _cachedView.MatchCDImage.fillAmount = getMatchPointLeftTimeInPercentage;

            if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_WaitForChance) {
                int modifyCDInSecond = LocalUser.Instance.MatchUserData.ReformIntervalSeconds
                    - (int)(0.001f * (now - LocalUser.Instance.MatchUserData.CurPublishTime));
                int hour = modifyCDInSecond / 60 / 60;
                int minute = modifyCDInSecond / 60 - hour * 60;
                int second = modifyCDInSecond - hour * 60 * 60 - minute * 60;
                _cachedView.ModifyCDText.text = string.Format ("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
                _cachedView.ModifyCDImage.fillAmount = (float)modifyCDInSecond / LocalUser.Instance.MatchUserData.ReformIntervalSeconds;
            } else if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_ChanceReady) {
                _cachedView.ModifyCDImage.fillAmount = 0;
                _cachedView.ModifyCDText.text = string.Empty;                
            } else if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_Editing) {
                _cachedView.ModifyCDImage.fillAmount = 0;
                _cachedView.ModifyCDText.text = string.Empty;                
            }
        }

        private void RefreshPublishedProject () {
            bool hasValidPublishProject = CheckPublishedProjectValid ();

            if (hasValidPublishProject) {
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.PublishedProjectSnapShoot, 
                    LocalUser.Instance.MatchUserData.CurPublishProject.IconPath,
                    _cachedView.DefaultProjectCoverTex);
                _cachedView.PassingRate.text = string.Format("{0}%", (int)(LocalUser.Instance.MatchUserData.CurPublishProject.ExtendData.CompleteCount / 
                    (float)LocalUser.Instance.MatchUserData.CurPublishProject.ExtendData.PlayCount * 100));
                int validSecond = (int)((_publishedProjectValidTimeLength - 
                    (DateTimeUtil.GetServerTimeNowTimestampMillis () - LocalUser.Instance.MatchUserData.CurPublishTime)) 
                    / GameTimer.Second2Ms);
                int hour = validSecond / 60 / 60;
                int minute = validSecond / 60 - hour * 60;
                int second = validSecond - hour * 60 * 60 - minute * 60;
                _cachedView.ValidTime.text = string.Format ("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
                // todo 分档位的总数
                _cachedView.ChallengeUserCnt.text = string.Format("{0} / 0", LocalUser.Instance.MatchUserData.PlayCountForReward);                
            } else {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.PublishedProjectSnapShoot, _cachedView.DefaultProjectCoverTex);
                _cachedView.PassingRate.text = "--";
                _cachedView.ValidTime.text = "--:--:--";
                _cachedView.ChallengeUserCnt.text = "0 / 0";
            }
        }

        private bool CheckPublishedProjectValid () {
            bool hasValidPublishProject = LocalUser.Instance.MatchUserData.CurPublishProject != null;

            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
            if ((now - LocalUser.Instance.MatchUserData.CurPublishTime) > _publishedProjectValidTimeLength) {
                hasValidPublishProject = false;
            }
            return hasValidPublishProject;
        }


        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.PopUpUI;
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
                SocialGUIManager.Instance.OpenPopupUI<UICtrlModify> ();
            }
		}

        private void OnMatchBtn () {
            SocialGUIManager.Instance.OpenPopupUI<UICtrlChallengeMatch>();
        }

        private void OnClaimBtn () {
            if (!CheckPublishedProjectValid ()) {
                return;
            }

            RemoteCommands.GetReformReward (1,
                msg => {
                },
                code => {
                }
            );

        }

        #endregion 接口
        #endregion

    }
}
