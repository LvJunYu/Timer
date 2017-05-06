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
    public class UICtrlChallengeMatch : UISocialCtrlBase<UIViewChallengeMatch>
    {
        #region 常量与字段


        #endregion

        #region 属性


        #endregion

        #region 方法
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
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

        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();

            //
        }
			

		private void Refresh () {
		}



        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.PopUpUI;
        }
			

		private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlChallengeMatch>();
		}

		private void OnPublishBtn () {
			
		}

        private void OnEditBtn () {
        }

        private void OnRepickBtn () {
            

        }

        #endregion 接口
        #endregion

    }
}
