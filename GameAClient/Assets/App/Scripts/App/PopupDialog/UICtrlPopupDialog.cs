  /********************************************************************
  ** Filename : UICtrlPopupDialog.cs
  ** Author : quan
  ** Date : 2015/6/5 7:53
  ** Summary : UICtrlPopupDialog
  ***********************************************************************/


using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using System;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPopupDialog : UICtrlGenericBase<UIViewPopupDialog>
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        protected override void InitEventListener ()
        {
            base.InitEventListener ();
            Messenger<string, string, KeyValuePair<string, Action>[]>.AddListener(SoyEngine.EMessengerType.ShowDialog, MessengerShowDialogHandler);
            Messenger<string, string, KeyValuePair<string, Action>[]>.AddListener(EMessengerType.ShowDialog, MessengerShowDialogHandler);
        }

        protected override void OnDestroy ()
        {
            Messenger<string, string, KeyValuePair<string, Action>[]>.RemoveListener(SoyEngine.EMessengerType.ShowDialog, MessengerShowDialogHandler);
            Messenger<string, string, KeyValuePair<string, Action>[]>.RemoveListener(EMessengerType.ShowDialog, MessengerShowDialogHandler);
            base.OnDestroy ();
        }

        private void MessengerShowDialogHandler(string msg, string title, KeyValuePair<string, Action>[] btnParam)
        {
            if (!this.IsOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPopupDialog>();
            }
            ShowDialog(msg, title, btnParam);
        }
        /// <summary>
        /// 最多支持三个按钮 string Action * 3
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <param name="title">Title.</param>
        /// <param name="btnParam">Button parameter.</param>
        public void ShowDialog(string msg, string title, params KeyValuePair<string, Action>[] btnParam)
        {
            if(!CheckParam(btnParam))
            {
                LogHelper.Warning("Show Dialog Param error, msg: " + msg);
                return;
            }
            if(!_isOpen)
            {
                Open(null);
            }
            UMCtrlDialog ctrl = new UMCtrlDialog();
            ctrl.Init(_cachedView.ContentDock);
            ctrl.Set(msg, title, btnParam);
        }

        public bool CheckParam(KeyValuePair<string, Action>[] btnParam)
        {
            if(btnParam.Length > 3)
            {
                return false;
            }
            for (int i = 0; i < btnParam.Length; i++)
            {
                if(string.IsNullOrEmpty(btnParam[i].Key))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}