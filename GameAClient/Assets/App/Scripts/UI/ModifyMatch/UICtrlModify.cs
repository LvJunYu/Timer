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
	public class UICtrlModify : UISocialCtrlBase<UIViewModify>
    {
        #region 常量与字段
        private const float _randomPickStateTime = 4.0f;
        /// <summary>
        /// 随机改造关卡计时器，点击随机按钮后进入随机状态，至少持续N秒，收到回包且计时器结束则进入下一状态
        /// </summary>
        private float _randomPickTimer;

        #endregion

        #region 属性


        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            Refresh ();

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
                _cachedView.StateTxt.SetActiveEx (false);
                _cachedView.ProjectLocTxt.SetActiveEx (false);
                _cachedView.RandomPickBtn.SetActiveEx (true);
                _cachedView.EditBtn.SetActiveEx (false);
                _cachedView.RepickBtn.SetActiveEx (false);
                _cachedView.PublishBtn.SetActiveEx (false);
                ImageResourceManager.Instance.SetDynamicImageDefault (
                    _cachedView.CoverImg, 
                    _cachedView.DefaultProjectCoverTex);
            } else if (LocalUser.Instance.MatchUserData.CurReformState == (int)EReformState.RS_Editing) {
                _cachedView.StateTxt.SetActiveEx (true);
                _cachedView.ProjectLocTxt.text = string.Format ("Chapt. {0} Level.{1}", 
                    LocalUser.Instance.MatchUserData.CurReformSection,
                    LocalUser.Instance.MatchUserData.CurReformLevel
                );
                _cachedView.ProjectLocTxt.SetActiveEx (true);
                _cachedView.RandomPickBtn.SetActiveEx (false);
                _cachedView.EditBtn.SetActiveEx (true);
                _cachedView.RepickBtn.SetActiveEx (true);
                _cachedView.PublishBtn.SetActiveEx (true);
                // 取单人模式的project，因为改造数据中的project可能还没有获得
                Project project = null;
                int sectionIdx = LocalUser.Instance.MatchUserData.CurReformSection;
                int levelIdx = LocalUser.Instance.MatchUserData.CurReformLevel;
                if (sectionIdx >= AppData.Instance.AdventureData.ProjectList.SectionList.Count) {
                    // todo out of range exception
                    return;
                }
                var section = AppData.Instance.AdventureData.ProjectList.SectionList [sectionIdx];
                if (levelIdx >= section.NormalProjectList.Count) {
                    // todo out of range exception
                    return;
                }
                project = section.NormalProjectList [levelIdx];

                ImageResourceManager.Instance.SetDynamicImage(_cachedView.CoverImg, 
                    project.IconPath,
                    _cachedView.DefaultProjectCoverTex);
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
                    }
                },
                code => {
                    // todo network error handle
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
