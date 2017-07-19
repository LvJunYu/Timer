/********************************************************************
** Filename : UIViewSignup
** Author : Dong
** Date : 2015/4/27 22:20:02
** Summary : UIViewSignup
***********************************************************************/

using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewSignup : UIViewBase
    {
        public InputField Phone;
        public InputField Pwd;
        public InputField SmsCode;
        public Button GetSMSCode;
        public Text GetSMSCodeBtnLabel;
        public Button DoSignup;
        public Button ReturnLogin;
        public Text DoSignupText;
        public Text RegistrationAgreement;

        public Button QQ;
        public Button Weibo;
        public Button WeChat;
    }
}