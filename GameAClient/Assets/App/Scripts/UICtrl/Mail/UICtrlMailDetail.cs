using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlMailDetail : UICtrlGenericBase<UIViewMailDetail>
    {
        private const string _friendMail = "好友邮件";
        private const string _systemMail = "系统邮件";
        private const string _detialMail = "邮件详情";
        private const string _ok = "确 定";
        private const string _receive = "接 受";
        private const string _check = "查 看";
        private const string _get = "领 取";
        private const string _titleFormat = "<color=orange>{0}</color>给您分享了一个关卡";
        private const string _contentFormat = "<color=orange>{0}</color>给您分享了一个很有意思的关卡：{1}";
        private Mail _mail;
        private List<long> _idList = new List<long>();
        private Reward _reward = new Reward();
        private USCtrlGameFinishReward[] _rewardCtrl;
        private Project _project;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteBtn);
            _cachedView.ProjectBtn.onClick.AddListener(OnOKBtn);
            _rewardCtrl = new USCtrlGameFinishReward[_cachedView.Rewards.Length];
            for (int i = 0; i < _cachedView.Rewards.Length; i++)
            {
                _rewardCtrl[i] = new USCtrlGameFinishReward();
                _rewardCtrl[i].Init(_cachedView.Rewards[i]);
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _mail = parameter as Mail;
            RefreshView();
        }

        protected override void OnClose()
        {
            if (_mail.FuncType == EMailFuncType.MFT_ShareProject)
            {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.ProjectRawImage,
                    _cachedView.DefaltTexture);
            }
            base.OnClose();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainPopUpUI2;
        }

        public void RefreshView()
        {
            if (!_isOpen || _mail == null) return;
            _idList.Clear();
            _idList.Add(_mail.Id);
            _mail.ReadFlag = true;
            _cachedView.UITitle.text = GetUITitle();
            _cachedView.OKBtnTxt.text = GetOKBtnText();
            RemoteCommands.MarkMailRead(EMarkMailReadTargetType.EMMRC_List, _idList, _mail.MailType, null, null);
            _cachedView.RewardPannel.SetActive(_mail.FuncType == EMailFuncType.MFT_Reward);
            _cachedView.ProjectPannel.SetActive(_mail.FuncType == EMailFuncType.MFT_ShareProject);
            _cachedView.ShadowBattlePannel.SetActive(_mail.FuncType == EMailFuncType.MFT_ShadowBattleHelp);
            _cachedView.NormalPannel.SetActive(_mail.FuncType == EMailFuncType.MFT_None);
            RefreshPannel();
        }

        private void RefreshPannel()
        {
            if (_mail.FuncType == EMailFuncType.MFT_ShareProject)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在读取关卡数据");
                _project = new Project();
                _project.Request(_mail.ContentId,
                    () =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        _cachedView.Title.text = GetMailTile();
                        _cachedView.Content.text = GetMailDesc();
                        ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectRawImage, _project.IconPath,
                            _cachedView.DefaltTexture);
                    },
                    code =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
                        SocialGUIManager.ShowPopupDialog("请求关卡数据失败。");
                    });
            }
            else if (_mail.FuncType == EMailFuncType.MFT_Reward)
            {
                _cachedView.Title.text = GetMailTile();
                _cachedView.Content.text = GetMailDesc();
                _reward = _mail.AttachItemList;
                UpdateReward(_reward);
            }
            else
            {
                _cachedView.Title.text = GetMailTile();
                _cachedView.Content.text = GetMailDesc();
            }
        }

        private string GetMailTile()
        {
            if (_mail.FuncType == EMailFuncType.MFT_ShareProject)
            {
                return string.Format(_titleFormat, _mail.UserInfoDetail.UserInfoSimple.NickName);
            }
            return _mail.Title;
        }

        private string GetMailDesc()
        {
            if (_mail.FuncType == EMailFuncType.MFT_ShareProject)
            {
                return string.Format(_contentFormat, _mail.UserInfoDetail.UserInfoSimple.NickName, _project.ShortId);
            }
            return _mail.Content;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
        }

        private void OnOKBtn()
        {
            if (_mail.FuncType == EMailFuncType.MFT_Reward)
            {
                RemoteCommands.ReceiptMailAttach(EReceiptMailAttachTargetType.ERMATT_List, _idList, _mail.MailType,
                    ret =>
                    {
                        _mail.ReceiptedFlag = false;
                        SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
                    }
                    , code => { SocialGUIManager.ShowPopupDialog("领取奖励失败。"); }
                );
            }
            else if (_mail.FuncType == EMailFuncType.MFT_ShadowBattleHelp)
            {
            }
            else if (_mail.FuncType == EMailFuncType.MFT_ShareProject)
            {
                if (_project != null)
                {
                    SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(_project);
                }
            }
            else
            {
                SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
            }
        }

        private void OnDeleteBtn()
        {
            RemoteCommands.DeleteMail(EDeleteMailTargetType.EDMTT_List, _idList, _mail.MailType,
                msg =>
                {
                    if (msg.ResultCode == (int) EDeleteMailCode.EDMC_Success)
                    {
                        SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
                        Messenger.Broadcast(EMessengerType.OnMailListChanged);
                    }
                    else
                    {
                        SocialGUIManager.ShowPopupDialog("删除失败。");
                    }
                }, code => { SocialGUIManager.ShowPopupDialog(string.Format("删除失败。错误代码{0}", code)); });
        }

        private string GetOKBtnText()
        {
            if (_mail.FuncType == EMailFuncType.MFT_Reward)
            {
                return _get;
            }
            if (_mail.FuncType == EMailFuncType.MFT_ShadowBattleHelp)
            {
                return _receive;
            }
            if (_mail.FuncType == EMailFuncType.MFT_ShareProject)
            {
                return _check;
            }
            return _ok;
        }

        private void UpdateReward(Reward reward)
        {
            if (null != reward && reward.IsInited)
            {
                int i = 0;
                for (; i < _rewardCtrl.Length && i < reward.ItemList.Count; i++)
                {
                    _rewardCtrl[i].Set(reward.ItemList[i].GetSprite(), RewardInfo(reward.ItemList[i])
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

        private string RewardInfo(RewardItem rewardItem)
        {
            switch (rewardItem.Type)
            {
                //8E6F54FF F0954AFF
                //"<color=#8E6F54FF>" + "金币" + "</color>" + rewardItem.Count;
                case (int) ERewardType.RT_Gold:
                    return "<color=#8E6F54FF>" + "金币" + "</color>" + rewardItem.Count;
                case (int) ERewardType.RT_Diamond:
                    return "<color=#8E6F54FF>" + "钻石" + "</color>" + rewardItem.Count;
                case (int) ERewardType.RT_PlayerExp:
                    return "<color=#8E6F54FF>" + "冒险经验" + "</color>" + rewardItem.Count;
                case (int) ERewardType.RT_CreatorExp:
                    return "<color=#8E6F54FF>" + "工匠经验" + "</color>" + rewardItem.Count;
                case (int) ERewardType.RT_FashionCoupon:
                    return "<color=#8E6F54FF>" + "时装券" + "</color>" + rewardItem.Count;
                case (int) ERewardType.RT_RaffleTicket:
                    return "<color=#8E6F54FF>" + "抽奖券" + "</color>" + rewardItem.Count;
                case (int) ERewardType.RT_RandomReformUnit:
                    return "<color=#8E6F54FF>" + "地块" + "</color>" + rewardItem.Count;
                //case (int)ERewardType.RT_RandomReformUnit:
                //    return string.Format("地块{0}", rewardItem.Count);
                default:
                    return rewardItem.Count.ToString();
            }
        }

        private string GetUITitle()
        {
            if (_mail.MailType == EMailType.EMailT_Friend)
            {
                return _friendMail;
            }
            if (_mail.MailType == EMailType.EMailT_System)
            {
                return _systemMail;
            }
            return _detialMail;
        }
    }
}