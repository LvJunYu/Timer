using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public  class UPCtrlQQBluePrivilegeIntroduce : UPCtrlQQBlueBase
    {
        
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
           _cachedView.IntroduceBlueBtn.onClick.AddListener(OnIntroduceBtn);
        }   
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Open()
        {
            base.Open();
            _isOpen = true;
          
        }

        public override void Close()
        {
            base.Close();
            _isOpen = false;
        }
        private void OnIntroduceBtn()
        {
           Application.OpenURL("http://gamevip.qq.com/?ADTAG=VIP.WEB.XXXX");
        }

    }
}