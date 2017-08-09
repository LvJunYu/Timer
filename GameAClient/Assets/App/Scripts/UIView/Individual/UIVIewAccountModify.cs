  /********************************************************************
  ** Filename : UIVIewAccountModify.cs
  ** Author : quan
  ** Date : 2016/6/30 16:24
  ** Summary : UIVIewAccountModify.cs
  ***********************************************************************/
using System;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIVIewAccountModify : UIViewBase
    {
        public Button PhoneNumBtn;
        public Text PhoneNumText;
        public Button ChangePwByOldPwBtn;
        public Button ChangePwBySmsCodeBtn;

        public Toggle WechatToggle;
        public Text WechatNameText;
        public Toggle QQToggle;
        public Text QQNameText;
        public Toggle WeiboToggle;
        public Text WeiboNameText;
    }
}

