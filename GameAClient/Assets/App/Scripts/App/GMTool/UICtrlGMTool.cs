 /********************************************************************
 ** Filename : UICtrlTitlebar.cs
 ** Author : quansiwei
 ** Date : 2015/5/6 21:02
 ** Summary : 公用标题栏
 ***********************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SoyEngine.Proto;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlGMTool : UICtrlGenericBase<UIViewGMTool>
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法
        public override void Open(object parameter)
        {
            base.Open(parameter);
        }
        public override void OnUpdate ()
        {
            base.OnUpdate ();
            if (_cachedView.InputObj.activeSelf && Input.GetKeyDown (KeyCode.Return)) {
                OnEnterBtn ();
            }
        }
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.EnterBtn.onClick.AddListener (OnEnterBtn);
            _cachedView.SwitchBtn.onClick.AddListener (OnSwitchBtn);
            _cachedView.Weapon.onClick.AddListener(OnWeaponBtn);
        }


        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void SendGMCmd (string cmd) {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Format("正在执行GM命令[{0}]", cmd));
            RemoteCommands.ExecuteCommand (
                LocalUser.Instance.UserGuid,
                cmd,
                msg => {
                    if (msg.ResultCode == (int)EExecuteCommandCode.ECC_Success) {
                        OnCmdSucceed();
                    } else {
                        OnCmdFailed();
                    }
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                },
                code => {
                    OnCmdFailed();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                }
            );
        }

        private void OnCmdSucceed () {
            _cachedView.SucceedImg.SetActiveEx (true);
            _cachedView.FailedImg.SetActiveEx (false);
        }

        private void OnCmdFailed () {
            _cachedView.SucceedImg.SetActiveEx (false);
            _cachedView.FailedImg.SetActiveEx (true);
        }
        private void OnEnterBtn () {
            string inputStr = _cachedView.InputField.text;
            _cachedView.InputField.text = string.Empty;
            if (string.IsNullOrEmpty (inputStr)) {
                OnCmdFailed ();
                return;
            } else {
                SendGMCmd (inputStr);
            }
        }

        private void OnSwitchBtn () {
            if (_cachedView.InputObj.activeSelf) {
                _cachedView.InputObj.SetActive (false);
            } else {
                _cachedView.InputObj.SetActive (true);
            }
        }
        private void OnWeaponBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlWeapon>();
        }

        #endregion
    }
}
