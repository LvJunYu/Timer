using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using System.IO;
using GameA.Game;


namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMailDetail : UICtrlGenericBase<UIViewMailDetail>
    {
        private Mail _mail;
        List<long> idList = new List<long>();

        public void Set(Mail mail)
        {
            _mail = mail;
            idList.Add(mail.Id);
            _cachedView.MailSource.text = JudgeSource(mail);
            _cachedView.Title.text = mail.Title;
            _cachedView.MainBody.text = mail.Content;
            _cachedView.Date.text = GameATools.GetYearMonthDayHourMinuteSecondByMilli(mail.CreateTime, 1);
            mail.ReadFlag = true;
            RemoteCommands.MarkMailRead(
                EMarkMailReadTargetType.EMMRC_List,
                idList,
                null, null
                );
            _cachedView.Fetch.onClick.AddListener(Fentch);
            _cachedView.Delete.onClick.AddListener(Delete);

        }

        private void Fentch()
        {
            RemoteCommands.ReceiptMailAttach(
                 EReceiptMailAttachTargetType.ERMATT_List,
                 idList,
                 null, null
                 );

        }

        private void Delete()
        {
            RemoteCommands.DeleteMail(
           EDeleteMailTargetType.EDMTT_List,
           idList,
           null, null
           );

        }

        private String JudgeSource(Mail mail)
        {
            String source;
            if (mail.ReadFlag)
            {

                return source = "系统邮件";
            }
            else
                return mail.UserInfo.NickName;
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
        }
    }
}
