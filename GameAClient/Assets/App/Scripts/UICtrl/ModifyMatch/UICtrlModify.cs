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
	public class UICtrlModify : UICtrlGenericBase<UIViewModify>
    {
        #region 常量与字段
        private const float _randomPickStateTime = 4.0f;
        private const string _canPublish = "可发布";
        private const string _cantPublish = "还未通关";
        /// <summary>
        /// 随机改造关卡计时器，点击随机按钮后进入随机状态，至少持续N秒，收到回包且计时器结束则进入下一状态
        /// </summary>
        private float _randomPickTimer;
        private USCtrlModifyCard _curModifyCard;
        #endregion

        #region 属性


        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            Refresh ();
            _cachedView.InputBlock.SetActiveEx (false);
            _randomPickTimer = 0;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
			RegisterEvent(EMessengerType.OnChangeToAppMode, OnReturnToApp);
//            RegisterEvent(EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

			_cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);

            _cachedView.RandomPickBtn.onClick.AddListener (OnRamdomPickBtn);
            _cachedView.PublishBtn.onClick.AddListener (OnPublishBtn);
            _cachedView.EditBtn.onClick.AddListener (OnEditBtn);
            _cachedView.RepickBtn.onClick.AddListener (OnRepickBtn);

            _curModifyCard = new USCtrlModifyCard ();
            _curModifyCard.Init (_cachedView.CurrentModifyCard);
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();

            if (_randomPickTimer > 0) {
                _randomPickTimer -= Time.deltaTime;
                if (_randomPickTimer <= 0) {
                    if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_Editing) {
                        if (_isOpen)
                            Refresh ();
                        _cachedView.InputBlock.SetActiveEx (false);
                    } else {
                        _randomPickTimer = 0.001f;
                    }
                }
            }
        }
			

		private void Refresh () {
            if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_ChanceReady) {
                _curModifyCard.SeEmpty ();
                _cachedView.RandomPickBtn.SetActiveEx (true);
                _cachedView.EditBtn.SetActiveEx (false);
                _cachedView.RepickBtn.SetActiveEx (false);
                _cachedView.PublishBtn.SetActiveEx (false);
            } else if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_Editing) {
                _curModifyCard.SetProject (
                    LocalUser.Instance.MatchUserData.CurReformProject,
                    LocalUser.Instance.MatchUserData.CurReformSection,
                    LocalUser.Instance.MatchUserData.CurReformLevel
                );

                _cachedView.RandomPickBtn.SetActiveEx (false);
                _cachedView.EditBtn.SetActiveEx (true);
                _cachedView.RepickBtn.SetActiveEx (true);
                _cachedView.PublishBtn.SetActiveEx (true);

            } else {
                SocialGUIManager.Instance.CloseUI<UICtrlModify> ();
            }
		}

        private void StartModifyEdit () {
            if (!LocalUser.Instance.MatchUserData.CurReformProject.IsInited)
                return;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在获取关卡数据");
            Project project = LocalUser.Instance.MatchUserData.CurReformProject;
//            if (LocalUser.Instance.MatchUserData.CurReformProject.IsInited && 
//                !string.IsNullOrEmpty(LocalUser.Instance.MatchUserData.CurReformProject.ResPath)) {
//                Debug.Log ("_______________________________ modify reform project");
//                project = LocalUser.Instance.MatchUserData.CurReformProject;
//
//            } else {
//            if (string.IsNullOrEmpty(LocalUser.Instance.MatchUserData.CurReformProject.ResPath) &&
//                LocalUser.Instance.MatchUserData.CurReformProject.BytesData == null) {
//                int sectionIdx = LocalUser.Instance.MatchUserData.CurReformSection - 1;
//                int levelIdx = LocalUser.Instance.MatchUserData.CurReformLevel - 1;
//                if (sectionIdx >= AppData.Instance.AdventureData.ProjectList.SectionList.Count) {
//                    Debug.Log ("no project");
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
//
//                    // todo out of range exception
//                    return;
//                }
//                var section = AppData.Instance.AdventureData.ProjectList.SectionList [sectionIdx];
//                if (levelIdx >= section.NormalProjectList.Count) {
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
//
//                    // todo out of range exception
//                    return;
//                }
//                project.ResPath = section.NormalProjectList [levelIdx].ResPath;
////                project = section.NormalProjectList [levelIdx];
//                Debug.Log ("_______________________________ modify orig project");
//            }
//            if (null == project)
//                return;
            project.PrepareRes(
                () => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    GameManager.Instance.RequestModify(project);
                    SocialGUIManager.Instance.ChangeToGameMode();
                }
            );
        }

        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.PopUpUI;
        }
			

		private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlModify>();
		}

		private void OnPublishBtn () {
            if (LocalUser.Instance.MatchUserData.CurReformProject.IsInited &&
                true == LocalUser.Instance.MatchUserData.CurReformProject.PassFlag) {
                SocialGUIManager.ShowPopupDialog (
                    "关卡越难发布后获得的收益越多",
                    "确认发布",
                    new KeyValuePair<string, Action> ("确定", () => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在发布关卡");
                        RemoteCommands.PublishReformProject(
                            LocalUser.Instance.MatchUserData.CurReformProject.ProjectId,
                            LocalUser.Instance.MatchUserData.CurReformProject.ProgramVersion,
                            LocalUser.Instance.MatchUserData.CurReformProject.ResourcesVersion,
                            LocalUser.Instance.MatchUserData.CurReformProject.RecordUsedTime,
                            LocalUser.Instance.MatchUserData.CurReformProject.GetMsgProjectUploadParam(),
                            msg => {
                                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                                if (msg.ResultCode == (int)EProjectOperateResult.POR_Success) {
                                    SocialGUIManager.Instance.CloseUI<UICtrlModify> ();
                                    SocialGUIManager.ShowPopupDialog ("改造关卡发布成功");
                                    LocalUser.Instance.MatchUserData.CurReformState = (int)EReformState.RS_WaitForChance;
                                    LocalUser.Instance.MatchUserData.CurPublishTime = LocalUser.Instance.MatchUserData.CurPublishTime;
                                    LocalUser.Instance.MatchUserData.CurPublishProject.OnSyncFromParent(msg.ProjectData);
                                    Messenger.Broadcast(EMessengerType.OnReformProjectPublished);
                                } else {
                                    SocialGUIManager.ShowPopupDialog ("改造关卡发布失败，代码：9");
                                }
                            },
                            code => {
                                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                                SocialGUIManager.ShowPopupDialog ("改造关卡发布失败，代码：" + code.ToString());
                            }
                        );
                    }),
                    new KeyValuePair<string, Action> ("取消", null)         
                );
            } else {
                SocialGUIManager.ShowPopupDialog ("改造关卡还没有成功通关，不可发布");
            }
		}

        private void OnEditBtn () {
            if (LocalUser.Instance.MatchUserData.IsDirty) {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在获取关卡数据");
                LocalUser.Instance.MatchUserData.Request (
                    LocalUser.Instance.UserGuid,
                    () => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        StartModifyEdit();
                    },
                    code => {
                        // todo network error
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    }
                );
            } else {
                StartModifyEdit();
            }
        }

        private void OnRepickBtn () {
            if (LocalUser.Instance.MatchUserData.CurReformState != (int)EReformState.RS_Editing) {
                return;
            }
            SocialGUIManager.ShowPopupDialog (
                string.Format ("花费 {0}金币 重新随机改造的基础关卡，当前编辑的内容全部作废，可以吗？", 10),
                "确定重新随机吗",
                new KeyValuePair<string, Action> ("确定", () => {
                    // todo repick cost
                    if (GameATools.CheckGold(10)) {
                        LocalUser.Instance.MatchUserData.CurReformState = (int)EReformState.RS_ChanceReady;
                        _randomPickTimer = _randomPickStateTime;
                        _cachedView.InputBlock.SetActiveEx (true);                        
                        RemoteCommands.ReselectReformLevel(
                            LocalUser.Instance.MatchUserData.CurReformSection,
                            LocalUser.Instance.MatchUserData.CurReformLevel,
                            msg => {
                                if (msg.ResultCode == (int)EReselectReformLevelCode.RRLC_Success) {
                                    LocalUser.Instance.MatchUserData.CurReformSection = msg.NewReformSection;
                                    LocalUser.Instance.MatchUserData.CurReformLevel = msg.NewReformLevel;
                                    LocalUser.Instance.MatchUserData.CurReformState = (int)EReformState.RS_Editing;
                                    LocalUser.Instance.MatchUserData.CurReformProject.TargetSection = msg.NewReformSection;
                                    LocalUser.Instance.MatchUserData.CurReformProject.TargetLevel = msg.NewReformLevel;
                                    LocalUser.Instance.MatchUserData.CurReformProject.ResPath = string.Empty;
                                    GameATools.LocalUseGold(10);
                                    // 立刻请求更新数据，以获取改造中的project
                                    LocalUser.Instance.MatchUserData.Request(LocalUser.Instance.UserGuid, null, null);
                                } else {
                                    LocalUser.Instance.MatchUserData.CurReformState = (int)EReformState.RS_Editing;
                                    SocialGUIManager.ShowPopupDialog("重新随机改造关卡失败，原因代码：" + msg.ResultCode.ToString());
                                    _randomPickTimer = 0.01f;
                                }
                            },
                            code => {
                                // todo handle error
                                SocialGUIManager.ShowPopupDialog("重新随机改造关卡失败，原因代码：" + code.ToString());
                                _randomPickTimer = 0.01f;
                            }
                        );
                    }
                }),
                new KeyValuePair<string, Action> ("取消", () => {
                })
            );
        }

        private void OnRamdomPickBtn () {
            if (LocalUser.Instance.MatchUserData.CurReformState != (int)EReformState.RS_ChanceReady) {
                return;
            }
            _randomPickTimer = _randomPickStateTime;
            _cachedView.InputBlock.SetActiveEx (true);
            RemoteCommands.Reform (0,
                msg => {
                    if ((int)EReformCode.ReformC_Success == msg.ResultCode) {
                        LocalUser.Instance.MatchUserData.CurReformSection = msg.NewReformSection;
                        LocalUser.Instance.MatchUserData.CurReformLevel = msg.NewReformLevel;
                        LocalUser.Instance.MatchUserData.CurReformState = (int)EReformState.RS_Editing;
                        // 立刻请求更新数据，以获取改造中的project
                        LocalUser.Instance.MatchUserData.Request(LocalUser.Instance.UserGuid, null, null);
                    } else {
                        // todo network error handle
                        //Debug.Log ("___________________Reform error result code : " + msg.ResultCode);
                        _cachedView.InputBlock.SetActiveEx (false);
                        _randomPickTimer = 0f;
                        SocialGUIManager.ShowPopupDialog ("错误代码： " + msg.ResultCode, "出错了", new KeyValuePair<string, Action>("确认", null));
                    }
                },
                code => {
                    // todo network error handle
                    _cachedView.InputBlock.SetActiveEx (false);
                    _randomPickTimer = 0f;
                    SocialGUIManager.ShowPopupDialog ("错误代码： " + code.ToString (), "网络错误", new KeyValuePair<string, Action> ("确认", null));
                }
            );
        }

        private void OnReturnToApp () {
//            if (LocalUser.Instance.MatchUserData.IsDirty) {
//                LocalUser.Instance.MatchUserData.Request (
//                    LocalUser.Instance.UserGuid,
//                    () => {
//                        Refresh();
//                    },
//                    code => {
//                        // todo network error
//                    }
//                );
//            }
        }
        #endregion 接口
        #endregion

    }
}
