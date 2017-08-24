
using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlMail: UMCtrlBase<UMViewMail>
    {
        private Mail _mail;

        public void Set(Mail mail)
        {
            _mail = mail;
            //_cachedView.MailSource.text = JudgeSource(mail);
            _cachedView.Title.text = mail.Title;
            _cachedView.MainBody.onClick.AddListener(OnButton);
            _cachedView.Date.text = GameATools.GetYearMonthDayHourMinuteSecondByMilli(mail.CreateTime, 1);
            if (mail.ReadFlag == true)
            {
                _cachedView.ReadFlag.gameObject.SetActiveEx(true);

            }
            else
                _cachedView.ReadFlag.gameObject.SetActiveEx(false);

        }

        //private String JudgeSource(Mail mail)
        //{
        //    String source;
        //    if (mail.Type == EMailType.EMailT_System)
        //    {

        //        source = "系统邮件";
        //    }
        //    else
        //    {
        //        source = mail.UserInfo.NickName;
        //    }
        //    return source;
        //}

        private void OnButton()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlMailDetail>();
            SocialGUIManager.Instance.GetUI<UICtrlMailDetail>().Set(_mail);

        }



    }

}


   
