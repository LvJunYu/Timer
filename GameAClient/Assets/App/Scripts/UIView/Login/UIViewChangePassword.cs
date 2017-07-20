using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewChangePassword : UIViewBase
    {
        public InputField Phone;
        public InputField OldPwd;
        public InputField NewPwd;

        public InputField SmsCode;
        public Button GetSMSCode;
        public Text GetSMSCodeBtnLabel;
        public Button DoResetPassword;
        public Button ForgetPassword;
        public Button CloseBtn;
    }
}