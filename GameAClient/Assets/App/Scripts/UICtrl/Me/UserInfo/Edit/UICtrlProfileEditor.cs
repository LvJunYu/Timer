  /********************************************************************
  ** Filename : UICtrlProfileEditor.cs
  ** Author : quan
  ** Date : 2016/4/13 14:32
  ** Summary : UICtrlProfileEditor.cs
  ***********************************************************************/



using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlProfileEditor : UISocialContentCtrlBase<UIViewProfileEditor>, IUIWithTitle, IUIWithRightCustomButton
    {
        #region 常量与字段
        private Action<string> _callback;

        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            DictionaryTools.SetContentText(_cachedView.TipText,
                string.Format("签名长度不超过{0}个字符",
                    SoyConstDefine.MaxProfileLength));
            _cachedView.ProfileInput.characterLimit = SoyConstDefine.MaxProfileLength;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Tuple<string, Action<string>> tuple = parameter as Tuple<string, Action<string>>;
            if(tuple == null || tuple.Item2 == null)
            {
                LogHelper.Error("UICtrlProfileEditor OnOpen, argument error");
                return;
            }
            _cachedView.ProfileInput.text = tuple.Item1;
            _callback = tuple.Item2;
        }

        protected override void OnDestroy()
        {
        }

        #endregion

        #region 事件处理

        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "签名";
        }
        public UnityEngine.UI.Button GetRightButton()
        {
            return _cachedView.ConfirmButtonRes;
        }

//        public void OnRightButtonClick(UICtrlTitlebar titleBar)
//        {
//            string newProfile = _cachedView.ProfileInput.text;
//            if(newProfile.Length > SoyConstDefine.MaxProfileLength)
//            {
//                CommonTools.ShowPopupDialog(string.Format("签名长度不应超过{0}个字符，当前长度为{1}",
//                    SoyConstDefine.MaxProfileLength, newProfile.Length));
//                return;
//            }
//            _uiStack.OpenPrevious();
//            _callback.Invoke(newProfile);
//        }

        #endregion
    }
}
