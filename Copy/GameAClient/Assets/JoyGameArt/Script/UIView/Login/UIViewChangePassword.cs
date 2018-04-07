using UnityEngine.UI;

namespace GameA
{
    public class UIViewChangePassword : UIViewResManagedBase
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