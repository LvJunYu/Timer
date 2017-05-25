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
    public class UICtrlChallengeMatch : UICtrlGenericBase<UIViewChallengeMatch>
    {
        #region 常量与字段
        private USCtrlChallengeProjectCard _easyCard;
        private USCtrlChallengeProjectCard _normalCard;
        private USCtrlChallengeProjectCard _hardCard;
        private USCtrlChallengeProjectCard _unkownCard;

//        private EChallengeProjectType _selectChallengeTemp;

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

            _cachedView.InputBlock.gameObject.SetActive (false);
            _randomPickTimer = 0f;
            if (LocalUser.Instance.MatchUserData.IsDirty) {
                LocalUser.Instance.MatchUserData.Request (
                    LocalUser.Instance.UserGuid,
                    () => {
                        Refresh();
                    },
                    code => {
                    }
                );
            }
            Refresh ();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            Messenger<EChallengeProjectType>.AddListener (EMessengerType.OnChallengeProjectSelected, OnChallengeProjectSelected);
			RegisterEvent(EMessengerType.OnChangeToAppMode, OnReturnToApp);
//            RegisterEvent(EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

			_cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
            _cachedView.AbandomBtn.onClick.AddListener (OnAbandomBtn);
            _cachedView.RandomPickBtn.onClick.AddListener (OnRandomPickBtn);
            _cachedView.ChallengeBtn.onClick.AddListener (OnChallengeBtn);
            _easyCard = new USCtrlChallengeProjectCard ();
            _easyCard.Init (_cachedView.ChallengeProjectEasy);
            _normalCard = new USCtrlChallengeProjectCard ();
            _normalCard.Init (_cachedView.ChallengeProjectNormal);
            _hardCard = new USCtrlChallengeProjectCard ();
            _hardCard.Init (_cachedView.ChallengeProjectHard);
            _unkownCard = new USCtrlChallengeProjectCard ();
            _unkownCard.Init (_cachedView.ChallengeProjectUnkown);
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();

            if (_randomPickTimer > 0) {
                _randomPickTimer -= Time.deltaTime;
                if (_randomPickTimer <= 0) {
                    if (LocalUser.Instance.MatchUserData.CurrentChallengeState() == MatchUserData.EChallengeState.Selecting) {
                        if (_isOpen)
                            Refresh ();
                        _cachedView.InputBlock.gameObject.SetActive (false);
                    } else {
                        _randomPickTimer = 0.001f;
                    }
                }
            }
        }
			

		private void Refresh () {
            if (LocalUser.Instance.MatchUserData.CurrentChallengeState() == MatchUserData.EChallengeState.ChanceReady) {
                _easyCard.SeEmpty ();
                _normalCard.SeEmpty ();
                _hardCard.SeEmpty ();
                _unkownCard.SeEmpty ();
                _cachedView.RandomPickBtn.gameObject.SetActive (true);
                _cachedView.ChallengeBtn.gameObject.SetActive (false);
                _cachedView.AbandomBtn.gameObject.SetActive (false);
                _cachedView.SelectTip.gameObject.SetActive (false);
            } else if (LocalUser.Instance.MatchUserData.CurrentChallengeState() == MatchUserData.EChallengeState.Selecting) {
                _easyCard.SetProject (LocalUser.Instance.MatchUserData.EasyChallengeProjectData, EChallengeProjectType.CPT_Easy);
                _normalCard.SetProject (LocalUser.Instance.MatchUserData.MediumChallengeProjectData, EChallengeProjectType.CPT_Medium);
                _hardCard.SetProject (LocalUser.Instance.MatchUserData.DifficultChallengeProjectData, EChallengeProjectType.CPT_Difficult);
                _unkownCard.SetProject (LocalUser.Instance.MatchUserData.RandomChallengeProjectData, EChallengeProjectType.CPT_Random);
                _cachedView.RandomPickBtn.gameObject.SetActive (false);
                _cachedView.ChallengeBtn.gameObject.SetActive (false);
                _cachedView.AbandomBtn.gameObject.SetActive (false);
                _cachedView.SelectTip.gameObject.SetActive (true);
//                _selectChallengeTemp = EChallengeProjectType.CPT_None;
            } else if (LocalUser.Instance.MatchUserData.CurrentChallengeState() == MatchUserData.EChallengeState.Challenging) {
                _easyCard.SetProject (LocalUser.Instance.MatchUserData.EasyChallengeProjectData, EChallengeProjectType.CPT_Easy,
                    LocalUser.Instance.MatchUserData.CurSelectedChallengeType == (int)EChallengeProjectType.CPT_Easy ? 1 : -1);
                _normalCard.SetProject (LocalUser.Instance.MatchUserData.MediumChallengeProjectData, EChallengeProjectType.CPT_Medium,
                    LocalUser.Instance.MatchUserData.CurSelectedChallengeType == (int)EChallengeProjectType.CPT_Medium ? 1 : -1);
                _hardCard.SetProject (LocalUser.Instance.MatchUserData.DifficultChallengeProjectData, EChallengeProjectType.CPT_Difficult,
                    LocalUser.Instance.MatchUserData.CurSelectedChallengeType == (int)EChallengeProjectType.CPT_Difficult ? 1 : -1);
                _unkownCard.SetProject (LocalUser.Instance.MatchUserData.RandomChallengeProjectData, EChallengeProjectType.CPT_Random,
                    LocalUser.Instance.MatchUserData.CurSelectedChallengeType == (int)EChallengeProjectType.CPT_Random ? 1 : -1);
                _cachedView.RandomPickBtn.gameObject.SetActive (false);
                _cachedView.ChallengeBtn.gameObject.SetActive (true);
                _cachedView.AbandomBtn.gameObject.SetActive (true);
                _cachedView.SelectTip.gameObject.SetActive (false);
                _cachedView.ChallengeEnergyCost.text = string.Format ("-{0}", LocalUser.Instance.MatchUserData.CurMatchChallengeCount);
            } else {
                SocialGUIManager.Instance.CloseUI<UICtrlChallengeMatch> ();
            }
		}



        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.PopUpUI;
        }
			

		private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlChallengeMatch>();
		}

