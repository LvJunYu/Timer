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

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.FetchBtn.onClick.AddListener(OnFentchBtn);
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
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

        public void RefreshView()
        {
            if (!_isOpen || _mail == null) return;
            _cachedView.FetchBtn.SetActiveEx(_mail.ReceiptedFlag);
            _reward = _mail.AttachItemList;
            _cachedView.Title.text = _mail.Title;
            _cachedView.Content.text = _mail.Content;
            _mail.ReadFlag = true;
            _idList.Clear();
            _idList.Add(_mail.Id);
            RemoteCommands.MarkMailRead(EMarkMailReadTargetType.EMMRC_List, _idList,
                null, null
            );
            UpdateReward(_reward);
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

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
        }

        private void OnFentchBtn()
        {
            RemoteCommands.ReceiptMailAttach(EReceiptMailAttachTargetType.ERMATT_List, _idList,
                ret =>
                {
                    _mail.ReceiptedFlag = false;
                    SocialGUIManager.Instance.CloseUI<UICtrlMailDetail>();
                }
                , code => { SocialGUIManager.ShowPopupDialog("领取奖励失败。"); }
            );
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }
    }
}