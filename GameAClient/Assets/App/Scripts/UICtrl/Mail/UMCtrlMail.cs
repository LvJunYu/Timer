
using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;
using GameA;
using NewResourceSolution;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlMail: UMCtrlBase<UMViewMail>
    {
        private string _mailfetched = "icon_enclosure_d";
        private string _mailUnfetched = "icon_enclosure";
        private string _mailRead = "icon_mail_open";
        private string _mailUnRead = "icon_mail";
        private Mail _mail;

        public void Set(Mail mail)
        {
            _mail = mail;
            //_cachedView.MailSource.text = JudgeSource(mail);
            _cachedView.Title.text = mail.Title;
            _cachedView.MainBody.onClick.AddListener(OnButton);
            _cachedView.Date.text = GameATools.DateCount(mail.CreateTime);
            Sprite Flag = null;
            if (mail.ReadFlag == false)
            { 
                //未读
                if (ResourcesManager.Instance.TryGetSprite(_mailUnRead, out Flag))
                {
                    _cachedView.ReadFlag.sprite = Flag;
                }
            }
            else
            {
                if (ResourcesManager.Instance.TryGetSprite(_mailRead, out Flag))
                {
                    _cachedView.ReadFlag.sprite = Flag;
                }
            }

            if (mail.ReceiptedFlag == false)
            {
                //未接收
                if (ResourcesManager.Instance.TryGetSprite(_mailUnfetched, out Flag))
                {
                    _cachedView.RewardFlag.sprite = Flag;
                }
            }
            else
            {
                if (ResourcesManager.Instance.TryGetSprite(_mailfetched, out Flag))
                {
                    _cachedView.RewardFlag.sprite = Flag;
                }
            }
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


   
