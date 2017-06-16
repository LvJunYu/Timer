  /********************************************************************
  ** Filename : UICtrlNickNameEditor.cs
  ** Author : quan
  ** Date : 2016/4/13 14:32
  ** Summary : UICtrlNickNameEditor.cs
  ***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlNickNameEditor : UISocialContentCtrlBase<UIViewNickNameEditor>, IUIWithTitle, IUIWithRightCustomButton
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
                string.Format("昵称长度{0}-{1}个字符，可包含中文、英文、数字、下划线和减号",
                    SoyConstDefine.MinNickNameLength,
                    SoyConstDefine.MaxNickNameLength));
            _cachedView.NickNameInput.characterLimit = SoyConstDefine.MaxNickNameLength;
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
                LogHelper.Error("UICtrlNickNameEditor OnOpen, argument error");
                return;
            }
            _cachedView.NickNameInput.text = tuple.Item1;
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
            return "昵称";
        }
        public UnityEngine.UI.Button GetRightButton()
        {
            return _cachedView.ConfirmButtonRes;
        }

//        public void OnRightButtonClick(UICtrlTitlebar titleBar)
//        {
//            string newNickName = _cachedView.NickNameInput.text;
//            CheckTools.ECheckNickNameResult result = CheckTools.CheckNickName(newNickName);
//            if(result == CheckTools.ECheckNickNameResult.TooLong)
//            {
//                CommonTools.ShowPopupDialog(string.Format("昵称长度不应超过{0}个字符，当前长度为{1}",
//                    SoyConstDefine.MaxNickNameLength, newNickName.Length));
//                return;
//            }
//            if(result == CheckTools.ECheckNickNameResult.TooShort)
//            {
//                CommonTools.ShowPopupDialog(string.Format("昵称长度不应小于{0}个字符，当前长度为{1}",
//                    SoyConstDefine.MinNickNameLength, newNickName.Length));
//                return;
//            }
//            if(result == CheckTools.ECheckNickNameResult.IllegalCharacter)
//            {
//                CommonTools.ShowPopupDialog(string.Format("非法字符请更换，昵称可包含中文、英文、数字、下划线和减号"));
//                return;
//            }
//            if(result == CheckTools.ECheckNickNameResult.Duplication)
//            {
//                CommonTools.ShowPopupDialog("昵称已经存在");
//                return;
//            }
//            _uiStack.OpenPrevious();
//            _callback.Invoke(newNickName);
//        }

        #endregion
    }
}