//        private void OnSelectBtn () {
//            if (MatchUserData.EChallengeState.Selecting != LocalUser.Instance.MatchUserData.CurrentChallengeState())
//                return;
//            if (_selectChallengeTemp == EChallengeProjectType.CPT_None) {
//                SocialGUIManager.ShowPopupDialog (
//                    "请先点击一个关卡卡片", null, new KeyValuePair<string, Action> ("知道了", null));
//            } else {
//                SocialGUIManager.ShowPopupDialog(
//                    string.Format("确定选择 {1} 进行挑战吗？首次选择免费，重选需要支付金币", _selectChallengeTemp),
//                    "请确定",
//                    new KeyValuePair<string, Action>("确定", () => {
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
//                        RemoteCommands.SelectMatchChallengeProject(
//                            _selectChallengeTemp, false, 
//                            msg => {
//                                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
//                                if (msg.ResultCode == (int)ESelectMatchChallengeProjectCode.SMCPC_Success) {
//
//                                } else {
//                                    // error handle
//                                }
//                            },
//                            code => {
//                                // error handle
//                                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
//                            }
//                        );
//                    }),
//                    new KeyValuePair<string, Action>("取消", null)
//                );
//            }			
//		}

        private void OnChallengeBtn () {
            if (MatchUserData.EChallengeState.Challenging != LocalUser.Instance.MatchUserData.CurrentChallengeState())
                return;


            if (GameATools.CheckEnergy(LocalUser.Instance.MatchUserData.CurMatchChallengeCount)) {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在进入挑战关卡");
                LocalUser.Instance.MatchUserData.PlayChallenge (
                    () => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        GameATools.LocalUseEnergy (LocalUser.Instance.MatchUserData.CurMatchChallengeCount);
                    },
                    () => {
                        // todo error handle
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    }
                );
            }
        }

        private void OnRandomPickBtn () {
            if (MatchUserData.EChallengeState.ChanceReady != LocalUser.Instance.MatchUserData.CurrentChallengeState())
                return;
            _randomPickTimer = _randomPickStateTime;
            _cachedView.InputBlock.gameObject.SetActive (true);
            RemoteCommands.GetMatchChallengeProject (
                0,
                msg => {
                    if ((int)EGetMatchChallengeProjectCode.GMCPC_Success == msg.ResultCode) {
                        LocalUser.Instance.MatchUserData.EasyChallengeProjectData.OnSyncFromParent(msg.EasyChallengeProjectData);
                        LocalUser.Instance.MatchUserData.MediumChallengeProjectData.OnSyncFromParent(msg.MediumChallengeProjectData);
                        LocalUser.Instance.MatchUserData.DifficultChallengeProjectData.OnSyncFromParent(msg.DifficultChallengeProjectData);
                        LocalUser.Instance.MatchUserData.RandomChallengeProjectData.OnSyncFromParent(msg.RandomChallengeProjectData);
                        LocalUser.Instance.MatchUserData.LeftChallengeCount--;
                        LocalUser.Instance.MatchUserData.CurSelectedChallengeType = (int)EChallengeProjectType.CPT_None;
                    } else {
                        SocialGUIManager.ShowPopupDialog("随机挑战关卡失败，原因代码：" + msg.ResultCode.ToString());
                        _randomPickTimer = 0.01f;
                    }
                },
                code => {
                    // todo error handle
                    SocialGUIManager.ShowPopupDialog("随机挑战关卡失败，原因代码：" + code.ToString());
                    _randomPickTimer = 0.01f;
                }
            );
        }

        private void OnAbandomBtn () {
            if (MatchUserData.EChallengeState.Challenging != LocalUser.Instance.MatchUserData.CurrentChallengeState())
                return;
            SocialGUIManager.ShowPopupDialog(
                string.Format("确定放弃这次挑战吗？使用的挑战机会不会退回"),
                "请确定",
                new KeyValuePair<string, Action>("确定", () => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
                    RemoteCommands.MatchSkipChallenge(
                        0,
                        msg => {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                            if (msg.ResultCode == (int)EMatchSkipChallengeCode.MSCC_Success) {
                                LocalUser.Instance.MatchUserData.OnAbandomChallengeSuccess();
                                Refresh();
                            } else {
                                // error handle
                            }
                        },
                        code => {
                            // error handle
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        }
                    );
                }),
                new KeyValuePair<string, Action>("取消", null)
            );
        }

        #endregion 接口
        private void OnChallengeProjectSelected (EChallengeProjectType type) {
            if (LocalUser.Instance.MatchUserData.CurrentChallengeState() == MatchUserData.EChallengeState.Selecting) {
//                _selectChallengeTemp = type;
                SocialGUIManager.ShowPopupDialog(
                    string.Format("确定选择 {0} 进行挑战吗？首次选择免费，重选需要支付金币", type),
                    "请确定",
                    new KeyValuePair<string, Action>("确定", () => {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
                        RemoteCommands.SelectMatchChallengeProject(
                            type, false, 
                            msg => {
                                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                                if (msg.ResultCode == (int)ESelectMatchChallengeProjectCode.SMCPC_Success) {
                                    LocalUser.Instance.MatchUserData.CurSelectedChallengeType = (int)type;
                                    Refresh();
                                } else {
                                    // error handle
                                }
                            },
                            code => {
                                // error handle
                                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                            }
                        );
                    }),
                    new KeyValuePair<string, Action>("取消", null)
                );
            } else if (LocalUser.Instance.MatchUserData.CurrentChallengeState() == MatchUserData.EChallengeState.Challenging) {
                if (LocalUser.Instance.MatchUserData.CurSelectedChallengeType != (int)type) {
                    SocialGUIManager.ShowPopupDialog(
                        string.Format("确定花费 {0} 金币将挑战关卡改为 {1} 吗？", 10, type),
                        "请确定",
                        new KeyValuePair<string, Action>("确定", () => {
                            if (GameATools.CheckGold(10)) {
                                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
                                RemoteCommands.SelectMatchChallengeProject(
                                    type, true, 
                                    msg => {
                                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                                        if (msg.ResultCode == (int)ESelectMatchChallengeProjectCode.SMCPC_Success) {
                                            LocalUser.Instance.MatchUserData.CurSelectedChallengeType = (int)type;
                                            GameATools.LocalUseGold(10);
                                            Refresh();
                                        } else {
                                            // error handle
                                        }
                                    },
                                    code => {
                                        // error handle
                                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                                    }
                                );
                            }
                        }),
                        new KeyValuePair<string, Action>("取消", null)
                    );
                }
            } else {
            }
        }

        private void OnReturnToApp () {
            if (_isOpen) {
                Refresh ();
            }
        }
        #endregion

    }
}
