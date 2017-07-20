  /********************************************************************
  ** Filename : UIViewForgetPassword.cs
  ** Author : quan
  ** Date : 2016/6/24 16:55
  ** Summary : UIViewForgetPassword.cs
  ***********************************************************************/
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewForgetPassword : UIViewBase
    {
        public InputField Phone;
        public InputField Pwd;
        public InputField SmsCode;
        public Button GetSMSCode;
        public Text GetSMSCodeBtnLabel;
        public Button DoResetPassword;
        public Button CloseBtn;
    }
}