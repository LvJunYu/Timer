using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlMailDetail : UICtrlGenericBase<UIViewMailDetail>
    {
        private Mail _mail;
        private List<long> _idList = new List<long>();
        private Reward _reward = new Reward();
        private USCtrlGameFinishReward[] _rewardCtrl;

        public void Set(Mail mail)
        {
            
            _mail = mail;
            _idList.Clear();
            _idList.Add(mail.Id);
            _reward = _mail.AttachItemList;
            _cachedView.MailSource.text = JudgeSource(mail);
            _cachedView.Title.text = mail.Title;
            _cachedView.MainBody.text = mail.Content;
            if (mail.ReceiptedFlag)
            {
                _cachedView.RewardObj.SetActiveEx(false);
            }
            else
                _cachedView.RewardObj.SetActiveEx(true);

            _cachedView.Date.text = GameATools.GetYearMonthDayHourMinuteSecondByMilli(mail.CreateTime, 1);
            mail.ReadFlag = true;
            RemoteCommands.MarkMailRead(
                EMarkMailReadTargetType.EMMRC_List,
                _idList,
                null, null
                );
            _cachedView.Fetch.onClick.AddListener(Fentch);
            _cachedView.Delete.onClick.AddListener(Delete);
            _cachedView.Close.onClick.AddListener(Close);

            _rewardCtrl = new USCtrlGameFinishReward[_cachedView.Rewards.Length];
            for (int i = 0; i < _cachedView.Rewards.Length; i++)
            {
                _rewardCtrl[i] = new USCtrlGameFinishReward();
                _rewardCtrl[i].Init(_cachedView.Rewards[i]);
            }

            UpdateReward(_reward);

        }

        private void UpdateReward(Reward reward)
        {
            if (null != reward && reward.IsInited)
            {
                int i = 0;
                for (; i < _rewardCtrl.Length && i < reward.ItemList.Count; i++)
                {
                    _rewardCtrl[i].Set(reward.ItemList[i].GetSprite(), reward.ItemList[i].Count.ToString()
                    );
                }
                for (; i < _rewardCtrl.Length; i++)
                {
                    _rewardCtrl[i].Hide();
                }
            }
            else
            {
                for (int i = 0; i < _rewardCtrl.Length; i++)
                {
                    _rewardCtrl[i].Hide();
                }
            }
        }


        public override void Close()
        {
            SocialGUIManager.Instance.GetUI<UICtrlMail>().LoadMyMailList();
            SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
           
        }

    


        private void Fentch()
        {
            RemoteCommands.ReceiptMailAttach(
                 EReceiptMailAttachTargetType.ERMATT_List,
                 _idList,
                 (ret)=>
                 {
                     _cachedView.RewardObj.SetActiveEx(false);

                 }
        , null
                 );

        }

        private void Delete()
        {
           RemoteCommands.DeleteMail(
           EDeleteMailTargetType.EDMTT_List,
           _idList,
                (ret) =>
                {
                    Close();
                    SocialGUIManager.Instance.GetUI<UICtrlMail>().LoadMyMailList();
                }, null
           );
        }

        private String JudgeSource(Mail mail)
        {
            if (mail.Type ==EMailType.EMailT_System)
            {

                return "系统邮件";
            }
            else
                return mail.UserInfo.NickName;
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

    }
}
