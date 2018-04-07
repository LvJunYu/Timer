/********************************************************************
** Filename : UIViewSignup
** Author : Dong
** Date : 2015/4/27 22:20:02
** Summary : UIViewSignup
***********************************************************************/

using UnityEngine.UI;

namespace GameA
{
    public class UIViewSignup : UIViewResManagedBase
    {
        public InputField Phone;
        public InputField Pwd;
        public InputField SmsCode;
        public Button GetSMSCode;
        public Text GetSMSCodeBtnLabel;
        public Button DoSignup;
        public Button ReturnLogin;
        public Text DoSignupText;
    }
}